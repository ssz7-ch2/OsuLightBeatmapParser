using OsuLightBeatmapParser.Objects;
using System.Linq;

namespace OsuLightBeatmapParser.Helpers
{
    public static class BeatmapExtensions
    {
        public static double? BeatLengthAt(this Beatmap beatmap, int time)
        {
            return beatmap.UninheritedTimingPointAt(time)?.BeatLength;
        }

        public static double SliderVelocityAt(this Beatmap beatmap, int time)
        {
            return 100d * beatmap.Difficulty.SliderMultiplier * beatmap.BpmMultiplierAt(time);
        }

        public static double BpmMultiplierAt(this Beatmap beatmap, int time)
        {
            if (beatmap.TimingPoints.Count == 0)
                return 0;

            var timingPoint = beatmap.TimingPointAt(time);

            return MathHelper.CalculateBpmMultiplierFromBeatLength(timingPoint.BeatLength);
        }

        public static int ComboAt(this Beatmap beatmap, int time)
        {
            return beatmap.HitObjects.Where(h => h.EndTime < time).Sum(h => MathHelper.CalculateCombo(beatmap, h));
        }

        public static TimingPoint TimingPointAt(this Beatmap beatmap, int time)
        {
            if (time < beatmap.TimingPoints[0].Time)
                return beatmap.TimingPoints[0];
            for (int i = beatmap.TimingPoints.Count - 1; i >= 0; i--)
            {
                if (beatmap.TimingPoints[i].Time <= time)
                    return beatmap.TimingPoints[i];
            }

            return null;
        }

        public static TimingPoint UninheritedTimingPointAt(this Beatmap beatmap, int time)
        {
            TimingPoint firstUninherited = null;
            for (int i = beatmap.TimingPoints.Count - 1; i >= 0; i--)
            {
                if (beatmap.TimingPoints[i].Uninherited)
                {
                    if (beatmap.TimingPoints[i].Time <= time)
                        return beatmap.TimingPoints[i];
                    firstUninherited = beatmap.TimingPoints[i];
                }
            }

            return firstUninherited;
        }
    }
}
