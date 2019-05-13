using Microsoft.ML.Data;

namespace MLNetTests100.Shared
{
    public class AgeRangePrediction
    {
        [ColumnName("PredictedLabel")]
        public string Label;

        public float[] Score;
    }
}