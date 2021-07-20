using OsuLightBeatmapParser.Sections;
using System.IO;

namespace OsuLightBeatmapParser
{
    public class Beatmap
    {
        public int Version { get; set; } = 14;
        public GeneralSection General { get; set; }
        public EditorSection Editor { get; set; }
        public MetadataSection Metadata { get; set; }
        public DifficultySection Difficulty { get; set; }
        public EventsSection Events { get; set; }
        public TimingPointsSection TimingPoints { get; set; }
        public ColoursSection Colours { get; set; }
        public HitObjectsSection HitObjects { get; set; }

        public void Save(string path)
        {
            File.WriteAllLines(path, BeatmapEncoder.Encode(this));
        }
    }
}
