using OsuLightBeatmapParser.Enums;
using System.Collections.Generic;

namespace OsuLightBeatmapParser.Sections
{
    public class GeneralSection
    {
        public List<string> Unparsed { get; set; }
        public string AudioFilename { get; set; }
        public int AudioLeadIn { get; set; }
        public int PreviewTime { get; set; } = -1;
        public CountdownType Countdown { get; set; } = CountdownType.Normal;
        public SampleSet SampleSet { get; set; } = SampleSet.Normal;
        public float StackLeniency { get; set; } = 0.7f;
        public Ruleset Mode { get; set; } = Ruleset.Standard;
        public bool LetterboxInBreaks { get; set; }
        public bool UseSkinSprites { get; set; }
        public OverlayPositionType OverlayPosition { get; set; } = OverlayPositionType.NoChange;
        public string SkinPreference { get; set; }
        public bool EpilepsyWarning { get; set; }
        public int CountdownOffset { get; set; }
        public bool SpecialStyle { get; set; }
        public bool WidescreenStoryboard { get; set; }
        public bool SamplesMatchPlaybackRate { get; set; }
    }
}
