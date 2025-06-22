using System;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Channels;
using RegionNG;

public class ProcessQueueAsync<T>
{
    private readonly Func<T, ValueTask> _actionAsync;
    private ConcurrentExclusiveSchedulerPair _scheduler;
    private Channel<T> _channel = Channel.CreateUnbounded<T>(new UnboundedChannelOptions{
        SingleReader = true,
        SingleWriter = false,
    });

    private CancellationTokenSource _cts;

    public int Count => _channel.Reader.Count;

    public ProcessQueueAsync(Func<T, ValueTask> lamdaFunc, int threadCount = 1)
    {
        _actionAsync = lamdaFunc;
        _scheduler = new ConcurrentExclusiveSchedulerPair(TaskScheduler.Default, threadCount);


        // 쓰레드가 1개라면 배타적 스케줄
        // 쓰레드가 1개 이상이면 동시 스케줄
        Task.Factory.StartNew(
            () => ProcessLoopAsync(CancellationToken.None),
            CancellationToken.None,
            TaskCreationOptions.None,
            (threadCount == 1 ? _scheduler.ExclusiveScheduler : _scheduler.ConcurrentScheduler)
        );

    }

    /// <summary>
    /// 비동기적으로 아이템을 큐에 추가합니다.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public async ValueTask EnqueueAsync(T item)
    {
        await _channel.Writer.WriteAsync(item);
    }

    /// <summary>
    /// 동기적으로 아이템을 큐에 추가합니다.
    /// </summary>
    /// <param name="item"></param>
    public void Enqueue(T item)
    {
        _channel.Writer.TryWrite(item);
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
