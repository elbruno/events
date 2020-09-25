using Microsoft.ML.Data;

namespace SpikeDetection.DataStructures
{
    public class Ec2Data
    {
        [LoadColumn(0)]
        public string timestamp;

        [LoadColumn(1)]
        public float value;
    }
}
