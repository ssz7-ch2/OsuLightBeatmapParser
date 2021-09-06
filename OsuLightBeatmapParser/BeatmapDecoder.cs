using OsuLightBeatmapParser.Enums;
using OsuLightBeatmapParser.Helpers;
using OsuLightBeatmapParser.Objects;
using OsuLightBeatmapParser.Sections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;

namespace OsuLightBeatmapParser
{
    public static class BeatmapDecoder
    {
        public static Beatmap Decode(string path, FileSection[] fileSections)
        {
            var beatmap = new Beatmap();
            var currentSection = FileSection.Format;
            var parse = true;

            InitializeSections(beatmap);

            foreach (var line in File.ReadLines(path))
            {
                if (!string.IsNullOrWhiteSpace(line) && !line.StartsWith("//"))
                {
                    var section = ParseHelper.GetCurrentSection(line);
                    if (section != FileSection.None)
                    {
                        if (currentSection == FileSection.TimingPoints)
                            FixTimingPoints(beatmap);
                        currentSection = section;
                        parse = fileSections.Contains(currentSection);
                        if (!parse)
                            InitializeUnparsed(beatmap, currentSection);
                    }
                    else
                    {
                        ParseLine(beatmap, currentSection, line, parse);
                    }
                }
            }

            //FixColours(beatmap);
            CalculateExtraInfo(beatmap);

            return beatmap;
        }

        // only parses sections in fileSections, skips others
        public static Beatmap DecodeRead(string path, FileSection[] fileSections)
        {
            if (fileSections.Contains(FileSection.HitObjects))
            {
                var neededSections = new List<FileSection>();
                if (!fileSections.Contains(FileSection.Difficulty))
                    neededSections.Add(FileSection.Difficulty);
                if (!fileSections.Contains(FileSection.TimingPoints))
                    neededSections.Add(FileSection.TimingPoints);

                fileSections = neededSections.Concat(fileSections).ToArray();
            }

            var beatmap = new Beatmap();
            var currentSection = FileSection.Format;
            var skip = false;
            var sectionsToRead = fileSections.Length;


            foreach (var line in File.ReadLines(path))
            {
                if (!string.IsNullOrWhiteSpace(line) && !line.StartsWith("//"))
                {
                    var section = ParseHelper.GetCurrentSection(line);
                    if (section != FileSection.None)
                    {
                        if (sectionsToRead == 0)
                            break;
                        if (currentSection == FileSection.TimingPoints)
                            FixTimingPoints(beatmap);
                        currentSection = section;
                        skip = !fileSections.Contains(currentSection);
                        if (!skip)
                        {
                            InitializeSection(beatmap, currentSection);
                            sectionsToRead--;
                        }
                    }
                    else if (!skip)
                    {
                        ParseLine(beatmap, currentSection, line, true);
                    }
                }
            }

            //FixColours(beatmap);
            CalculateExtraInfo(beatmap);

            return beatmap;
        }

        public static Beatmap Decode(string path)
        {
            var beatmap = new Beatmap();
            var currentSection = FileSection.Format;

            InitializeSections(beatmap);

            foreach (var line in File.ReadLines(path))
            {
                if (!string.IsNullOrWhiteSpace(line) && !line.StartsWith("//"))
                {
                    var section = ParseHelper.GetCurrentSection(line);
                    if (section != FileSection.None)
                    {
                        if (currentSection == FileSection.TimingPoints)
                            FixTimingPoints(beatmap);
                        currentSection = section;
                    }
                    else
                        ParseLine(beatmap, currentSection, line, true);
                }
            }

            //FixColours(beatmap);
            CalculateExtraInfo(beatmap);

            return beatmap;
        }

