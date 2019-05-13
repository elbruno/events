// <copyright file="ObjectDetection.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

/// Script for CustomVision's exported object detection Model.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.AI.MachineLearning;
using Windows.Media;
using Windows.Storage;

namespace CustomVisionMarvel01
{
    public sealed class BoundingBox
    {
        public BoundingBox(float left, float top, float width, float height)
        {
            Left = left;
            Top = top;
            Width = width;
            Height = height;
        }

        public float Left { get; private set; }
        public float Top { get; private set; }
        public float Width { get; private set; }
        public float Height { get; private set; }
    }

    public sealed class PredictionModel
    {
        public PredictionModel(float probability, string tagName, BoundingBox boundingBox)
        {
            Probability = probability;
            TagName = tagName;
            BoundingBox = boundingBox;
        }

        public float Probability { get; private set; }
        public string TagName { get; private set; }
        public BoundingBox BoundingBox { get; private set; }
    }

    public class ObjectDetection
    {
        private static readonly float[] Anchors = new float[] { 0.573f, 0.677f, 1.87f, 2.06f, 3.34f, 5.47f, 7.88f, 3.53f, 9.77f, 9.17f };

        public readonly IList<string> Labels;
        public readonly int MaxDetections;
        public readonly float ProbabilityThreshold;
        public readonly float IouThreshold;
        public LearningModel Model;
        public LearningModelSession Session;

        public ObjectDetection(int maxDetections = 20, float probabilityThreshold = 0.1f, float iouThreshold = 0.45f)
        {
            Labels = new List<string>()
            {
                "Black_Widow",
                "Captain_America_Body",
                "Captain_America_Shield",
                "Iron_Man_Face",
                "Rocket_Face",
                "Rocket_Gun",
                "Rocket_Racoon"
            };
            MaxDetections = maxDetections;
            ProbabilityThreshold = probabilityThreshold;
            IouThreshold = iouThreshold;
        }

        public async Task Init(StorageFile file)
        {
            Model = await LearningModel.LoadFromStorageFileAsync(file);
            Session = new LearningModelSession(Model);

            Debug.Assert(Model.InputFeatures.Count == 1, "The number of input must be 1");
            Debug.Assert(Model.OutputFeatures.Count == 1, "The number of output must be 1");
        }

        /// <summary>
        /// Detect objects from the given image.
        /// The input image must be 416x416.
        /// </summary>
        public async Task<IList<PredictionModel>> PredictImageAsync(VideoFrame image)
        {
            var output = new modelOutput();
            var imageFeature = ImageFeatureValue.CreateFromVideoFrame(image);
            var bindings = new LearningModelBinding(Session);
            bindings.Bind("data", imageFeature);
            bindings.Bind("model_outputs0", output.Model_outputs0);
            var result = await Session.EvaluateAsync(bindings, "0");

            return Postprocess(output.Model_outputs0);
        }

        private IList<PredictionModel> Postprocess(TensorFloat predictionOutputs)
        {
            var (boxes, probs) = ExtractBoxes(predictionOutputs, Anchors);
            return SuppressNonMaximum(boxes, probs);
        }

