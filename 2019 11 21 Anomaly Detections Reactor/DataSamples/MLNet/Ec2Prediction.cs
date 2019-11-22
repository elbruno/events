using Microsoft.ML.Data;

namespace SpikeDetection.DataStructures
{
    public class Ec2Prediction
    {
        //vector to hold alert,score,p-value values
        [VectorType(3)]
        public double[] Prediction { get; set; }
    }
}
