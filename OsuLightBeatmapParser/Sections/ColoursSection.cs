using System.Collections.Generic;

namespace OsuLightBeatmapParser.Sections
{
    public class ColoursSection
    {
        public List<string> Unparsed { get; set; }
        public List<byte[]> ComboColours { get; set; } = new List<byte[]>();
        public byte[] SliderTrackOverride { get; set; }
        public byte[] SliderBorder { get; set; }
    }
}
