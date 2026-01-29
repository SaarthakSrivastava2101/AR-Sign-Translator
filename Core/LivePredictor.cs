using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ARSignTranslator.AI;
using ARSignTranslator.Dataset;

namespace ARSignTranslator.Core
{
    public static class LivePredictor
    {
        private static readonly Queue<GesturePrediction> Window = new();
        private const int WindowSize = 5;

        public static GesturePrediction PredictOnce(string modelPath, string liveCsvPath)
        {
            var state = ModelSerializer.Load(modelPath);
            var classifier = SimpleKnnClassifier.ImportState(state);

            var sample = DatasetLoader.LoadSingleSample(liveCsvPath);
            var pred = classifier.Predict(sample.Features);

            Window.Enqueue(pred);
            while (Window.Count > WindowSize)
                Window.Dequeue();

            var grouped = Window
                .GroupBy(p => p.Label)
                .Select(g => new
                {
                    Label = g.Key,
                    Votes = g.Count(),
                    AvgConf = g.Average(x => x.Confidence)
                })
                .OrderByDescending(x => x.Votes)
                .ThenByDescending(x => x.AvgConf)
                .First();

            float stableConf = (float)(grouped.Votes / (double)Window.Count);

            return new GesturePrediction(grouped.Label, stableConf);
        }
    }
}
