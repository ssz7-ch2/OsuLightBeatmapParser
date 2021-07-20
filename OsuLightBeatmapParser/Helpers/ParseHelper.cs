using OsuLightBeatmapParser.Enums;
using System;

namespace OsuLightBeatmapParser.Helpers
{
    internal static class ParseHelper
    {
        public static FileSection GetCurrentSection(string line)
        {
            if (line[0] != '[')
                return FileSection.None;
            Enum.TryParse(line.Trim('[', ']'), true, out FileSection parsedSection);
            return parsedSection;
        }

        public static CurveType GetCurveType(char c)
        {
            return c switch
            {
                'C' => CurveType.Catmull,
                'B' => CurveType.Bezier,
                'L' => CurveType.Linear,
                'P' => CurveType.PerfectCurve,
                _ => CurveType.PerfectCurve
            };
        }
    }
}
