using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace ARSignTranslator.Dataset
{
    public class DatasetLoader
    {
        private readonly DatasetConfig _config;

        public DatasetLoader(DatasetConfig config)
        {
            _config = config;
        }

        public List<GestureSample> LoadTrainSamples()
        {
            return LoadCsv(_config.TrainCsvPath);
        }

        public List<GestureSample> LoadTestSamples()
        {
            return LoadCsv(_config.TestCsvPath);
        }

        private List<GestureSample> LoadCsv(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException($"CSV file not found: {path}");

            var samples = new List<GestureSample>();

            using var sr = new StreamReader(path);
            sr.ReadLine(); // header skip

            while (!sr.EndOfStream)
            {
                var line = sr.ReadLine();
                if (string.IsNullOrWhiteSpace(line)) continue;

                var parts = line.Split(',', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 2) continue;

                var label = parts[0].Trim();

                var featureCount = parts.Length - 1;
                if (featureCount != _config.ExpectedFeatureCount)
                    throw new InvalidDataException(
                        $"Invalid feature count in {path}. Expected {_config.ExpectedFeatureCount}, got {featureCount}");

                var features = new float[_config.ExpectedFeatureCount];

                for (int i = 0; i < _config.ExpectedFeatureCount; i++)
                {
                    features[i] = float.Parse(parts[i + 1], CultureInfo.InvariantCulture);
                }

                samples.Add(new GestureSample(label, features));
            }

            return samples;
        }

        // ✅ THIS IS THE LIVE METHOD (correct place)
        public static GestureSample LoadSingleSample(string csvPath)
        {
            if (!File.Exists(csvPath))
                throw new FileNotFoundException($"Live CSV not found: {csvPath}");

            var line = File.ReadAllLines(csvPath)[0];
            var parts = line.Split(',', StringSplitOptions.RemoveEmptyEntries);

            var features = new float[parts.Length - 1];

            for (int i = 1; i < parts.Length; i++)
            {
                features[i - 1] = float.Parse(parts[i], CultureInfo.InvariantCulture);
            }

            return new GestureSample("LIVE", features);
        }
    }
}
