using System;
using Microsoft.ML.Legacy;
using Microsoft.ML.Legacy.Data;
using Microsoft.ML.Legacy.Trainers;
using Microsoft.ML.Legacy.Transforms;
using Microsoft.ML.Runtime.Api;

namespace MLNetMetroTor01
{
    class Program
    {
        static void Main(string[] args)
        {
            string fileName = "AgeRangeData.csv";
            var pipeline = new LearningPipeline();
            pipeline.Add(new TextLoader(fileName).CreateFrom<AgeRange>(separator: ',', useHeader: true));
            pipeline.Add(new Dictionarizer("Label"));
            pipeline.Add(new ColumnConcatenator("Features", "Age"));
            pipeline.Add(new StochasticDualCoordinateAscentClassifier());
            pipeline.Add(new PredictedLabelColumnOriginalValueConverter { PredictedLabelColumn = "PredictedLabel" });

            var model = pipeline.Train<AgeRange, AgeRangePrediction>();

            Predict(model, "Mike", 5, "M");

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
