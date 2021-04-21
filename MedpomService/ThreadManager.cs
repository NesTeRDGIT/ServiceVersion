using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MedpomService
{
    public class ThreadItem<T>
    {
        public Thread Th { get; set; }
        public T Param { get; set; }
    }
    public class ThreadManager<T>
    {
        List<ThreadItem<T>> listCloser = new List<ThreadItem<T>>();

        public void AddListTh(Thread th, T param)
        {
            listCloser.Add(new ThreadItem<T> { Param = param, Th = th });
        }
        public void RemoveTh(Thread th)
        {
            var item = listCloser.FirstOrDefault(x => x.Th == th);
            listCloser.Remove(item);
        }

        public List<ThreadItem<T>> Get()
        {
            return listCloser;
        }
    }

}
