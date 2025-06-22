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
                return true;
            });

            // 5초 이후 ProcessTimer 종료 되도록 함
            ProcessTimer.AddOneTimeTimer(5000, (int id) =>
            {
                ProcessTimer.StopThread();
                return true;
            });

        }
    }
}
