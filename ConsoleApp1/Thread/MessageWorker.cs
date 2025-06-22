using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegionNG
{

    public struct ActionMsg
    {
        public Action<int> _action;

        public void Exec()
        {
            _action?.Invoke(0);
        }
    }

    public class MessageWorker
    {
        ProcessQueue<ActionMsg> _processQueue = new ProcessQueue<ActionMsg>(MessageProc, 1, 1024, "MessageWorker");

        private static void MessageProc(ActionMsg actionMsg)
        {
            actionMsg.Exec();
        }

        public void EnqueueLamda(Action<int> action)
        {
            _processQueue.Enqueue(new ActionMsg { _action = action });
        }

        public void AddOneTimeTimer(long delayMs, Action<int> action)
        {
            if(delayMs > 0 )
            {
                ProcessTimer.AddOneTimeTimer(delayMs, (int Id) =>
                {
                    AddOneTimeTimer(0, action);
                    return true;
                });
                return;
            }

            EnqueueLamda((int id) =>
            {
                action(id);
            });
        }

        public void AddPeriodicTimer(long delayMs, Action<int> action)
        {
            ProcessTimer.AddPeriodicTimer(delayMs, (int Id) =>
            {
                EnqueueLamda((int id) =>
                {
                    action(id);
                });
                return true;
            });
        }


    }

    public class MessageWorkerPack
    {
        Dictionary<int /*threadId*/ , MessageWorker> _messageWorkerDic = new();
        List<int> _threadIdList = new List<int>();
        object _lock = new object();
        
        public MessageWorkerPack( int ThreadCount) 
        {
            for (int i = 0; i < ThreadCount; i++)
            {
                var msgWorker = new MessageWorker();
                msgWorker.EnqueueLamda((Id) =>
                {
                    lock (_lock)
                    {
                        _messageWorkerDic.Add(Thread.CurrentThread.ManagedThreadId, msgWorker);
                        _threadIdList.Add(Thread.CurrentThread.ManagedThreadId);
                    }
                });
            }
        }

        public MessageWorker GetWorker(int key)
        {
            int threadId = _threadIdList[key % _threadIdList.Count];

            return _messageWorkerDic[threadId];
        }

    }


}
