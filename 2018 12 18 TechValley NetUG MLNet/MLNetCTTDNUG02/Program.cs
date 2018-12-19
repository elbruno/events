using System;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Runtime.Api;
using Microsoft.ML.Trainers;
using Microsoft.ML.Transforms;

namespace MLNetCTTDNUG02
{
    class Program
    {
        static void Main(string[] args)
        {

            var pipeline = new LearningPipeline();

            pipeline.Add(new TextLoader("AgeRangeData.csv").CreateFrom<AgeRange>(separator: ',', useHeader: true));
            pipeline.Add(new Dictionarizer("Label"));
            pipeline.Add(new TextFeaturizer("Gender", "Gender"));
            pipeline.Add(new ColumnConcatenator("Features", "Age", "Gender"));
            pipeline.Add(new StochasticDualCoordinateAscentClassifier());
            pipeline.Add(new PredictedLabelColumnOriginalValueConverter { PredictedLabelColumn = "PredictedLabel" });

            var model = pipeline.Train<AgeRange, AgeRangePrediction>();

            Predict(model, "Santi", 4, "M");
            Predict(model, "Paola", 46, "F");
            Predict(model, "Bruno", 42, "M");
            Predict(model, "Tino", 1, "M");
            Console.ReadLine();

        }

        private static void Predict(PredictionModel<AgeRange, AgeRangePrediction> model, string name, float age, string gender)
        {
            var input = new AgeRange
            {
                Age = age,
                Name = name,
                Gender = gender
            };
            var pred = model.Predict(input);
            Console.WriteLine($"Predicted label for Name {name}, Age {age}, Gender {gender}; is {pred.PredictedLabel}");
        }
    }

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

    public class AgeRangePrediction
    {
        [ColumnName("PredictedLabel")]
        public string PredictedLabel;
    }
}