        private static void FixTimingPoints(Beatmap beatmap)
        {
            // attempt to fix map as best as possible as it is impossible to replicate game exactly
            if (!beatmap.TimingPoints.Where(t => t.Uninherited).Any())
            {
                foreach (var timingPoint in beatmap.TimingPoints)
                    timingPoint.BeatLength = -100;

                beatmap.TimingPoints.Insert(0, new TimingPoint()
                {
                    Time = 0,
                    BeatLength = 1000,
                    Meter = 4,
                    SampleSet = SampleSet.Normal,
                    SampleIndex = 0,
                    Volume = 100,
                    Uninherited = true,
                    Effects = Effects.None
                });
            }
        }

        private static void CalculateExtraInfo(Beatmap beatmap)
        {
            if (beatmap.HitObjects != null && beatmap.HitObjects.Any() && beatmap.Difficulty != null && beatmap.TimingPoints != null)
            {
                beatmap.General.Length = beatmap.HitObjects.Last().EndTime - beatmap.HitObjects.First().StartTime;
                beatmap.General.MaxCombo = beatmap.ComboAt(beatmap.HitObjects.Last().EndTime + 1);
                beatmap.General.MainBPM = MathHelper.CalculateMainBPM(beatmap);
            }
        }

        private static void InitializeUnparsed(Beatmap beatmap, FileSection section)
        {
            switch (section)
            {
                case FileSection.General:
                    beatmap.General.Unparsed = new List<string>();
                    break;
                case FileSection.Editor:
                    beatmap.Editor.Unparsed = new List<string>();
                    break;
                case FileSection.Metadata:
                    beatmap.Metadata.Unparsed = new List<string>();
                    break;
                case FileSection.Difficulty:
                    beatmap.Difficulty.Unparsed = new List<string>();
                    break;
                case FileSection.Events:
                    beatmap.Events.Unparsed = new List<string>();
                    break;
                case FileSection.TimingPoints:
                    beatmap.TimingPoints.Unparsed = new List<string>();
                    break;
                case FileSection.Colours:
                    beatmap.Colours.Unparsed = new List<string>();
                    break;
                case FileSection.HitObjects:
                    beatmap.HitObjects.Unparsed = new List<string>();
                    break;
            }
        }

        private static void InitializeSections(Beatmap beatmap)
        {
            beatmap.General = new GeneralSection();
            beatmap.Editor = new EditorSection();
            beatmap.Metadata = new MetadataSection();
            beatmap.Difficulty = new DifficultySection();
            beatmap.Events = new EventsSection {Breaks = new List<Break>()};
            beatmap.TimingPoints = new TimingPointsSection();
            beatmap.Colours = new ColoursSection();
            beatmap.HitObjects = new HitObjectsSection();
        }

        private static void InitializeSection(Beatmap beatmap, FileSection section)
        {
            switch (section)
            {
                case FileSection.General:
                    beatmap.General = new GeneralSection();
                    break;
                case FileSection.Editor:
                    beatmap.Editor = new EditorSection();
                    break;
                case FileSection.Metadata:
                    beatmap.Metadata = new MetadataSection();
                    break;
                case FileSection.Difficulty:
                    beatmap.Difficulty = new DifficultySection();
                    break;
                case FileSection.Events:
                    beatmap.Events = new EventsSection {Breaks = new List<Break>()};
                    break;
                case FileSection.TimingPoints:
                    beatmap.TimingPoints = new TimingPointsSection();
                    break;
                case FileSection.Colours:
                    beatmap.Colours = new ColoursSection();
                    break;
                case FileSection.HitObjects:
                    beatmap.HitObjects = new HitObjectsSection();
                    break;
            }
        }

        private static void ParseLine(Beatmap beatmap, FileSection section, string line, bool parse)
        {
            switch (section)
            {
                case FileSection.Format:
                    beatmap.Version = Convert.ToInt32(line.Split('v')[1]);
                    break;
                case FileSection.General:
                    ParseGeneral(beatmap, line, parse);
                    break;
                case FileSection.Editor:
                    ParseEditor(beatmap, line, parse);
                    break;
                case FileSection.Metadata:
                    ParseMetadata(beatmap, line, parse);
                    break;
                case FileSection.Difficulty:
                    ParseDifficulty(beatmap, line, parse);
                    break;
                case FileSection.Events:
                    ParseEvents(beatmap, line, parse);
                    break;
                case FileSection.TimingPoints:
                    ParseTimingPoints(beatmap, line, parse);
                    break;
                case FileSection.Colours:
                    ParseColours(beatmap, line, parse);
                    break;
                case FileSection.HitObjects:
                    ParseHitObjects(beatmap, line, parse);
                    break; 
            }
        }

