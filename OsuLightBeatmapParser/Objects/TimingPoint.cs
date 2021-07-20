using OsuLightBeatmapParser.Enums;

namespace OsuLightBeatmapParser.Objects
{
    public class TimingPoint
    {
        public int Time { get; set; }
        public double BeatLength { get; set; }
        public int Meter { get; set; }
        public SampleSet SampleSet { get; set; }
        public int SampleIndex { get; set; }
        public int Volume { get; set; }
        public bool Uninherited { get; set; }
        public Effects Effects { get; set; }
    }
}
