using System;
using System.Threading.Tasks;
using Microsoft.ML;
using MLNetAvaLL.Shared;

namespace MLNetDemos03ImportModel
{
    class Program
    {
        private static PredictionModel<AgeRange, AgeRangePrediction> _model;
        private static string _fileLocation = "AgeRangeModel.zip";

        static void Main(string[] args)
        {
            LoadModel();
            Predict(_model, "john", 9, "M");
            Predict(_model, "mary", 7, "F");
            Predict(_model, "abigail", 6, "F");
            Console.ReadLine();
        }

        private static async Task LoadModel()
        {
            _model = await PredictionModel.ReadAsync<AgeRange, AgeRangePrediction>(_fileLocation);
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
}
