using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace RegionNG
{


    public class MessageWorkerTask<T> where T : class, new()
    {

        private readonly Channel<Func<T, Task>> _channel;
        private RegionTask _regionTask;
        private TaskFactory _taskFactory = new TaskFactory(new RegionScheduler());
        private Action _action;

        public MessageWorkerTask(Action action)
        {
            _channel = Channel.CreateUnbounded<Func<T, Task>>();
            _action = action;
            //RegionTask.CreateTask(() => await ProcessMessagesAsync() ,  "regionWorker");

            _taskFactory.StartNew(action);
        }

        private async Task ProcessMessagesAsync()
        {
            while (_channel.Reader.TryRead(out var func))
            {
                try
                {
                    //await func(0); // id 값이 필요하면 전달
                }
                catch (Exception ex)
                {
                    // 로깅 등 예외 처리
                }
            }

        }

        public void EnqueueLambda(Func<int, Task> func)
        {
            //_channel.Writer.TryWrite(func);
        }


        public void AddOneTimeTimer(long delayMs, Func<int, Task> action)
        {
            if (delayMs > 0)
            {
                ProcessTimer.AddOneTimeTimer(delayMs, (int Id) =>
                {
                    AddOneTimeTimer(0, action);
                    return true;
                });
                return;
            }

            EnqueueLambda(action);
        }

        public void AddPeriodicTimer(long delayMs, Func<int, Task> action)
        {
            ProcessTimer.AddPeriodicTimer(delayMs, (int Id) =>
            {
                EnqueueLambda(action);
                return true;
            });
        }




    }

}
