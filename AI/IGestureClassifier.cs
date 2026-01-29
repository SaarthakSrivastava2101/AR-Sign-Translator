using System.Collections.Generic;
using ARSignTranslator.Dataset;

namespace ARSignTranslator.AI
{
    public record GesturePrediction(string Label, float Confidence);

    public interface IGestureClassifier
    {
        void Fit(List<GestureSample> samples);
        GesturePrediction Predict(float[] features);
    }
}
