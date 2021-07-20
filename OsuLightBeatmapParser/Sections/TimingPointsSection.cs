using OsuLightBeatmapParser.Objects;
using System.Collections.Generic;

namespace OsuLightBeatmapParser.Sections
{
    public class TimingPointsSection : List<TimingPoint>
    {
        public List<string> Unparsed { get; set; }
    }
}
