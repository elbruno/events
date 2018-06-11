using System;
using Microsoft.ML;
using Microsoft.ML.Runtime.Api;

namespace MLNetMundoSql05Import
{
    class Program
    {
        private static PredictionModel<AgeRange, AgeRangePrediction> _model;

        static void Main(string[] args)
        {
            LoadModel();
            PredictLabel(_model, "Max", 5, "M");
            PredictLabel(_model, "Jennifer", 2, "F");
            PredictLabel(_model, "Borja", 10, "M");
            PredictLabel(_model, "Paula", 6, "F");
            PredictLabel(_model, "Nancy", 16, "F");
            Console.ReadLine();
        }

        private static async void LoadModel()
        {
            _model = await PredictionModel.ReadAsync<AgeRange, AgeRangePrediction>("AgeRangeModel.zip");
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
