using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegionNG
{
    public class LogicThread : MessageWorkerPack
    {
        public LogicThread() : base( 4 ){}

        public static LogicThread Instance = new LogicThread();

    }

    public class DBThread  : MessageWorkerPack
    {
        public static DBThread Instance = new DBThread();
        public DBThread() : base(4) { }
    }

    public class PacketThread : MessageWorkerPack
    {
        public PacketThread() : base(4) { }
    }

}
