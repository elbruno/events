using System;
using System.IO;
using Microsoft.ML;
using Microsoft.ML.Runtime.Data;
using MLNetDemo070.Shared;

namespace MLNetDemo07005
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

            // split data 80/20
            var dvFull = _textLoader.Read(TrainDataPath);
            var (dvTrain, dvTest) = mlContext.MulticlassClassification.TrainTestSplit(dvFull, testFraction: 0.2);

            // Train
            var pipeline = mlContext.Transforms.Concatenate("Features", "Age")
                .Append(mlContext.Transforms.Categorical.MapValueToKey("Label"), TransformerScope.TrainTest)
                .Append(mlContext.MulticlassClassification.Trainers.StochasticDualCoordinateAscent())
                .Append(mlContext.Transforms.Conversion.MapKeyToValue(("PredictedLabel", "Label")));

            Console.WriteLine("Peek TRAIN dataset");
            Common.ConsoleHelper.PeekDataViewInConsole<AgeRange>(mlContext, dvTrain, pipeline, 2);
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Peek TEST dataset");
            Common.ConsoleHelper.PeekDataViewInConsole<AgeRange>(mlContext, dvTest, pipeline, 1);

            var model = pipeline.Fit(dvTrain);
            Console.WriteLine("Model trained");

            Console.ReadLine();
        }
    }
}
