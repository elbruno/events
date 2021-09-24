using Microsoft.ML.Data;

namespace SpikeDetection.DataStructures
{
    public class DroneData
    {
        [LoadColumn(0)]
        public string DateTimeRecord;

        [LoadColumn(1)]
        public float agx;

        [LoadColumn(2)]
        public float agy;

        [LoadColumn(2)]
        public float agz;
    }
}
