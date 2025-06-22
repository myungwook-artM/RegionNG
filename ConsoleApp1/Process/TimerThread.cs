using System;
using System.Collections.Concurrent;
using System.Collections;
using System.Threading;

namespace RegionNG
{

    public abstract class IOTimerBase
    {
        public Func<int, bool>? _func;
        public long _delayMs = 0;
        public DateTime _execDate;
        public DateTime _endDate = DateTime.MaxValue;

        //public long Cost => _execDate.Ticks;

        public abstract void TimerExpired(int id);
    }
       
    
    public class TimerPermanment : IOTimerBase
    {
        public override void TimerExpired(int id)
        {
            throw new NotImplementedException();
        }
    }

    public class TimerOneTime : IOTimerBase
    {
        public override void TimerExpired(int id)
        {
            _func?.Invoke(id);
        }
    }

    public class TimerPeriodic : IOTimerBase
    {
        public override void TimerExpired(int id)
        {
            _func?.Invoke(id);

            _execDate = DateTime.Now.AddMilliseconds( _delayMs );        
            
            ProcessTimer.AddTimer(this);
        }
    }
    

    public static class ProcessTimer 
    {
        private static DateTime _topDate = DateTime.Now.AddDays(365);
        private static PriorityQueue<IOTimerBase, DateTime> _priorityQueue = new( 1024);
        private static ProcessQueueAsync<IOTimerBase> _processQueue = new (ProcessQueueThread, 4);
        private static ConcurrentBag<IOTimerBase> _bufferQueue = new ();
        private static bool _isPrcess= true;

        public static void InitProcessTimer()
        {
            _bufferQueue.Add(new TimerPermanment { _execDate = _topDate });

            ManagedThread.CreateThread(TimerCheckerThreadProc, "TimerCheckerThread");
        }
        public static ValueTask ProcessQueueThread(IOTimerBase timerObject)
        {
            timerObject.TimerExpired(0);
            return ValueTask.CompletedTask;
        }

        public static void StopThread()
        {
            _isPrcess = false;
        }

        private static void TimerCheckerThreadProc()
        {
            while(_isPrcess)
            {
                DateTime nowDate = DateTime.Now;
                while( _bufferQueue.TryTake(out var timerObj))
                {
                    if( timerObj._execDate <= nowDate )
                    {
                        _processQueue.Enqueue(timerObj);
                        continue;
                    }
                    else
                    {
                        _priorityQueue.Enqueue(timerObj, timerObj._execDate);

                        if( _topDate > timerObj._execDate )
                        {
                            _topDate = timerObj._execDate;
                        }
                    }
                }

                if( _topDate <= nowDate )
                {
                    while(true)
                    {
                        var top = _priorityQueue.Dequeue();
                        _processQueue.Enqueue(top);

                        top = _priorityQueue.Peek();
                        if( top._execDate > nowDate )
                        {
                            _topDate = top._execDate;
                            break;
                        }
                    }
                }

                Thread.Sleep(1);
            }
        }

        
        public  static void AddTimer(IOTimerBase timerObject)
        {
            timerObject._execDate = DateTime.Now.AddMilliseconds(timerObject._delayMs);
            _bufferQueue.Add(timerObject);
        }

        public static void AddPeriodicTimer(long delayMs,  Func<int, bool> timerFunc)
        {
            var periodicTimer = new TimerPeriodic {  _delayMs = delayMs,  _func = timerFunc };

            ProcessTimer.AddTimer(periodicTimer);
        }

        public static void AddOneTimeTimer(long delayMs, Func<int, bool> timerFunc)
        {
            var onetimeTimer = new TimerOneTime{ _delayMs = delayMs, _func = timerFunc };

            ProcessTimer.AddTimer(onetimeTimer);
        }

        
    }

  
}
