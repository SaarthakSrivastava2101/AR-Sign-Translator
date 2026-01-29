using System;

namespace ARSignTranslator.Dataset
{
    public class GestureSample
    {
        public string Label { get; }
        public float[] Features { get; }

        public GestureSample(string label, float[] features)
        {
            Label = label ?? throw new ArgumentNullException(nameof(label));
            Features = features ?? throw new ArgumentNullException(nameof(features));
        }
    }
}
