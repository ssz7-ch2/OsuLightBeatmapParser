using System.Collections.Generic;

namespace OsuLightBeatmapParser.Sections
{
    public class MetadataSection
    {
        public List<string> Unparsed { get; set; }
        public string Title { get; set; }
        public string TitleUnicode { get; set; }
        public string Artist { get; set; }
        public string ArtistUnicode { get; set; }
        public string Creator { get; set; }
        public string Version { get; set; }
        public string Source { get; set; } = "";
        public HashSet<string> Tags { get; set; } = new();
        public int BeatmapID { get; set; }
        public int BeatmapSetID { get; set; } = -1;
    }
}
