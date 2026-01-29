using System.Collections.Generic;

namespace ARSignTranslator.AI
{
    public class ModelState
    {
        public int K { get; set; }
        public List<float[]> Samples { get; set; } = new();
        public List<string> Labels { get; set; } = new();
        public float[] Mean { get; set; } = new float[0];
        public float[] Std { get; set; } = new float[0];
    }
}
