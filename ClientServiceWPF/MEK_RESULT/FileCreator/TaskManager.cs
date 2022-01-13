using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Presentation;

namespace ClientServiceWPF.MEK_RESULT.FileCreator
{
    public class TaskItem
    {
        public int ID { get; set; }

        public bool Free { get; set; } = true;

        public Task TSK { get; set; }
    }

    public class TaskManager
    {
        private List<TaskItem> tasks;

        public TaskManager(int count)
        {
            tasks = new List<TaskItem>();
            for (var i = 0; i < count; i++)
            {
                tasks.Add(new TaskItem());
            }
        }

        public TaskItem WaitFreeItem(CancellationToken cancel)
        {
            while (true)
            {
                var fr = FreeItem;
                if (fr != null) return fr;
                Task.Delay(500, cancel).Wait(cancel);
            }
        }

        public TaskItem FreeItem
        {
            get
            {
                Check();
                return tasks.FirstOrDefault(x => x.Free);
            }
        }

        public void Check()
        {
            foreach (var t in tasks)
            {
                if (t.TSK != null)
                {
                    if (t.TSK.IsCompleted)
                    {
                        t.Free = true;
                    }
                }
                else
                    t.Free = true;
            }
        }

        public bool IsSTOP
        {
            get
            {
                Check();
                return tasks.Count == tasks.Count(x => x.Free);
            }
        }

        public void WaitIsSTOP(CancellationToken cancel)
        {
            while (!IsSTOP)
            {
                Task.Delay(500, cancel).Wait(cancel);
            }
        }

        public void ThrowIfException()
        {
            var ex_task = tasks.Where(x => x.TSK?.Exception != null).SelectMany(x => x.TSK.Exception.InnerExceptions).ToList();
           
            if (ex_task.Count != 0)
            {
                throw new AggregateException(ex_task);
            }
                
        }

    }


    public class ParallelManager<T>
    {
        private Task mainTask;
        private Action<T> action;
        private SemaphoreSlim semaphore;
        private List<Exception> exceptions = new List<Exception>();

        public ParallelManager(IEnumerable<T> items, int countParallel, Action<T> action)
        {
            semaphore = new SemaphoreSlim(countParallel);
            this.action = action;
            mainTask = Task.Run(async () =>
            {
                try
                {
                    var list = items.ToList();
                    var tasks = new List<Task>();
                    for (var i = 0; i < list.Count; i++)
                    {
                        var item = list[i];
                        await semaphore.WaitAsync();
                       // Debug.WriteLine($"Запуск:{item}");
                        tasks.Add(RunActionAsync(item, i, list.Count));
                    }
                    Task.WaitAll(tasks.ToArray());
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            });
        }
        private Task RunActionAsync(T value, int index, int count)
        {
            return Task.Run(() =>
            {
                try
                {
                    action.Invoke(value);
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
                finally
                {
                    semaphore.Release();
                }
            });
        }


        public bool IsCompleted
        {
            get
            {
                if (exceptions.Count != 0)
                {
                    if (exceptions.Count == 1)
                        throw exceptions[0];
                    throw new AggregateException(exceptions);
                }
                return mainTask != null && mainTask.IsCompleted;
            }
        }

        public Task<bool> WaitCompletedAsync()
        {
            return Task.Run(WaitCompleted);
        }

        public bool WaitCompleted()
        {
            while (!IsCompleted)
            {

            }
            return true;
        }
    }

}
