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


            gp.PushJob(1, GlobalPipelineType.LOGIC_THREAD, async (int Id) =>
            {
                Console.WriteLine($"LOGIC_THREAD threadId:{Thread.CurrentThread.ManagedThreadId} ScheduleId: {Id}" );

                await Task.Delay(1000);  // 비동기 작업 예시
                return true;
            });

            gp.PushJob(1, GlobalPipelineType.DB_THREAD, async (int Id) =>
            {
                Console.WriteLine($"DB_THREAD threadId:{Thread.CurrentThread.ManagedThreadId} ScheduleId: {Id}");

                await Task.Delay(100);
                return true;
            });

            gp.PushJob(1, GlobalPipelineType.LOGIC_THREAD, async (int Id) =>
            {
                Console.WriteLine($"LOGIC_THREAD threadId:{Thread.CurrentThread.ManagedThreadId} ScheduleId: {Id}");

                await Task.Delay(10);
                return false;           // 실패를 유도하기 위해 false 반환
            });

            gp.FailJob(1, async (int Id) =>
            {
                Console.WriteLine($"FailJob threadId:{Thread.CurrentThread.ManagedThreadId}");
                return true;
            });

            gp.BatchJob();


        }
    }
}