        public static void ParseGeneral(Beatmap beatmap, string line, bool parse)
        {
            if (!parse)
            {
                beatmap.General.Unparsed.Add(line);
                return;
            }

            var optionValue = line.Split(':', 2, StringSplitOptions.TrimEntries);
            var option = optionValue[0];
            var value = optionValue[1];

            switch (option)
            {
                case "AudioFilename":
                    beatmap.General.AudioFilename = value;
                    break;
                case "AudioLeadIn":
                    beatmap.General.AudioLeadIn = Convert.ToInt32(value);
                    break;
                case "PreviewTime":
                    beatmap.General.PreviewTime = Convert.ToInt32(value);
                    break;
                case "Countdown":
                    beatmap.General.Countdown = (CountdownType)Convert.ToInt32(value);
                    break;
                case "SampleSet":
                    beatmap.General.SampleSet = (SampleSet)Enum.Parse(typeof(SampleSet), value);
                    break;
                case "StackLeniency":
                    beatmap.General.StackLeniency = float.Parse(value);
                    break;
                case "Mode":
                    beatmap.General.Mode = (Ruleset)Convert.ToInt32(value);
                    break;
                case "LetterboxInBreaks":
                    beatmap.General.LetterboxInBreaks = value != "0";
                    break;
                case "UseSkinSprites":
                    beatmap.General.UseSkinSprites = value != "0";
                    break;
                case "OverlayPosition":
                    beatmap.General.OverlayPosition = (OverlayPositionType)Enum.Parse(typeof(OverlayPositionType), value);
                    break;
                case "SkinPreference":
                    beatmap.General.SkinPreference = value;
                    break;
                case "EpilepsyWarning":
                    beatmap.General.EpilepsyWarning = value != "0";
                    break;
                case "CountdownOffset":
                    beatmap.General.CountdownOffset = Convert.ToInt32(value);
                    break;
                case "SpecialStyle":
                    beatmap.General.SpecialStyle = value != "0";
                    break;
                case "WidescreenStoryboard":
                    beatmap.General.WidescreenStoryboard = value != "0";
                    break;
                case "SamplesMatchPlaybackRate":
                    beatmap.General.SamplesMatchPlaybackRate = value != "0";
                    break;
                case "EditorBookmarks":
                    // v5 or before
                    beatmap.Editor ??= new EditorSection();
                    beatmap.Editor.Bookmarks = value.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(b => Convert.ToInt32(b)).ToArray();
                    break;
                case "Index":
                    beatmap.General.Index = Convert.ToInt32(value);
                    break;
                case "Total":
                    beatmap.General.Total = Convert.ToInt32(value);
                    break;
                case "StartTime":
                    beatmap.General.StartTime = Convert.ToInt32(value);
                    break;
                case "Script":
                    beatmap.General.Script = value;
                    break;
            }
        }

        public static void ParseEditor(Beatmap beatmap, string line, bool parse)
        {
            if (!parse)
            {
                beatmap.Editor.Unparsed.Add(line);
                return;
            }

            var optionValue = line.Split(':', 2, StringSplitOptions.TrimEntries);
            var option = optionValue[0];
            var value = optionValue[1];

            switch (option)
            {
                case "Bookmarks":
                    beatmap.Editor.Bookmarks = value.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(b => Convert.ToInt32(b)).ToArray();
                    break;
                case "DistanceSpacing":
                    beatmap.Editor.DistanceSpacing = float.Parse(value);
                    break;
                case "BeatDivisor":
                    beatmap.Editor.BeatDivisor = Convert.ToInt32(value);
                    break;
                case "GridSize":
                    beatmap.Editor.GridSize = Convert.ToInt32(value);
                    break;
                case "TimelineZoom":
                    beatmap.Editor.TimelineZoom = float.Parse(value);
                    break;
            }
        }

