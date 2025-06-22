using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegionNG.World
{
    internal class Zone
    {
        public int ZoneId { get; private set; } = 0;
        public Zone (int zoneId)
        {
            ZoneId = zoneId;
        }

        public void Update(int Id)
        {
            Console.WriteLine($"Updating Zone {ZoneId}... Id {Id} ThreadId {Thread.CurrentThread.ManagedThreadId}");
            
        }
    }
}
