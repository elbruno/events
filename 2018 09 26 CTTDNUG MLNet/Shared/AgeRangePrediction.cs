using Microsoft.ML.Runtime.Api;

namespace MLNetAvaLL.Shared
{
    public class AgeRangePrediction
    {
        [ColumnName("PredictedLabel")]
        public string PredictedLabel;
    }
}