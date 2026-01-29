namespace ARSignTranslator.Dataset
{
    public class DatasetConfig
    {
        // Paths to CSV files
        public string TrainCsvPath { get; set; } = "data/train.csv";
        public string TestCsvPath { get; set; } = "data/test.csv";

        // Gesture sample settings
        // Example: 30 frames per sample (1 second at 30 FPS)
        public int FramesPerSample { get; set; } = 30;

        // MediaPipe Hands = 21 landmarks
        public int LandmarksPerFrame { get; set; } = 21;

        // Each landmark has x,y,z → 3 values
        public int ValuesPerLandmark { get; set; } = 3;

        // Total expected features for each sample row
        public int ExpectedFeatureCount =>
            FramesPerSample * LandmarksPerFrame * ValuesPerLandmark;
    }
}
