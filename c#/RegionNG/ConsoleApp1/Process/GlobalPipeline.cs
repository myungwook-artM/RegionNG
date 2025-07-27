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
        public Func<int, ValueTask<bool>> _action;
        public GlobalPipelineType _type;

        public async ValueTask<bool> ExecAsync(int Id)
        {
            return await _action.Invoke(Id);
        }
    }

    public class GlobalPipeline
    {
        private Queue<PipelineJob> _queue = new();
        private PipelineJob _failjob = new();

        public void PushJob(int key, GlobalPipelineType type, Func<int, ValueTask<bool>> action)
        {
            _queue.Enqueue(new PipelineJob { _key = key, _type = type, _action = action });
        }

        public void FailJob(int key, Func<int, ValueTask<bool>> action)
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
            var worker = LogicThread.Instance.GetWorkerByKey(job._key);
            
            worker.EnqueueLamda(async ( int Id) =>
            {
                await InternalExecJob(Id, job );
            });
        }

        private void ExecJobForDBThread(PipelineJob job)
        {
            var worker = DBThread.Instance.GetWorkerByKey(job._key);

            worker.EnqueueLamda(async (int Id) =>
            {
                await InternalExecJob(Id, job );
            });
        }

        private void ExecJobForTimer(PipelineJob job)
        {
            //ProcessTimer.AddOneTimeTimer(0, async  (int Id) =>
            //{
            //    await InternalExecJob(Id, job);
            //    return true;
            //});
        }

        private async ValueTask<bool> InternalExecJob(int Id, PipelineJob job )
        {
            if (false == await job.ExecAsync(Id))
            {
                if (_failjob != null)
                    await _failjob.ExecAsync(Id);

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
