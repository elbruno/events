using System;
using Microsoft.ML.Legacy;
using Microsoft.ML.Legacy.Data;
using Microsoft.ML.Legacy.Trainers;
using Microsoft.ML.Legacy.Transforms;
using Microsoft.ML.Runtime.Api;

namespace MLNetMetroTor02
{
    public static class Program
    {
        static void Main(string[] args)
        {
            string fileName = "AgeRangeData.csv";

            var pipeline = new LearningPipeline
            {
                new TextLoader(fileName).CreateFrom<AgeRange>(useHeader: true),
                new Dictionarizer("Label"),
                new ColumnConcatenator("Features", "Age"),
                new StochasticDualCoordinateAscentClassifier()
            };

            var model = pipeline.Train<AgeRange, AgeRangePrediction>();

            Predict(model, "Mike", 5, "M");

            Console.ReadLine();
        }

        private static void Predict(PredictionModel<AgeRange, AgeRangePrediction> model, string name, float age,
            string gender)
        {
            var input = new AgeRange
            {
                Age = age,
                Name = name,
                Gender = gender
            };
            var pred = model.Predict(input);
            Console.WriteLine($"Predicted label for Name {name}, Age {age}, Gender {gender}; is {pred.Label}");
        }

    }

    public class AgeRange
    {
        [Column(ordinal: "0")] public string Name;

        [Column(ordinal: "1")] public float Age;

        [Column(ordinal: "2")] public string Gender;

        [Column(ordinal: "3", name: "Label")] 
        public string Label;
    }

    public class AgeRangePrediction
    {
        [ColumnName("PredictedLabel")]
        public string Label;
    }
}
