using System;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using Microsoft.ML.Transforms;
using MLNetAvaLL.Shared;

namespace MLNetDemos03ExportModel
{
    class Program
    {
        static void Main(string[] args)
        {
            var fileName = "AgeRanges.csv";
            var pipeline = new LearningPipeline();
            pipeline.Add(new TextLoader(fileName).CreateFrom<AgeRange>(separator: ',', useHeader: true));
            pipeline.Add(new Dictionarizer("Label"));
            pipeline.Add(new TextFeaturizer("Gender", "Gender"));
            pipeline.Add(new ColumnConcatenator("Features", "Age", "Gender"));
            pipeline.Add(new StochasticDualCoordinateAscentClassifier());
            pipeline.Add(new PredictedLabelColumnOriginalValueConverter { PredictedLabelColumn = "PredictedLabel" });
            var model = pipeline.Train<AgeRange, AgeRangePrediction>();
            model.WriteAsync("AgeRangeModel.zip");
            Console.ReadLine();
        }
   }
}
