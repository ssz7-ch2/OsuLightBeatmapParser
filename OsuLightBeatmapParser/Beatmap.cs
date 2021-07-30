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
        public string FileName => string.Join("", $"{Metadata.Artist} - {Metadata.Title} ({Metadata.Creator}) [{Metadata.Version}].osu".Split(Path.GetInvalidFileNameChars()));
        public void Save(string folderPath) => File.WriteAllLines(Path.Combine(folderPath, FileName), BeatmapEncoder.Encode(this));
    }
}
