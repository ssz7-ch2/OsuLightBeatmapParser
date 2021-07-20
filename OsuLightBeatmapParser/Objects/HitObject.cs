using OsuLightBeatmapParser.Enums;
using System.Numerics;

namespace OsuLightBeatmapParser.Objects
{
    public class HitObject
    {
        public Vector2 Position { get; set; }
        public int StartTime { get; set; }
        public int EndTime { get; set; }
        public HitSoundType HitSound { get; set; } = 0;
        public HitSample HitSample { get; set; }
        public bool NewCombo { get; set; }
        public int ComboColourOffset { get; set; }
    }
}
