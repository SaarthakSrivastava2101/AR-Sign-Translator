using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ARSignTranslator.AI;
using ARSignTranslator.Dataset;
using ARSignTranslator.Output;
using ARSignTranslator.Core;
namespace ARSignTranslator
{
    public class App
    {
        public async Task RunAsync(CancellationToken token)
        {
            var config = new DatasetConfig
            {
                TrainCsvPath = "data/train.csv",
                TestCsvPath = "data/test.csv",
                FramesPerSample = 30,
                LandmarksPerFrame = 21
            };

            var modelPath = Path.Combine("data", "knn_model.json");

            Console.WriteLine("[INFO] Loading dataset...");
            var loader = new DatasetLoader(config);

            var trainSamples = loader.LoadTrainSamples();
            var testSamples = loader.LoadTestSamples();

            Console.WriteLine($"[INFO] Train Samples: {trainSamples.Count}");
            Console.WriteLine($"[INFO] Test Samples:  {testSamples.Count}");
            Console.WriteLine();

            IGestureClassifier classifier;

            if (File.Exists(modelPath))
            {
                Console.WriteLine("[INFO] Loading saved model...");
                var state = ModelSerializer.Load(modelPath);
                classifier = SimpleKnnClassifier.ImportState(state);
                Console.WriteLine("[INFO] Model loaded ✅");
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine("[INFO] Training classifier (KNN)...");
                var knn = new SimpleKnnClassifier(k: 9);
                knn.Fit(trainSamples);
                Console.WriteLine("[INFO] Training done ✅");
                Console.WriteLine();

                Console.WriteLine("[INFO] Saving model...");
                ModelSerializer.Save(modelPath, knn.ExportState());
                Console.WriteLine("[INFO] Model saved ✅");
                Console.WriteLine();

                classifier = knn;
            }

            var overlay = new ConsoleOverlayRenderer();
            var tts = new ConsoleTTS();

            
           
           var livePath = Path.Combine("data", "live_sample.csv");

if (File.Exists(livePath))
{
    Console.WriteLine();
    Console.WriteLine("[LIVE] Found live_sample.csv ✅");
     var livePrediction = LivePredictor.PredictOnce(modelPath, livePath);


    Console.WriteLine($"[LIVE] Predicted: {livePrediction.Label} (Conf: {livePrediction.Confidence:0.00})");

    if (livePrediction.Confidence >= 0.50f)
        tts.Speak(livePrediction.Label);
}
else
{
    Console.WriteLine();
    Console.WriteLine("[LIVE] live_sample.csv not found. Run live_capture.py first.");
}

            Console.WriteLine("[DONE] Evaluation finished.");

        }
    }
}
