using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegionNG
{
    public class GlobalPipelineTest
    {
        public static void Test()
        {
            Console.WriteLine($"GlobalPipelineTest---------------------------------------");

            var gp = new GlobalPipeline();


            gp.PushJob(1, GlobalPipelineType.LOGIC_THREAD, (int Id) =>
            {
                Console.WriteLine($"LOGIC_THREAD threadId:{Thread.CurrentThread.ManagedThreadId}");
                return true;
            });

            gp.PushJob(1, GlobalPipelineType.DB_THREAD, (int Id) =>
            {
                Console.WriteLine($"DB_THREAD threadId:{Thread.CurrentThread.ManagedThreadId}");
                return true;
            });

            gp.PushJob(1, GlobalPipelineType.LOGIC_THREAD, (int Id) =>
            {
                Console.WriteLine($"LOGIC_THREAD threadId:{Thread.CurrentThread.ManagedThreadId}");
                return false;  // false 리턴시 실패처리
            });

            gp.FailJob(1, (int Id) =>
            {
                Console.WriteLine($"FailJob threadId:{Thread.CurrentThread.ManagedThreadId}");
                return true;
            });

            gp.BatchJob();


        }
    }
}
