using OsuLightBeatmapParser.Enums;
using OsuLightBeatmapParser.Helpers;
using OsuLightBeatmapParser.Sections;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OsuLightBeatmapParser
{
    public static class BeatmapEncoder
    {
        public static List<string> Encode(Beatmap beatmap)
        {
            var contents = new List<string>
            {
                $"osu file format v14" // always save as latest version
            };

            contents.AddRange(GeneralSection(beatmap.General));
            contents.AddRange(EditorSection(beatmap.Editor));
            contents.AddRange(MetadataSection(beatmap.Metadata));
            contents.AddRange(DifficultySection(beatmap.Difficulty));
            contents.AddRange(EventsSection(beatmap.Events));
            contents.AddRange(TimingPointsSection(beatmap.TimingPoints));
            contents.AddRange(ColoursSection(beatmap.Colours));
            contents.AddRange(HitObjectsSection(beatmap.HitObjects));

            return contents;
        }

        public static List<string> GeneralSection(GeneralSection section)
        {
            if (section.Unparsed != null)
            {
                section.Unparsed.Insert(0, string.Empty);
                section.Unparsed.Insert(1, "[General]");
                return section.Unparsed;
            }

            var list = new List<string>
            {
                string.Empty,
                "[General]",
                "AudioFilename: " + section.AudioFilename,
                "AudioLeadIn: " + section.AudioLeadIn,
                "PreviewTime: " + section.PreviewTime,
                "Countdown: " + (int)section.Countdown,
                "SampleSet: " + section.SampleSet,
                "StackLeniency: " + section.StackLeniency,
                "Mode: " + (int)section.Mode,
                "LetterboxInBreaks: " + Convert.ToInt32(section.LetterboxInBreaks)
            };

            if (section.UseSkinSprites)
                list.Add("UseSkinSprites: " + Convert.ToInt32(section.UseSkinSprites));
            if (section.OverlayPosition != OverlayPositionType.NoChange)
                list.Add("OverlayPosition: " + section.OverlayPosition);
            if (!string.IsNullOrEmpty(section.SkinPreference))
                list.Add("SkinPreference: " + section.SkinPreference);
            if (section.EpilepsyWarning)
                list.Add("EpilepsyWarning: " + Convert.ToInt32(section.EpilepsyWarning));
            if (section.CountdownOffset != 0)
                list.Add("CountdownOffset: " + section.CountdownOffset);
            if (section.Mode == Ruleset.Mania)
                list.Add("SpecialStyle: " + Convert.ToInt32(section.SpecialStyle));

            list.Add("WidescreenStoryboard: " + Convert.ToInt32(section.WidescreenStoryboard));
            if (section.SamplesMatchPlaybackRate)
                list.Add("SamplesMatchPlaybackRate: " + Convert.ToInt32(section.SamplesMatchPlaybackRate));

            return list;
        }

        public static List<string> EditorSection(EditorSection section)
        {
            if (section.Unparsed != null)
            {
                section.Unparsed.Insert(0, string.Empty);
                section.Unparsed.Insert(1, "[Editor]");
                return section.Unparsed;
            }

            var list = new List<string>
            {
                string.Empty,
                "[Editor]"
            };

            if (section.Bookmarks != null)
                list.Add("Bookmarks: " + string.Join(',', section.Bookmarks));

            list.AddRange(new List<string>
            {
                "DistanceSpacing: " + section.DistanceSpacing,
                "BeatDivisor: " + section.BeatDivisor,
                "GridSize: " + section.GridSize,
                "TimelineZoom: " + section.TimelineZoom
            });

            return list;
        }

        public static List<string> MetadataSection(MetadataSection section)
        {
            if (section.Unparsed != null)
            {
                section.Unparsed.Insert(0, string.Empty);
                section.Unparsed.Insert(1, "[Metadata]");
                return section.Unparsed;
            }

            return new List<string>
            {
                string.Empty,
                "[Metadata]",
                "Title:" + section.Title,
                "TitleUnicode:" + section.TitleUnicode,
                "Artist:" + section.Artist,
                "ArtistUnicode:" + section.ArtistUnicode,
                "Creator:" + section.Creator,
                "Version:" + section.Version,
                "Source:" + section.Source,
                "Tags:" + string.Join(' ', section.Tags),
                "BeatmapID:" + section.BeatmapID,
                "BeatmapSetID:" + section.BeatmapSetID,
            };
        }

        public static List<string> DifficultySection(DifficultySection section)
        {
            if (section.Unparsed != null)
            {
                section.Unparsed.Insert(0, string.Empty);
                section.Unparsed.Insert(1, "[Difficulty]");
                return section.Unparsed;
            }

            return new List<string>
            {
                string.Empty,
                "[Difficulty]",
                "HPDrainRate:" + section.HPDrainRate,
                "CircleSize:" + section.CircleSize,
                "OverallDifficulty:" + section.OverallDifficulty,
                "ApproachRate:" + section.ApproachRate,
                "SliderMultiplier:" + section.SliderMultiplier,
                "SliderTickRate:" + section.SliderTickRate
            };
        }

        public static List<string> EventsSection(EventsSection section)
        {
            var list = new List<string>
            {
                string.Empty,
                "[Events]"
            };

            if (section.BackgroundImage != null)
                list.Add($"0,0,\"{section.BackgroundImage}\",0,0");

            if (section.Video != null)
                list.Add($"Video,{section.VideoStartTime},\"{section.Video}\"");

            if (section.Breaks.Any())
                list.AddRange(section.Breaks.ConvertAll(b => $"2,{b.Item1},{b.Item2}"));

            if (section.Unparsed.Count > 0)
                list.AddRange(section.Unparsed);

            return list;
        }

        public static List<string> TimingPointsSection(TimingPointsSection section)
        {
            if (section.Unparsed != null)
            {
                if (section.Unparsed.Count == 0)
                    return new List<string>();
                section.Unparsed.Insert(0, string.Empty);
                section.Unparsed.Insert(1, "[TimingPoints]");
                return section.Unparsed;
            }

            if (section.Count == 0)
                return new List<string>();

            var list = new List<string>
            {
                string.Empty,
                "[TimingPoints]"
            };

            list.AddRange(section.ConvertAll(WriteHelper.TimingPoint));

            return list;
        }

        public static List<string> ColoursSection(ColoursSection section)
        {
            if (section.Unparsed != null)
            {
                if (section.Unparsed.Count == 0)
                    return new List<string>();
                section.Unparsed.Insert(0, string.Empty);
                section.Unparsed.Insert(1, "[Colours]");
                return section.Unparsed;
            }

            if (section.ComboColours.Count == 0 && section.SliderTrackOverride is null && section.SliderBorder is null)
                return new List<string>();

            var list = new List<string>
            {
                string.Empty,
                "[Colours]"
            };

            if (section.ComboColours.Count != 0)
                for (int i = 0; i < section.ComboColours.Count; i++)
                    list.Add($"Combo{i + 1} : {WriteHelper.Colour(section.ComboColours[i])}");

            if (section.SliderTrackOverride != null)
                list.Add($"SliderTrackOverride : {WriteHelper.Colour(section.SliderTrackOverride)}");

            if (section.SliderBorder != null)
                list.Add($"SliderBorder : {WriteHelper.Colour(section.SliderBorder)}");

            return list;
        }

        public static List<string> HitObjectsSection(HitObjectsSection section)
        {
            if (section.Unparsed != null)
            {
                section.Unparsed.Insert(0, string.Empty);
                section.Unparsed.Insert(1, "[HitObjects]");
                return section.Unparsed;
            }

            var list = new List<string>
            {
                string.Empty,
                "[HitObjects]"
            };

            list.AddRange(section.ConvertAll(WriteHelper.HitObject));
            return list;
        }
    }
}
