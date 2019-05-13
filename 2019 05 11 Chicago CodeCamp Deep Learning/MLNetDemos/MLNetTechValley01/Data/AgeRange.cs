using Microsoft.ML.Runtime.Api;

namespace MLNetDemo080.Shared
{
    public class AgeRange
    {
        [Column(ordinal: "0")]
        public string Name;

        [Column(ordinal: "1")]
        public float Age;

        [Column(ordinal: "2")]
        public string Gender;

        [Column(ordinal: "3")]
        public string Label;
    }
}