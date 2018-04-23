using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MalmoCSharpLauncher.Shared
{
    class Observation
    {
            public int DistanceTravelled { get; set; }
            public int TimeAlive { get; set; }
            public int MobsKilled { get; set; }
            public int PlayersKilled { get; set; }
            public int DamageTaken { get; set; }
            public double Life { get; set; }
            public int Score { get; set; }
            public int Food { get; set; }
            public int XP { get; set; }
            public bool IsAlive { get; set; }
            public int Air { get; set; }
            public string Name { get; set; }
            public double XPos { get; set; }
            public double YPos { get; set; }
            public double ZPos { get; set; }
            public double Pitch { get; set; }
            public double Yaw { get; set; }
            public int WorldTime { get; set; }
            public int TotalTime { get; set; }
            public List<string> floor3x3 { get; set; }
            public List<NearbyEntity> NearbyEntities { get; set; }
            public LineOfSight LineOfSight { get; set; }
    }

    public class NearbyEntity
    {
        public double x { get; set; }
        public double y { get; set; }
        public double z { get; set; }
        public string name { get; set; }
    }

    public class LineOfSight
    {
        public string hitType { get; set; }
        public double x { get; set; }
        public double y { get; set; }
        public double z { get; set; }
        public string type { get; set; }
        public string variant { get; set; }
        public string colour { get; set; }
        public bool inRange { get; set; }
    }
}
