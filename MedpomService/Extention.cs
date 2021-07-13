using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedpomService
{
    public static class TaskExtension
    {
        public static bool IsActive(this Task task)
        {
            return task.Status == TaskStatus.Running || task.Status == TaskStatus.WaitingForActivation || task.Status == TaskStatus.WaitingToRun;
        }
    }
}
