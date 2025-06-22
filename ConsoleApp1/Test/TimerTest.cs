using RegionNG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegionNG
{
    static class TimerTest
    {
        static int count = 0;
        public static void Test()
        {
            Console.WriteLine($"TimerTest---------------------------------------");

            // 타이머 쓰레드 생성
            ProcessTimer.InitProcessTimer();

            // 2초 이후 실행하는 타이머 추가
            ProcessTimer.AddOneTimeTimer(2000, (int id) =>
            {
                Console.WriteLine($"2000ms OneTimeTimer");
                return true;
            });

            // 1초 이후 실행하는 타이머 추가
            ProcessTimer.AddOneTimeTimer(1000, (int id) =>
            {
                Console.WriteLine($"1000ms OneTimeTimer");
                return true;
            });


            // 1초마다 반복하는 타이머 추가
            ProcessTimer.AddPeriodicTimer(1000, (int id) =>
            {
                Console.WriteLine($"1000ms PeriodicTimer");

                // 5초후 중단
                if( count++ > 5)
                {
                    return false; // 루프 중단
                }
                return true;
            });

        }
    }
}
