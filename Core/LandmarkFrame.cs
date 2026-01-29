using System;

namespace ARSignTranslator.Core
{
    public class LandmarkFrame
    {
        public float[] Values { get; }

        public LandmarkFrame(float[] values)
        {
            Values = values ?? throw new ArgumentNullException(nameof(values));
        }
    }
}
