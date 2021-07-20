using System.Collections.Generic;

namespace OsuLightBeatmapParser.Sections
{
    public class EditorSection
    {
        public List<string> Unparsed { get; set; }
        public int[] Bookmarks { get; set; }
        public float DistanceSpacing { get; set; }
        public int BeatDivisor { get; set; } = 1;
        public int GridSize { get; set; }
        public float TimelineZoom { get; set; } = 8;
    }
}
