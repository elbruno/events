using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

using Windows.AI.MachineLearning;
using Windows.Media;
using Windows.Storage;

namespace CodeMashMarvelUwp01
{
    public sealed class modelOutput
    {
        public TensorFloat Model_outputs0 = TensorFloat.Create(new long[] { 1, 4 });
    }

    public sealed class BoundingBox
    {
        public BoundingBox(float left, float top, float width, float height)
        {
            Left = left;
            Top = top;
            Width = width;
            Height = height;
        }

        public float Left { get; set; }
        public float Top { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }

        public RectangleF Rect
        {
            get { return new RectangleF(Left, Top, Width, Height); }
        }

        public override string ToString()
        {
            return $"Left: {Left}, Top: {Top}, Height: {Height}, Width: {Width}";
        }
    }

    public sealed class PredictionModel
    {
        public PredictionModel(float probability, string tagName, BoundingBox box)
        {
            Probability = probability;
            TagName = tagName;
            Box = box;
        }

        public float Probability { get; private set; }
        public string TagName { get; private set; }
        public BoundingBox Box { get; private set; }

        public override string ToString()
        {
            return $"{TagName} {Probability * 100.0f:#0.00}%";
        }
        public string GetExtendedDescription()
        {
            return $"{TagName} {Probability * 100.0f:#0.00}%{Environment.NewLine}\tOriginal\tTop: {Box.Top}, Left: {Box.Left}, Height: {Box.Height}, Width: {Box.Width}";
        }
    }

    public class ObjectDetection
    {
        private static readonly float[] Anchors = new float[] { 0.573f, 0.677f, 1.87f, 2.06f, 3.34f, 5.47f, 7.88f, 3.53f, 9.77f, 9.17f };

        public readonly IList<string> Labels;
        public int MaxDetections;
        public float ProbabilityThreshold;
        public readonly float IouThreshold;
        public LearningModel Model;
        public LearningModelSession Session;

        public ObjectDetection(int maxDetections = 20, float probabilityThreshold = 0.1f, float iouThreshold = 0.45f)
        {
            Labels = new List<string>() { "Iron_Fist", "Rocket_Racoon", "Venom" };
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
            var result = SuppressNonMaximum(boxes, probs);

            // filter to have the max % per detected label
            var topLabels = new List<PredictionModel>();
            var topValues = new Dictionary<string, float>();
            result = result.OrderByDescending(a => a.TagName).ThenByDescending(a => a.Probability).ToList();
            foreach (var prediction in result)
            {
                float value = 0;
                if (topValues.ContainsKey(prediction.TagName))
                { value = topValues[prediction.TagName]; }
                else
                { topValues.Add(prediction.TagName, prediction.Probability); }
                if (!(value < prediction.Probability)) continue;
                topLabels.Add(prediction);
                topValues[prediction.TagName] = prediction.Probability;
            }
            return topLabels;
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
                        for (var j = 0; j < numClass; j++)
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
            if (boxes == null || probs == null || boxes.Count == 0 || probs.Count == 0)
                return predictions;
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

                var predictionModel = new PredictionModel(max, Labels[maxClass], boxes[index]);
                predictionModel.Box.Top = predictionModel.Box.Top * 100f;
                predictionModel.Box.Left = predictionModel.Box.Left * 100f;
                predictionModel.Box.Width = predictionModel.Box.Width * 100f;
                predictionModel.Box.Height = predictionModel.Box.Height * 100f;

                predictions.Add(predictionModel);

                for (var i = 0; i < boxes.Count; i++)
                {
                    var intersectionOverUnion = CalculateIOU(boxes[index], boxes[i]);
                    if (intersectionOverUnion > IouThreshold)
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
