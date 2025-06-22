using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegionNG
{
    public class RegionTask
    {
        private string? _name;
        private int _threadId;
        private TaskFactory _taskFactory = new TaskFactory(new RegionScheduler());

        public static void CreateTask(Action action, string name, int idx = 0)
        {
            var thread = new RegionTask();
            thread._name = name;

            thread.RunTask(action);
            
            //thread.RunTask(() =>
            //{
            //    thread.InitThread();
            //});
        }
        //private void InitThread()
        //{
        //    _threadId = Thread.CurrentThread.ManagedThreadId;
        //}


        public void RunTask(Action action)
        {
            _taskFactory.StartNew(() =>
            {
                action.Invoke();
            });
        }
    }
}
