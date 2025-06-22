using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegionNG
{
    //public class RegionScheduler : TaskScheduler
    //{
    //    private int _maxConcurrencyThreads = 0;
    //    public RegionScheduler(int maxConcurrencyThreads = 1)
    //    {
    //        _maxConcurrencyThreads = maxConcurrencyThreads;
    //    }
    //    protected override void QueueTask(System.Threading.Tasks.Task task)
    //    {
    //        TryExecuteTask(task);
    //    }
        
    //    protected override bool TryExecuteTaskInline(System.Threading.Tasks.Task task, bool taskWasPreviouslyQueued)
    //    {
    //        return TryExecuteTask(task);
    //    }

    //    protected override IEnumerable<System.Threading.Tasks.Task> GetScheduledTasks()
    //    {
    //        return Enumerable.Empty<System.Threading.Tasks.Task>();
    //    }

    //    /// <summary>이 TaskScheduler가 지원할 수 있는 최대 동시성 수준을 나타냅니다.</summary>
    //    public override int MaximumConcurrencyLevel { get { return _maxConcurrencyThreads; } }
    //}
}
