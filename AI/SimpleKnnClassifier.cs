using System;
using System.Collections.Generic;
using System.Linq;
using ARSignTranslator.Dataset;
using ARSignTranslator.Utils;

namespace ARSignTranslator.AI
{
    public class SimpleKnnClassifier : IGestureClassifier
    {
        private readonly int _k;

        private readonly List<float[]> _x = new();
        private readonly List<string> _y = new();

        private float[] _mean = Array.Empty<float>();
        private float[] _std = Array.Empty<float>();

        public SimpleKnnClassifier(int k = 5)
        {
            _k = Math.Max(1, k);
        }

        public void Fit(List<GestureSample> samples)
        {
            _x.Clear();
            _y.Clear();

            if (samples == null || samples.Count == 0)
                return;

            int d = samples[0].Features.Length;

            _mean = new float[d];
            _std = new float[d];

            for (int j = 0; j < d; j++)
            {
                double sum = 0;
                for (int i = 0; i < samples.Count; i++)
                    sum += samples[i].Features[j];

                _mean[j] = (float)(sum / samples.Count);
            }

            for (int j = 0; j < d; j++)
            {
                double var = 0;
                for (int i = 0; i < samples.Count; i++)
                {
                    double diff = samples[i].Features[j] - _mean[j];
                    var += diff * diff;
                }

                double std = Math.Sqrt(var / samples.Count);
                _std[j] = (float)(std < 1e-6 ? 1.0 : std);
            }

            foreach (var s in samples)
            {
                var z = Normalize(s.Features);
                _x.Add(z);
                _y.Add(s.Label);
            }
        }

        public GesturePrediction Predict(float[] features)
        {
            if (_x.Count == 0)
                return new GesturePrediction("UNKNOWN", 0f);

            var z = Normalize(features);

            var neighbors = new List<(double dist, string label)>(_x.Count);

            for (int i = 0; i < _x.Count; i++)
            {
                double d = 0;
                var xi = _x[i];

                for (int j = 0; j < z.Length; j++)
                {
                    double diff = z[j] - xi[j];
                    d += diff * diff;
                }

                neighbors.Add((Math.Sqrt(d), _y[i]));
            }

            neighbors.Sort((a, b) => a.dist.CompareTo(b.dist));

            int kk = Math.Min(_k, neighbors.Count);

            var votes = new Dictionary<string, double>();
            double total = 0;

            for (int i = 0; i < kk; i++)
            {
                var (dist, label) = neighbors[i];
                double w = 1.0 / (dist + 1e-6);

                if (!votes.ContainsKey(label))
                    votes[label] = 0;

                votes[label] += w;
                total += w;
            }

            var best = votes.OrderByDescending(x => x.Value).First();
            float conf = total <= 0 ? 0f : (float)(best.Value / total);

            return new GesturePrediction(best.Key, conf);
        }

        private float[] Normalize(float[] v)
        {
            var z = new float[v.Length];
            for (int i = 0; i < v.Length; i++)
                z[i] = (v[i] - _mean[i]) / _std[i];
            return z;
        }

        public ModelState ExportState()
        {
            return new ModelState
            {
                K = _k,
                Samples = _x.Select(a => a.ToArray()).ToList(),
                Labels = _y.ToList(),
                Mean = _mean.ToArray(),
                Std = _std.ToArray()
            };
        }

        public static SimpleKnnClassifier ImportState(ModelState state)
        {
            var clf = new SimpleKnnClassifier(state.K);
            clf._x.Clear();
            clf._y.Clear();

            clf._mean = state.Mean ?? Array.Empty<float>();
            clf._std = state.Std ?? Array.Empty<float>();

            if (state.Samples != null)
            {
                foreach (var s in state.Samples)
                    clf._x.Add(s.ToArray());
            }

            if (state.Labels != null)
                clf._y.AddRange(state.Labels);

            return clf;
        }
    }
}
