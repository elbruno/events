using Microsoft.ML.Runtime.Api;

namespace MLDemos.Shared
{
    public class SentimentPrediction
    {
        [ColumnName("PredictedLabel")]
        public bool Sentiment;
    }
}