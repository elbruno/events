using System;
using System.IO;
using Microsoft.ML;
using Microsoft.ML.Data;

namespace MLNetGlobalAINight01
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create MLContext to be shared across the model creation workflow objects 
            // Set a random seed for repeatable/deterministic results across multiple trainings.
            var mlContext = new MLContext(seed: 1);

            // STEP 1: Common data loading configuration
            IDataView dataView = mlContext.Data.LoadFromTextFile<SentimentIssue>("wikiDetoxAnnotated40kRows.tsv", hasHeader: true);

            DataOperationsCatalog.TrainTestData trainTestSplit = mlContext.Data.TrainTestSplit(dataView, testFraction: 0.2);
            IDataView trainingData = trainTestSplit.TrainSet;
            IDataView testData = trainTestSplit.TestSet;

            // STEP 2: Common data process configuration with pipeline data transformations          
            var dataProcessPipeline = mlContext.Transforms.Text.FeaturizeText(outputColumnName: "Features", inputColumnName: nameof(SentimentIssue.Text));

            // STEP 3: Set the training algorithm, then create and config the modelBuilder                            
            var trainer = mlContext.BinaryClassification.Trainers.SdcaLogisticRegression(labelColumnName: "Label", featureColumnName: "Features");
            var trainingPipeline = dataProcessPipeline.Append(trainer);

            // STEP 4: Train the model fitting to the DataSet
            ITransformer trainedModel = trainingPipeline.Fit(trainingData);

            // STEP 5: Evaluate the model and show accuracy stats
            var predictions = trainedModel.Transform(testData);
            var metrics = mlContext.BinaryClassification.Evaluate(data: predictions, labelColumnName: "Label", scoreColumnName: "Score");

            //ConsoleHelper.PrintBinaryClassificationMetrics(trainer.ToString(), metrics);

            // STEP 6: Save/persist the trained model to a .ZIP file
            string modelPath = @"model.zip";
            mlContext.Model.Save(trainedModel, trainingData.Schema, modelPath);
            Console.WriteLine("The model is saved to {0}", modelPath);

            // Create prediction engine related to the loaded trained model
            var predEngine = mlContext.Model.CreatePredictionEngine<SentimentIssue, SentimentPrediction>(trainedModel);

            // Score
            Console.WriteLine($"=============== Single Prediction  ===============");
            Predict("I love this movie!", predEngine);
            Predict("This movie is stupid", predEngine);
            Predict("The vacuum that I got from BestBuy is amazing!", predEngine);
            Predict("This vacuum sucks so much dirt", predEngine);
            Console.WriteLine($"================End of Process.Hit any key to exit==================================");
            Console.ReadLine();
        }

        public static void Predict(string text, PredictionEngine<SentimentIssue, SentimentPrediction> predEngine)
        {
            // prediction
            var testSentiment = new SentimentIssue { Text = text };
            var prediction = predEngine.Predict(testSentiment);

            // Console Write
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"Text: {testSentiment.Text}");
            Console.ForegroundColor = prediction.Prediction ? ConsoleColor.Red : ConsoleColor.Green;
            Console.WriteLine($"    Prediction: {(Convert.ToBoolean(prediction.Prediction) ? "Toxic" : "Good")} | Probability of being toxic: {prediction.Probability} ");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
    public class SentimentIssue
    {
        [LoadColumn(0)]
        public bool Label { get; set; }
        [LoadColumn(2)]
        public string Text { get; set; }
    }
    public class SentimentPrediction
    {
        [ColumnName("PredictedLabel")]
        public bool Prediction { get; set; }
        public float Probability { get; set; }
        public float Score { get; set; }
    }
}
