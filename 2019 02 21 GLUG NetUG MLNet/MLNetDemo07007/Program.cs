using System;
using System.IO;
using Common;
using Microsoft.ML;
using Microsoft.ML.Runtime.Data;
using MLNetDemo070.Shared;


namespace MLNetDemo07007
{
    class Program
    {
        static readonly string TrainDataPath = Path.Combine(Environment.CurrentDirectory, "Data", "AgeRangeData03.csv");
        static TextLoader _textLoader;

        static void Main(string[] args)
        {
            var mlContext = new MLContext(seed: 0);
            _textLoader = mlContext.Data.TextReader(new TextLoader.Arguments()
            {
                Separator = ",",
                HasHeader = true,
                Column = new[]
                    {
                        new TextLoader.Column("Name", DataKind.Text, 0),
                        new TextLoader.Column("Age", DataKind.Num, 1),
                        new TextLoader.Column("Gender", DataKind.Text, 2),
                        new TextLoader.Column("Label", DataKind.Text, 3),
                    }
            });

            // load train data
            ConsoleHelper.ConsoleWriterSection("=============== Loading train data ===============");
            var dvTrain = _textLoader.Read(TrainDataPath);

            // Train
            ConsoleHelper.ConsoleWriterSection("=============== Training the model ===============");
            var dataProcessPipeline = mlContext.Transforms.Categorical.MapValueToKey("Label", "LabelKeys")
                .Append(mlContext.Transforms.Text.FeaturizeText("Gender", "GenderFeaturized"))
                .Append(mlContext.Transforms.Concatenate("Features", "Age", "GenderFeaturized"));

            ConsoleHelper.PeekDataViewInConsole<AgeRange>(mlContext, dvTrain, dataProcessPipeline);
            ConsoleHelper.PeekVectorColumnDataInConsole(mlContext, "Features", dvTrain, dataProcessPipeline, 2);

            var trainer = mlContext.MulticlassClassification.Trainers.StochasticDualCoordinateAscent(label: "LabelKeys", features: "Features");

            var trainingPipeline = dataProcessPipeline.Append(trainer)
                .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

            Console.WriteLine("=============== Cross-validating to get model's accuracy metrics ===============");

            var crossValidationResults = mlContext.MulticlassClassification.CrossValidate(dvTrain, trainingPipeline, numFolds: 6, labelColumn: "LabelKeys");
            ConsoleHelper.PrintMulticlassClassificationFoldsAverageMetrics(trainer.ToString(), crossValidationResults);

            Console.WriteLine("=============== Training the model ===============");
            var model = trainingPipeline.Fit(dvTrain);
            Console.WriteLine("Model trained");

            Console.ReadLine();
        }

        private static void PredictSimple(string name, float age, string gender, PredictionFunction<AgeRange, AgeRangePrediction> predictionFunction)
        {
            var example = new AgeRange()
            {
                Age = age,
                Name = name,
                Gender = gender
            };
            var prediction = predictionFunction.Predict(example);
            ConsoleHelper.ConsoleWriteHeader($"Name: {example.Name}, Age: {example.Age}, Gender: {example.Gender}");
            Console.WriteLine($">> Predicted Label: {prediction.PredictedLabel}");
        }
    }
}
