// Suggested steps before inference: 
// 1. for an image of width and height being (w, h) pixels, resize image to (w', h'), where w/h = w'/h' and w' x h' = 262144
// 2. resize network input size to (w', h')
// 3. pass the image to network and do inference
// (4. if inference speed is too slow for you, try to make w' x h' smaller)
// <copyright file="ObjectDetection.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

/// Script for CustomVision's exported object detection model.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.AI.MachineLearning;
using Windows.Media;
using Windows.Storage;

namespace CustomVisionParkingLot01
{
    public sealed class BoundingBox
    {
        public BoundingBox(float left, float top, float width, float height)
        {
            this.Left = left;
            this.Top = top;
            this.Width = width;
            this.Height = height;
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
            this.Probability = probability;
            this.TagName = tagName;
            this.BoundingBox = boundingBox;
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
        public float ProbabilityThreshold;
        public readonly float IouThreshold;
        public LearningModel Model;
        public LearningModelSession Session;

        public ObjectDetection(int maxDetections = 20, float probabilityThreshold = 0.1f, float iouThreshold = 0.45f)
        {
            Labels = new List<string>()
            {
                "spot 1",
                "spot 2",
                "spot 3"
            };
            this.MaxDetections = maxDetections;
            this.ProbabilityThreshold = probabilityThreshold;
            this.IouThreshold = iouThreshold;
        }

        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="file">The ONNX file</param>
        public async Task Init(StorageFile file)
        {
            this.Model = await LearningModel.LoadFromStorageFileAsync(file);
            this.Session = new LearningModelSession(this.Model);

            Debug.Assert(this.Model.InputFeatures.Count == 1, "The number of input must be 1");
            Debug.Assert(this.Model.OutputFeatures.Count == 1, "The number of output must be 1");
        }

        /// <summary>
        /// Detect objects from the given image.
        /// The input image must be 416x416.
        /// </summary>
        public async Task<IList<PredictionModel>> PredictImageAsync(VideoFrame image)
        {
            var imageFeature = ImageFeatureValue.CreateFromVideoFrame(image);
            var bindings = new LearningModelBinding(this.Session);
            bindings.Bind("data", imageFeature);
            var result = await this.Session.EvaluateAsync(bindings, "");
            return Postprocess(result.Outputs["model_outputs0"] as TensorFloat);
        }

        private class ExtractedBoxes
        {
            public List<BoundingBox> Boxes { get; }
            public List<float[]> Probabilities { get; }
            public ExtractedBoxes(List<BoundingBox> boxes, List<float[]> probs)
            {
                this.Boxes = boxes;
                this.Probabilities = probs;
            }
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
        private static float CalculateIou(BoundingBox box0, BoundingBox box1)
        {
            var x1 = Math.Max(box0.Left, box1.Left);
            var y1 = Math.Max(box0.Top, box1.Top);
            var x2 = Math.Min(box0.Left + box0.Width, box1.Left + box1.Width);
            var y2 = Math.Min(box0.Top + box0.Height, box1.Top + box1.Height);
            var w = Math.Max(0, x2 - x1);
            var h = Math.Max(0, y2 - y1);

            return w * h / ((box0.Width * box0.Height) + (box1.Width * box1.Height) - (w * h));
        }

        /// <summary>
        /// Extract bounding boxes and their probabilities from the prediction output.
        /// </summary>
        private ExtractedBoxes ExtractBoxes(TensorFloat predictionOutput, float[] anchors)
        {
            var shape = predictionOutput.Shape;
            Debug.Assert(shape.Count == 4, "The model output has unexpected shape");
            Debug.Assert(shape[0] == 1, "The batch size must be 1");

            IReadOnlyList<float> outputs = predictionOutput.GetAsVectorView();

            var numAnchor = anchors.Length / 2;
            var channels = shape[1];
            var height = shape[2];
            var width = shape[3];

            Debug.Assert(channels % numAnchor == 0);
            var numClass = (channels / numAnchor) - 5;

            Debug.Assert(numClass == Labels.Count);

            var boxes = new List<BoundingBox>();
            var probs = new List<float[]>();
            for (int gridY = 0; gridY < height; gridY++)
            {
                for (int gridX = 0; gridX < width; gridX++)
                {
                    int offset = 0;
                    int stride = (int)(height * width);
                    int baseOffset = gridX + gridY * (int)width;

                    for (int i = 0; i < numAnchor; i++)
                    {
                        var x = (Logistic(outputs[baseOffset + (offset++ * stride)]) + gridX) / width;
                        var y = (Logistic(outputs[baseOffset + (offset++ * stride)]) + gridY) / height;
                        var w = (float)Math.Exp(outputs[baseOffset + (offset++ * stride)]) * anchors[i * 2] / width;
                        var h = (float)Math.Exp(outputs[baseOffset + (offset++ * stride)]) * anchors[i * 2 + 1] / height;

                        x = x - (w / 2);
                        y = y - (h / 2);

                        var objectness = Logistic(outputs[baseOffset + (offset++ * stride)]);

                        var classProbabilities = new float[numClass];
                        for (int j = 0; j < numClass; j++)
                        {
                            classProbabilities[j] = outputs[baseOffset + (offset++ * stride)];
                        }
                        var max = classProbabilities.Max();
                        for (int j = 0; j < numClass; j++)
                        {
                            classProbabilities[j] = (float)Math.Exp(classProbabilities[j] - max);
                        }
                        var sum = classProbabilities.Sum();
                        for (int j=0; j<numClass;j++)
                        {
                            classProbabilities[j] *= objectness / sum;
                        }

                        if (classProbabilities.Max() > this.ProbabilityThreshold)
                        {
                            boxes.Add(new BoundingBox(x, y, w, h));
                            probs.Add(classProbabilities);
                        }
                    }
                    Debug.Assert(offset == channels);
                }
            }

            Debug.Assert(boxes.Count == probs.Count);
            return new ExtractedBoxes(boxes, probs);
        }

        /// <summary>
        /// Remove overlapping predictions and return top-n predictions.
        /// </summary>
        private List<PredictionModel> SuppressNonMaximum(ExtractedBoxes extractedBoxes)
        {
            var predictions = new List<PredictionModel>();

            if (extractedBoxes.Probabilities.Count > 0)
            {
                var maxProbs = extractedBoxes.Probabilities.Select(x => x.Max()).ToArray();

                while (predictions.Count < this.MaxDetections)
                {
                    var max = maxProbs.Max();
                    if (max < this.ProbabilityThreshold)
                    {
                        break;
                    }
                    var index = Array.IndexOf(maxProbs, max);
                    var maxClass = Array.IndexOf(extractedBoxes.Probabilities[index], max);

                    predictions.Add(new PredictionModel(max, Labels[maxClass], extractedBoxes.Boxes[index]));

                    for (int i = 0; i < extractedBoxes.Boxes.Count; i++)
                    {
                        if (CalculateIou(extractedBoxes.Boxes[index], extractedBoxes.Boxes[i]) > this.IouThreshold)
                        {
                            extractedBoxes.Probabilities[i][maxClass] = 0;
                            maxProbs[i] = extractedBoxes.Probabilities[i].Max();
                        }
                    }
                }
            }

            return predictions;
        }

        private List<PredictionModel> Postprocess(TensorFloat predictionOutputs)
        {
            var extractedBoxes = this.ExtractBoxes(predictionOutputs, ObjectDetection.Anchors);
            return this.SuppressNonMaximum(extractedBoxes);
        }
    }
}