        public static void ParseMetadata(Beatmap beatmap, string line, bool parse)
        {
            if (!parse)
            {
                beatmap.Metadata.Unparsed.Add(line);
                return;
            }

            var optionValue = line.Split(':', 2, StringSplitOptions.TrimEntries);
            var option = optionValue[0];
            var value = optionValue[1];

            switch (option)
            {
                case "Title":
                    beatmap.Metadata.Title = value;
                    if (beatmap.Version < 10)
                        beatmap.Metadata.TitleUnicode = value;
                    break;
                case "TitleUnicode":
                    beatmap.Metadata.TitleUnicode = value;
                    break;
                case "Artist":
                    beatmap.Metadata.Artist = value;
                    if (beatmap.Version < 10)
                        beatmap.Metadata.ArtistUnicode = value;
                    break;
                case "ArtistUnicode":
                    beatmap.Metadata.ArtistUnicode = value;
                    break;
                case "Creator":
                    beatmap.Metadata.Creator = value;
                    break;
                case "Version":
                    beatmap.Metadata.Version = value;
                    break;
                case "Source":
                    beatmap.Metadata.Source = value;
                    break;
                case "Tags":
                    beatmap.Metadata.Tags = value.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToHashSet();
                    break;
                case "BeatmapID":
                    beatmap.Metadata.BeatmapID = Convert.ToInt32(value);
                    break;
                case "BeatmapSetID":
                    beatmap.Metadata.BeatmapSetID = Convert.ToInt32(value);
                    break;
            }
        }

        public static void ParseDifficulty(Beatmap beatmap, string line, bool parse)
        {
            if (!parse)
            {
                beatmap.Difficulty.Unparsed.Add(line);
                return;
            }

            var optionValue = line.Split(':', 2, StringSplitOptions.TrimEntries);
            var option = optionValue[0];
            var value = optionValue[1];

            switch (option)
            {
                case "HPDrainRate":
                    beatmap.Difficulty.HPDrainRate = float.Parse(value);
                    break;
                case "CircleSize":
                    beatmap.Difficulty.CircleSize = float.Parse(value);
                    break;
                case "OverallDifficulty":
                    beatmap.Difficulty.OverallDifficulty = float.Parse(value);
                    if (beatmap.Version < 8)
                        beatmap.Difficulty.ApproachRate = beatmap.Difficulty.OverallDifficulty;
                    break;
                case "ApproachRate":
                    beatmap.Difficulty.ApproachRate = float.Parse(value);
                    break;
                case "SliderMultiplier":
                    beatmap.Difficulty.SliderMultiplier = double.Parse(value);
                    break;
                case "SliderTickRate":
                    beatmap.Difficulty.SliderTickRate = double.Parse(value);
                    break;
            }
        }

        public static void ParseEvents(Beatmap beatmap, string line, bool parse)
        {
            if (!parse)
            {
                beatmap.Events.Unparsed.Add(line);
                return;
            }

            var tokens = line.Split(',');

            if (line[0] == ' ' || line[0] == '_')
            {
                // beatmap.Events.Unparsed.Add(line);
                return;
            }

            if (Enum.TryParse(tokens[0], out EventType eventType))
            {
                switch (eventType)
                {
                    case EventType.Background:
                        beatmap.Events.BackgroundImage = tokens[2].Trim('"');
                        break;
                    case EventType.Video:
                        beatmap.Events.Video = tokens[2].Trim('"');
                        beatmap.Events.VideoStartTime = Convert.ToInt32(tokens[1]);
                        break;
                    case EventType.Break:
                        beatmap.Events.Breaks.Add(new Break{StartTime = Convert.ToInt32(tokens[1]), EndTime = Convert.ToInt32(tokens[2])});
                        break;
                    default:
                        // beatmap.Events.Unparsed.Add(line);
                        break;
                }
            }
        }

