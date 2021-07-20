using System;

namespace OsuLightBeatmapParser.Helpers
{
    public static class MathHelper
    {
        public static double CalculateBpmMultiplierFromBeatLength(double beatLength)
        {
            if (beatLength >= 0)
                return 1;

            return 100f / Math.Clamp((float)-beatLength, 10, 1000);
        }

        public static int CalculateSliderDuration(Beatmap beatmap, int startTime, int slides, double length)
        {
            return (int)(length / BeatmapHelper.SliderVelocityAt(beatmap, startTime) * slides * BeatmapHelper.BeatLengthAt(beatmap, startTime));
        }

        public static int CalculateEndTime(Beatmap beatmap, int startTime, int slides, double length)
        {
            return startTime + CalculateSliderDuration(beatmap, startTime, slides, length);
        }

    }
}
