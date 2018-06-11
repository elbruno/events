using System;
using System.Reflection.Emit;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Runtime.Api;
using Microsoft.ML.Trainers;
using Microsoft.ML.Transforms;

namespace MLNetMundoSql05
{
    class Program
    {
        static void Main(string[] args)
        {
            var pipeline = new LearningPipeline();
            pipeline.Add(new TextLoader("AgeRanges.csv").CreateFrom<AgeRange>(separator: ',', useHeader: true));
            pipeline.Add(new Dictionarizer("Label"));
            pipeline.Add(new TextFeaturizer("Gender", "Gender"));
            pipeline.Add(new ColumnConcatenator("Features", "Age", "Gender"));
            pipeline.Add(new StochasticDualCoordinateAscentClassifier());
            pipeline.Add(new PredictedLabelColumnOriginalValueConverter() { PredictedLabelColumn = "PredictedLabel" });
            var model = pipeline.Train<AgeRange, AgeRangePrediction>();

            model.WriteAsync("AgeRangeModel.zip");

            //PredictLabel(model, "Max", 5, "M");
            //PredictLabel(model, "Jennifer", 2, "F");
            //PredictLabel(model, "Borja", 10, "M");
            //PredictLabel(model, "Paula", 6, "F");
            //PredictLabel(model, "Nancy", 16, "F");
            Console.ReadLine();
        }

        private static void PredictLabel(PredictionModel<AgeRange, AgeRangePrediction> model, string name, float age, string gender)
        {
            var input = new AgeRange
            {
                Name = name,
                Age = age,
                Gender = gender
            };
            var prediction = model.Predict(input);
            Console.WriteLine(
                $"Predicted label for Name {input.Name}, Age {input.Age}, Gender {input.Gender}; is {prediction.PredictedLabel}");
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
