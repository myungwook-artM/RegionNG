using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegionNG
{
    public enum GlobalPipelineType
    {
        LOGIC_THREAD = 1,
        DB_THREAD = 2,
        TIMER_THREAD = 3,
    }

    public class PipelineJob
    {
        public int _key;
        public Func<int, bool> _action;
        public GlobalPipelineType _type;

        public bool Exec()
        {
            return _action.Invoke(0);
        }
    }

    public class GlobalPipeline
    {
        private Queue<PipelineJob> _queue = new();
        private PipelineJob _failjob = new();

        public void PushJob(int key, GlobalPipelineType type, Func<int, bool> action)
        {
            _queue.Enqueue(new PipelineJob { _key = key, _type = type, _action = action });
        }

        public void FailJob(int key, Func<int, bool> action)
        {
            _failjob = new PipelineJob {  _key = key, _type = GlobalPipelineType.LOGIC_THREAD, _action = action };
        }


        private bool ExecJob(PipelineJob job)
        {
            if (job == null)
                return false;

            switch( job._type)
            {
                case GlobalPipelineType.LOGIC_THREAD:
                    ExecJobForLogicThread(job);
                    break;
               case GlobalPipelineType.DB_THREAD:
                    ExecJobForDBThread(job);
                    break;
                case GlobalPipelineType.TIMER_THREAD:
                    ExecJobForTimer(job);
                    break;


            }
            return true;
        }

        private void ExecJobForLogicThread(PipelineJob job)
        {
            var worker = LogicThread.Instance.GetWorker(job._key);

            worker.EnqueueLamda( ( int Id) =>
            {
                InternalExecJob(job);
            });
        }

        private void ExecJobForDBThread(PipelineJob job)
        {
            var worker = DBThread.Instance.GetWorker(job._key);

            worker.EnqueueLamda((int Id) =>
            {
                InternalExecJob(job);
            });
        }

        private void ExecJobForTimer(PipelineJob job)
        {
            ProcessTimer.AddOneTimeTimer(0, (int Id) =>
            {
                InternalExecJob(job);
                return true;
            });

        }

        private  bool InternalExecJob(PipelineJob job)
        {
            if (false == job.Exec())
            {
                _failjob?.Exec();
                return false;            
            }
            
            BatchJob();
            return true;

        }
        public void BatchJob()
        {
            if( _queue.Count == 0)
            {
                return;
            }

            var job = _queue.Dequeue();
            ExecJob(job);
        }
    }
}
