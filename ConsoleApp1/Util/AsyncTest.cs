using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegionNG
{
    public class AsyncTest
    {

        static public void TestOne()
        {
            
            Console.WriteLine($"1 {Thread.CurrentThread.ManagedThreadId}");

            TestNum();

            Console.WriteLine($"2 {Thread.CurrentThread.ManagedThreadId}");

        }
        static public async void TestNum()
        {
            Console.WriteLine($"3 {Thread.CurrentThread.ManagedThreadId}");

            await Task.Delay(1000).ConfigureAwait(false);

            Console.WriteLine($"4 {Thread.CurrentThread.ManagedThreadId}");
        }
    }
}
