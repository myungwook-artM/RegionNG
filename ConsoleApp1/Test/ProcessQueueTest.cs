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
            public int _idx = 0;
        }

        public static void ExecFunc(TestItem item)
        {
            Console.WriteLine($"idx:{item._idx} ThreadId:{Thread.CurrentThread.ManagedThreadId}");
        }

        public static void Test()
        {
            Console.WriteLine($"ProcessQueueTest---------------------------------------");

            ProcessQueue<TestItem> _processQueue = new ProcessQueue<TestItem>(ExecFunc, 4, 1024, "TestQueue");

            for (int i = 0; i < 10; i++)
            {
                _processQueue.Enqueue(new TestItem { _idx = i });
            }
        }



        public static async ValueTask ExecFuncAsync(TestItem item)
        {
            Console.WriteLine($"idx:{item._idx} async ThreadId:{Thread.CurrentThread.ManagedThreadId}");

            await Task.Delay(10);
        }

        public static void TestAsync()
        {
            Console.WriteLine($"ProcessQueueAsyncTest---------------------------------------");

            ProcessQueueAsync<TestItem> _processQueue = new ProcessQueueAsync<TestItem>(ExecFuncAsync);

            for (int i = 0; i < 10; i++)
            {
                _processQueue.EnqueueAsync(new TestItem { _idx = i });
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


