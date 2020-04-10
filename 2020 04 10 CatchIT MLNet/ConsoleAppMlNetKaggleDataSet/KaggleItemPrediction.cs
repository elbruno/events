using Microsoft.ML.Data;

namespace ConsoleAppMlNetKaggleDataSet
{
    public class KaggleItemPrediction
    {
        //vector to hold alert,score,p-value values
        [VectorType(3)]
        public double[] Prediction { get; set; }
    }
}
