namespace ARSignTranslator.Dataset
{
    public class DatasetConfig
    {
        
        public string TrainCsvPath { get; set; } = "data/train.csv";
        public string TestCsvPath { get; set; } = "data/test.csv";

        public int FramesPerSample { get; set; } = 30;
        public int LandmarksPerFrame { get; set; } = 21;


        public int ValuesPerLandmark { get; set; } = 3;

        public int ExpectedFeatureCount =>
            FramesPerSample * LandmarksPerFrame * ValuesPerLandmark;
    }
}