        public static void ParseTimingPoints(Beatmap beatmap, string line, bool parse)
        {
            if (!parse)
            {
                beatmap.TimingPoints.Unparsed.Add(line);
                return;
            }

            var tokens = line.Split(',', StringSplitOptions.TrimEntries);

            beatmap.TimingPoints.Add(new TimingPoint
            {
                Time = (int)float.Parse(tokens[0]),
                BeatLength = double.Parse(tokens[1]),
                Meter = tokens.Length >= 3 ? Convert.ToInt32(tokens[2]) : 4,
                SampleSet = tokens.Length >= 4 ? (SampleSet)Convert.ToInt32(tokens[3]) : SampleSet.None,
                SampleIndex = tokens.Length >= 5 ? Convert.ToInt32(tokens[4]) : 0,
                Volume = tokens.Length >= 6 ? Convert.ToInt32(tokens[5]) : 100,
                Uninherited = tokens.Length < 7 || tokens[6] != "0",
                Effects = tokens.Length >= 8 ? (Effects)Convert.ToInt32(tokens[7]) : Effects.None
            });
        }

        public static void ParseColours(Beatmap beatmap, string line, bool parse)
        {
            if (!parse)
            {
                beatmap.Colours.Unparsed.Add(line);
                return;
            }

            var optionValue = line.Split(':', 2, StringSplitOptions.TrimEntries);
            var option = optionValue[0];
            var colour = optionValue[1].Split(',').Select(c => Convert.ToByte(c)).ToArray();


            switch (option)
            {
                case "SliderTrackOverride":
                    beatmap.Colours.SliderTrackOverride = colour;
                    break;
                case "SliderBorder":
                    beatmap.Colours.SliderBorder = colour;
                    break;
                default:
                    var colourIndex = option[5] - '0' - 1;
                    if (colourIndex < beatmap.Colours.ComboColours.Count)
                        beatmap.Colours.ComboColours[colourIndex] = colour;
                    else
                    {
                        for (int i = 0; i < colourIndex - beatmap.Colours.ComboColours.Count; i++)
                            beatmap.Colours.ComboColours.Add(new byte[3] {255, 255, 255});
                        beatmap.Colours.ComboColours.Insert(colourIndex, colour);
                    }
                    break;
            }
        }

        private static void FixColours(Beatmap beatmap)
        {
            if (beatmap.Colours is null || beatmap.Colours.ComboColours.Count <= 1)
                return;
            // game is weird man
            var firstColour = beatmap.Colours.ComboColours[0];
            beatmap.Colours.ComboColours.Remove(firstColour);
            beatmap.Colours.ComboColours.Add(firstColour);
        }

