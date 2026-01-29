using System;
using System.Globalization;
using System.IO;

namespace ARSignTranslator
{
    public static class DataGenerator
    {
        public static void Generate(string folderPath, int frames, int landmarks, int valuesPerLandmark)
        {
            Directory.CreateDirectory(folderPath);

            int featureCount = frames * landmarks * valuesPerLandmark;

            string trainPath = Path.Combine(folderPath, "train.csv");
            string testPath = Path.Combine(folderPath, "test.csv");

            var labels = new (string Label, float Base)[]
            {
                ("HELLO", 0.20f),
                ("THANKYOU", 0.50f),
                ("YES", 0.80f)
            };

            string header = "label";
            for (int i = 1; i <= featureCount; i++)
                header += $",f{i}";

            WriteFile(trainPath, header, labels, featureCount, samplesPerLabel: 60);
            WriteFile(testPath, header, labels, featureCount, samplesPerLabel: 15);
        }

        private static void WriteFile(string path, string header, (string Label, float Base)[] labels, int featureCount, int samplesPerLabel)
        {
            var rand = new Random(42);

            using var sw = new StreamWriter(path);
            sw.WriteLine(header);

            foreach (var item in labels)
            {
                for (int s = 0; s < samplesPerLabel; s++)
                {
                    sw.Write(item.Label);

                    for (int i = 0; i < featureCount; i++)
                    {
                        float noise = (float)(rand.NextDouble() * 0.10 - 0.05);
                        float val = item.Base + noise;
                        sw.Write(",");
                        sw.Write(val.ToString("0.00000", CultureInfo.InvariantCulture));
                    }

                    sw.WriteLine();
                }
            }
        }
    }
}
