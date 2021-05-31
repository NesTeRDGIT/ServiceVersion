using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
    }

}
