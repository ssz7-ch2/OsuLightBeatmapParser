using OsuLightBeatmapParser.Objects;
using System.Collections.Generic;

namespace OsuLightBeatmapParser.Sections
{
    public class HitObjectsSection : List<HitObject>
    {
        public List<string> Unparsed { get; set; }
    }
}
