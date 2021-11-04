using Microsoft.ML.Data;

namespace SpikeDetection.DataStructures
{
    public class DronePrediction
    {
        //vector to hold alert,score,p-value values
        [VectorType(4)]
        public double[] Prediction { get; set; }
    }
}
