using System;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Channels;
using RegionNG;

public class ProcessQueueAsync<T>
{
    private Queue<T> _queue;
    private readonly Func<T, ValueTask> _actionAsync;
    private readonly TaskScheduler _scheduler;
    private Channel<T> _channel;
    private CancellationTokenSource _cts;

    public int Count => _channel.Reader.Count;

    public ProcessQueueAsync(Func<T, ValueTask> lamdaFunc, TaskScheduler scheduler = null)
    {
        _channel = Channel.CreateUnbounded<T>();
        //_scheduler = scheduler ?? TaskScheduler.Default;
        _scheduler = new ConcurrentExclusiveSchedulerPair(TaskScheduler.Default, 4).ExclusiveScheduler;

        _actionAsync = lamdaFunc;

        Task.Factory.StartNew(
            () => ProcessLoopAsync(CancellationToken.None),
            CancellationToken.None,
            TaskCreationOptions.None,
            _scheduler
        );
        //    .Unwrap().ContinueWith(t =>
        //{
        //    if (t.IsFaulted)
        //    {
        //        // 예외 처리 또는 로깅
        //        Console.WriteLine(t.Exception);
        //    }
        //}, TaskScheduler.Default);
    }

    public async ValueTask EnqueueAsync(T item)
    {
        await _channel.Writer.WriteAsync(item);
    }

    private async ValueTask ProcessLoopAsync(CancellationToken token)
    {
        var reader = _channel.Reader;
        while (await reader.WaitToReadAsync(token))
        {
            while (reader.TryRead(out var item))
            {
                try
                {
                    await _actionAsync(item);
                }
                catch (Exception ex)
                {
                    // 예외 처리 또는 로깅
                    Console.WriteLine(ex);
                }
            }
        }
    }

}