        /// <summary>
        /// Extract bounding boxes and their probabilities from the prediction output.
        /// </summary>
        private (IList<BoundingBox>, IList<float[]>) ExtractBoxes(TensorFloat predictionOutput, float[] anchors)
        {
            var shape = predictionOutput.Shape;
            Debug.Assert(shape.Count == 4, "The Model output has unexpected shape");
            Debug.Assert(shape[0] == 1, "The batch size must be 1");

            var outputs = predictionOutput.GetAsVectorView();

            var numAnchor = anchors.Length / 2;
            var channels = shape[1];
            var height = shape[2];
            var width = shape[3];

            Debug.Assert(channels % numAnchor == 0);
            var numClass = (channels / numAnchor) - 5;

            Debug.Assert(numClass == Labels.Count);

            var boxes = new List<BoundingBox>();
            var probs = new List<float[]>();
            for (var gridY = 0; gridY < height; gridY++)
            {
                for (var gridX = 0; gridX < width; gridX++)
                {
                    var offset = 0;
                    var stride = (int)(height * width);
                    var baseOffset = gridX + gridY * (int)width;

                    for (var i = 0; i < numAnchor; i++)
                    {
                        var x = (Logistic(outputs[baseOffset + (offset++ * stride)]) + gridX) / width;
                        var y = (Logistic(outputs[baseOffset + (offset++ * stride)]) + gridY) / height;
                        var w = (float)Math.Exp(outputs[baseOffset + (offset++ * stride)]) * anchors[i * 2] / width;
                        var h = (float)Math.Exp(outputs[baseOffset + (offset++ * stride)]) * anchors[i * 2 + 1] / height;

                        x = x - (w / 2);
                        y = y - (h / 2);

                        var objectness = Logistic(outputs[baseOffset + (offset++ * stride)]);

                        var classProbabilities = new float[numClass];
                        for (var j = 0; j < numClass; j++)
                        {
                            classProbabilities[j] = outputs[baseOffset + (offset++ * stride)];
                        }
                        var max = classProbabilities.Max();
                        for (var j = 0; j < numClass; j++)
                        {
                            classProbabilities[j] = (float)Math.Exp(classProbabilities[j] - max);
                        }
                        var sum = classProbabilities.Sum();
                        for (var j=0; j<numClass;j++)
                        {
                            classProbabilities[j] *= objectness / sum;
                        }

                        if (classProbabilities.Max() > ProbabilityThreshold)
                        {
                            boxes.Add(new BoundingBox(x, y, w, h));
                            probs.Add(classProbabilities);
                        }
                    }
                    Debug.Assert(offset == channels);
                }
            }

            Debug.Assert(boxes.Count == probs.Count);
            return (boxes, probs);
        }

        /// <summary>
        /// Remove overlapping predictions and return top-n predictions.
        /// </summary>
        private IList<PredictionModel> SuppressNonMaximum(IList<BoundingBox> boxes, IList<float[]> probs)
        {
            var predictions = new List<PredictionModel>();
            var maxProbs = probs.Select(x => x.Max()).ToArray();

            while (predictions.Count < MaxDetections)
            {
                var max = maxProbs.Max();
                if (max < ProbabilityThreshold)
                {
                    break;
                }
                var index = Array.IndexOf(maxProbs, max);
                var maxClass = Array.IndexOf(probs[index], max);

                predictions.Add(new PredictionModel(max, Labels[maxClass], boxes[index]));

                for (var i = 0; i < boxes.Count; i++)
                {
                    if (CalculateIOU(boxes[index], boxes[i]) > IouThreshold)
                    {
                        probs[i][maxClass] = 0;
                        maxProbs[i] = probs[i].Max();
                    }
                }
            }

            return predictions;
        }

        private static float Logistic(float x)
        {
            if (x > 0)
            {
                return (float)(1 / (1 + Math.Exp(-x)));
            }
            else
            {
                var e = Math.Exp(x);
                return (float)(e / (1 + e));
            }
        }

        /// <summary>
        /// Calculate Intersection over Union (IOU) for the given 2 bounding boxes.
        /// </summary>
        private static float CalculateIOU(BoundingBox box0, BoundingBox box1)
        {
            var x1 = Math.Max(box0.Left, box1.Left);
            var y1 = Math.Max(box0.Top, box1.Top);
            var x2 = Math.Min(box0.Left + box0.Width, box1.Left + box1.Width);
            var y2 = Math.Min(box0.Top + box0.Height, box1.Top + box1.Height);
            var w = Math.Max(0, x2 - x1);
            var h = Math.Max(0, y2 - y1);

            return w * h / ((box0.Width * box0.Height) + (box1.Width * box1.Height) - (w * h));
        }

    }
}
