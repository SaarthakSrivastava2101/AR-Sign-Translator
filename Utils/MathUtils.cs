using System;

namespace ARSignTranslator.Utils
{
    public static class MathUtils
    {
        public static double EuclideanDistance(float[] a, float[] b)
        {
            if (a.Length != b.Length)
                throw new ArgumentException("Vectors must have same length.");

            double sum = 0;

            for (int i = 0; i < a.Length; i++)
            {
                double d = a[i] - b[i];
                sum += d * d;
            }

            return Math.Sqrt(sum);
        }
    }
}
