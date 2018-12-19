using System;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Runtime.Api;
using Microsoft.ML.Trainers;
using Microsoft.ML.Transforms;

namespace MLNetMetroTor03
{
    class Program
    {
        static void Main(string[] args)
        {
            var pipeline = new LearningPipeline();

            pipeline.Add(new TextLoader("AgeRAngeData.csv").CreateFrom<AgeRange>(separator: ',', useHeader: true));
            pipeline.Add(new Dictionarizer("Label"));
            pipeline.Add(new TextFeaturizer("Gender", "Gender"));
            pipeline.Add(new ColumnConcatenator("Features", "Age", "Gender"));
            pipeline.Add(new StochasticDualCoordinateAscentClassifier());
            pipeline.Add(new PredictedLabelColumnOriginalValueConverter { PredictedLabelColumn = "PredictedLabel" });

            var model = pipeline.Train<AgeRange, AgeRangePrediction>();

            Predict(model, "Mike", 5, "M");
            Predict(model, "Jane", 1, "F");
            Predict(model, "Bruno", 42, "M");
            Predict(model, "Paola", 32, "F");

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
