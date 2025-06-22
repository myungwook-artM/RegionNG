using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegionNG.World
{
    internal class WorldMap
    {

        Dictionary<int, Zone> _zoneDic = new Dictionary<int, Zone>();   
        public WorldMap() { }

        public static void Test()
        {
            Console.WriteLine($"WorldMap---------------------------------------");
            // 월드맵 초기화
            var worldMap = new WorldMap();
            worldMap.Init();

        }

        public void Init()
        {
            // Initialize the world map here
            Console.WriteLine("World map initialized.");

            for( int i = 0; i < 2; i++)
            {
                var zone = new Zone(i);
                _zoneDic.Add(zone.ZoneId, zone); // Example zone with ID 1
            }

            // 1초마다 존 업데이트
            ProcessTimer.AddPeriodicTimer( 1000, (int Id) =>
            {
                Update();
                return true;
            });

        }

        public void Update()
        {
            // 각 로직쓰레드에서 존 업데이트
            foreach( var zone in _zoneDic.Values)
            {
                var worker = LogicThread.Instance.GetWorkerByKey(zone.ZoneId);

                worker.EnqueueLamda((int Id) =>
                {
                    zone.Update(Id);
                });
            }
        }
    }
}
