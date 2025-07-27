using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace RegionNG;

public class MessageWorkerAsync
{
    private static int _IdCounter = -1;
    private int _Id;

    ProcessQueueAsync<ActionMsg> _processQueue;

    public MessageWorkerAsync()
    {
        _Id = Interlocked.Increment(ref _IdCounter);

        _processQueue = new ProcessQueueAsync<ActionMsg>(MessageProc, 1);
    }
    public int GetId()
    {
        return _Id;
    }

    private async ValueTask MessageProc(ActionMsg actionMsg)
    {
        actionMsg.Exec(GetId());
    }

    public void EnqueueLamda(Action<int> action)
    {
        _processQueue.Enqueue(new ActionMsg { _action = action });
    }

    //public async ValueTask EnqueueLamdaAsync(Action<int> action)
    //{
    //    await _processQueue.EnqueueAsync(new ActionMsg { _action = action });
    //}

    public void AddOneTimeTimer(long delayMs, Action<int> action)
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
