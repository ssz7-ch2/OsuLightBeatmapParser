using System;
using System.Collections.Generic;

namespace OsuLightBeatmapParser.Sections
{
    public class EventsSection
    {
        public List<string> Unparsed { get; set; } = new List<string>();
        public string BackgroundImage { get; set; }
        public string Video { get; set; }
        public int VideoStartTime { get; set; }
        public List<Tuple<int,int>> Breaks { get; set; }
    }
}
