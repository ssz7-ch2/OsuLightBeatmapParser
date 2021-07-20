using System.Collections.Generic;

namespace OsuLightBeatmapParser.Sections
{
    public class DifficultySection
    {
        public List<string> Unparsed { get; set; }
        public float HPDrainRate { get; set; }
        public float CircleSize { get; set; }
        public float OverallDifficulty { get; set; }
        public float ApproachRate { get; set; }
        public double SliderMultiplier { get; set; }
        public double SliderTickRate { get; set; }
    }
}
