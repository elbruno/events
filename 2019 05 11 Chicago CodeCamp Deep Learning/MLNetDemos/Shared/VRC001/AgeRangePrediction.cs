using Microsoft.ML.Data;

namespace MLNetTestsRC010.Shared
{
    public class AgeRangePrediction
    {
        [ColumnName("PredictedLabel")]
        public string Label;

        public float[] Score;
    }
}