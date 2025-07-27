using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegionNG
{
    public class ProcessQueueTest
    {
        public class TestItem
        {
            public int _worker = 0;
            public int _idx = 0;
        }

        public static void ExecFunc(TestItem item)
        {
            Console.WriteLine($"idx:{item._idx} ThreadId:{Thread.CurrentThread.ManagedThreadId}");
        }

        public static void Test()
        {
            Console.WriteLine($"ProcessQueueTest---------------------------------------");

            ProcessQueue<TestItem> _processQueue = new ProcessQueue<TestItem>(ExecFunc, 1, 1024, "TestQueue");

            for (int i = 0; i < 10; i++)
            {
                _processQueue.Enqueue(new TestItem { _idx = i });
            }
        }

        public static async ValueTask ExecFuncAsync(TestItem item)
        {
            Console.WriteLine($"_worker:{item._worker} idx:{item._idx} async ThreadId:{Thread.CurrentThread.ManagedThreadId}");

            // 하나의 쓰레드에서 처리가 되는지 확인하기 위함
            if (item._idx % 2 == 0)
            {
                await Task.Delay(100);
            }
        }

        public static void TestAsync(int worker = 0)
        {
            Console.WriteLine($"ProcessQueueAsyncTest---------------------------------------");

            ProcessQueueAsync<TestItem> _processQueue = new ProcessQueueAsync<TestItem>(ExecFuncAsync);

            for (int i = 0; i < 10; i++)
            {
                _processQueue.Enqueue(new TestItem
                {
                    _worker = worker,
                    _idx = i
                });
            }
        } 

        

    }

    //public class ProcessQueueTestByTask
    //{
    //    public class TestItem
    //    {
    //        public int _idx = 0;
    //    }

    //    public static void ExecFunc(TestItem item)
    //    {
    //        Console.WriteLine($"idx:{item._idx} ThreadId:{Thread.CurrentThread.ManagedThreadId}");
    //    }

    //    public static void Test()
    //    {
    //        Console.WriteLine($"ProcessQueueTest---------------------------------------");

    //        ProcessQueue<TestItem> _processQueue = new ProcessQueue<TestItem>(ExecFunc, 4, 1024, "TestQueue");


    //        for (int i = 0; i < 10; i++)
    //        {
    //            _processQueue.Enqueue(new TestItem { _idx = i });
    //        }


    //    }
    //}

}


