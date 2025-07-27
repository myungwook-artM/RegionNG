using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RegionNG.ProcessQueueTest;


namespace RegionNG
{
    
    public class MemoryPoolTest
    {
        public class MemoryPoolItem
        {
            public int _idx = 0;
        }

        public static async ValueTask ExecFunc(MemoryPoolItem item)
        {
            var singleObj = SingleMemoryPool<MemoryPoolItem>.Pop();
            SingleMemoryPool<MemoryPoolItem>.Push(singleObj);

            var threadObj = ThreadMemoryPool<MemoryPoolItem>.Pop();
            ThreadMemoryPool<MemoryPoolItem>.Push(threadObj);

            Console.WriteLine($"idx:{item._idx} ThreadId:{Thread.CurrentThread.ManagedThreadId}");     
            
        }

        

        public static void Test()
        {
            ProcessQueueAsync<MemoryPoolItem> _processQueue = new ProcessQueueAsync<MemoryPoolItem>(ExecFunc, 4);

            for (int i = 0; i < 1000; i++)
            {
                _processQueue.Enqueue(new MemoryPoolItem { _idx = i });
            }

        }
    }
}