        public static void ParseHitObjects(Beatmap beatmap, string line, bool parse)
        {
            if (!parse)
            {
                beatmap.HitObjects.Unparsed.Add(line);
                return;
            }

            var tokens = line.Split(',', StringSplitOptions.TrimEntries);

            var position = new Vector2(float.Parse(tokens[0]), float.Parse(tokens[1]));
            var startTime = Convert.ToInt32(tokens[2]);
            var type = int.Parse(tokens[3]);
            var hitObjectType = (HitObjectType)type & (HitObjectType.Circle | HitObjectType.Slider | HitObjectType.Spinner | HitObjectType.Hold);

            if (!Enum.IsDefined(typeof(HitObjectType), hitObjectType))
                hitObjectType = (HitObjectType)type & (HitObjectType.Circle | HitObjectType.Slider | HitObjectType.Spinner);

            var comboColourOffset = (type & (int)HitObjectType.ComboColourOffset) >> 4;
            var newCombo = (type & (int)HitObjectType.NewCombo) > 0;
            var hitSound = (HitSoundType)Convert.ToInt32(tokens[4]);

            HitObject hitObject = null;
            string[] hitSample = null;

            switch (hitObjectType)
            {
                case HitObjectType.Circle:
                    hitObject = new HitCircle
                    {
                        Position = position,
                        StartTime = startTime,
                        EndTime = startTime,
                        HitSound = hitSound,
                        NewCombo = newCombo,
                        ComboColourOffset = comboColourOffset
                    };

                    hitSample = tokens.Length >= 6 ? tokens[5].Split(':') : null;
                    break;
                case HitObjectType.Slider:
                    var curveType = ParseHelper.GetCurveType(tokens[5][0]);
                    var curvePoints = tokens[5].Split('|').Skip(1).Select(p =>
                    {
                        var pos = p.Split(':');
                        return new Vector2(float.Parse(pos[0]), float.Parse(pos[1]));
                    }).ToList();

                    var slides = Convert.ToInt32(tokens[6]);
                    var length = double.Parse(tokens[7]);

                    var endTime = 0;
                    try
                    {
                        endTime = MathHelper.CalculateEndTime(beatmap, startTime, slides, length);
                    }
                    catch (Exception)
                    {
                        // can't calculate end time without timing points and difficulty section
                    }

                    hitObject = new Slider
                    {
                        Position = position,
                        StartTime = startTime,
                        EndTime = endTime,
                        HitSound = hitSound,
                        NewCombo = newCombo,
                        CurveType = curveType,
                        CurvePoints = curvePoints,
                        Slides = slides,
                        Length = length
                    };

                    var hitSampleIndex = 8;

                    if (tokens.Length >= 9 && tokens[8].Length > 0 && tokens[8].Contains('|'))
                    {
                        ((Slider) hitObject).EdgeSounds =
                            tokens[8].Split('|').Select(e => (HitSoundType) Convert.ToInt32(e)).ToList();
                        hitSampleIndex = 9;
                    }

                    if (tokens.Length >= 10 && tokens[9].Length > 0 && tokens[9].Contains('|'))
                    {
                        ((Slider)hitObject).EdgeSets =
                            tokens[9].Split('|').Select(s =>
                            {
                                var set = s.Split(':');
                                return new Tuple<SampleSet, SampleSet>((SampleSet) Convert.ToInt32(set[0]),
                                    (SampleSet) Convert.ToInt32(set[1]));
                            }).ToList();
                        hitSampleIndex = 10;
                    }

                    hitSample = tokens.Length > hitSampleIndex ? tokens[hitSampleIndex].Split(':') : null;
                    break;
                case HitObjectType.Spinner:
                    hitObject = new Spinner
                    {
                        Position = position,
                        StartTime = startTime,
                        EndTime = Convert.ToInt32(tokens[5]),
                        HitSound = hitSound,
                        NewCombo = newCombo,
                        ComboColourOffset = comboColourOffset
                    };

                    hitSample = tokens.Length >= 7 ? tokens[6].Split(':') : null;
                    break;
                case HitObjectType.Hold:
                    var additions = tokens[5].Split(':', 2);

                    hitObject = new Hold
                    {
                        Position = position,
                        StartTime = startTime,
                        EndTime = Convert.ToInt32(additions[0]),
                        HitSound = hitSound,
                        NewCombo = newCombo,
                        ComboColourOffset = comboColourOffset
                    };

                    hitSample = additions[1].Split(':');
                    break;
            }

            if (hitObject == null) return;

            hitObject.HitSample = hitSample is null || hitSample.Length < 2 ? null : new HitSample
            {
                NormalSet = (SampleSet) Convert.ToInt32(hitSample[0]),
                AdditionSet = (SampleSet) Convert.ToInt32(hitSample[1]),
                Index = hitSample.Length >= 3 ? Convert.ToInt32(hitSample[2]) : 0,
                Volume = hitSample.Length >= 4 ? Convert.ToInt32(hitSample[3]) : 0,
                Filename = hitSample.Length >= 5 ? hitSample[4] : string.Empty
            };

            beatmap.HitObjects.Add(hitObject);
        }
    }
}
