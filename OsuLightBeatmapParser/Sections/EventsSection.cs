using OsuLightBeatmapParser.Objects;
using System.Collections.Generic;

namespace OsuLightBeatmapParser.Sections
{
    public class EventsSection
    {
        public List<string> Unparsed { get; set; } // = new();
        public string BackgroundImage { get; set; }
        public string Video { get; set; }
        public int VideoStartTime { get; set; }
        public List<Break> Breaks { get; set; }
    }
}
