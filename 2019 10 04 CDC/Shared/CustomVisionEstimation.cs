using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

// created with https://app.quicktype.io/#l=cs&r=json2csharp
namespace DistractedDriverDetectionShared
{
    public partial class CustomVisionEstimation
    {
        [JsonProperty("created")]
        public DateTimeOffset Created { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("iteration")]
        public string Iteration { get; set; }

        [JsonProperty("predictions")]
        public Prediction[] Predictions { get; set; }

        [JsonProperty("project")]
        public string Project { get; set; }
    }

    public partial class Prediction
    {
        [JsonProperty("boundingBox")]
        public object BoundingBox { get; set; }

        [JsonProperty("probability")]
        public double Probability { get; set; }

        [JsonProperty("tagId")]
        public string TagId { get; set; }

        [JsonProperty("tagName")]
        public string TagName { get; set; }
    }

    public partial class CustomVisionEstimation
    {
        public static CustomVisionEstimation FromJson(string json) => JsonConvert.DeserializeObject<CustomVisionEstimation>(json, Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this CustomVisionEstimation self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
}
