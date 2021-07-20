using OsuLightBeatmapParser.Enums;

namespace OsuLightBeatmapParser.Objects
{
    public class HitSample
    {
        public SampleSet NormalSet { get; set; }
        public SampleSet AdditionSet { get; set; }
        public int Index { get; set; }
        public int Volume { get; set; }
        public string Filename { get; set; }
    }
}
