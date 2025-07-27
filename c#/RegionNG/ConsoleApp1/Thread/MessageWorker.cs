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

        public void Exec(int Id)
        {
            _action?.Invoke(Id);
        }
    }

    public class MessageWorker
    {
        ProcessQueue<ActionMsg> _processQueue = new ProcessQueue<ActionMsg>(MessageProc, 1, 1024, "MessageWorker");

        private static void MessageProc(ActionMsg actionMsg)
        {
            actionMsg.Exec(0);
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
        Dictionary<int /*workerId*/ , MessageWorkerAsync> _messageWorkerDic = new();
        List<MessageWorkerAsync> _messageWorkerList = new();

        public MessageWorkerPack( int ThreadCount) 
        {
            for (int i = 0; i < ThreadCount; i++)
            {
                var msgWorker = new MessageWorkerAsync();
                _messageWorkerDic.Add(msgWorker.GetId(), msgWorker);
                _messageWorkerList.Add(msgWorker);
            }
        }

        public MessageWorkerAsync GetWorkerByKey(int key)
        {
            int index = key % _messageWorkerList.Count;

            return _messageWorkerList[index];
        }

        public MessageWorkerAsync GetWorkerByWorker(int key)
        {
            return _messageWorkerDic[key];
        }

    }


}
