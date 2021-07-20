using OsuLightBeatmapParser.Objects;

namespace OsuLightBeatmapParser.Helpers
{
    public static class BeatmapHelper
    {
        public static double BeatLengthAt(Beatmap beatmap, int time)
        {
            return UninheritedTimingPointAt(beatmap, time).BeatLength;
        }

        public static double SliderVelocityAt(Beatmap beatmap, int time)
        {
            return 100d * beatmap.Difficulty.SliderMultiplier * BpmMultiplierAt(beatmap, time);
        }

        public static double BpmMultiplierAt(Beatmap beatmap, int time)
        {
            if (beatmap.TimingPoints.Count == 0)
                return 0;

            var timingPoint = TimingPointAt(beatmap, time);

            return MathHelper.CalculateBpmMultiplierFromBeatLength(timingPoint.BeatLength);
        }

        public static TimingPoint TimingPointAt(Beatmap beatmap, int time)
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

        public static TimingPoint UninheritedTimingPointAt(Beatmap beatmap, int time)
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
