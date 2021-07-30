using OsuLightBeatmapParser.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public static double CalculateBeatLengthFromBpmMultiplier(double multiplier)
        {
            return 100f / -multiplier;
        }

        public static double CalculateBpmMultiplierFromSliderLength(Beatmap beatmap, double sliderLength, int startTime, int duration, int combo)
        {
            return sliderLength * beatmap.BeatLengthAt(startTime) * (combo - 1) /
                   (duration * 100d * beatmap.Difficulty.SliderMultiplier);
        }

        public static int CalculateSliderDuration(Beatmap beatmap, int startTime, int slides, double length)
        {
            return (int)(length / beatmap.SliderVelocityAt(startTime) * slides * beatmap.BeatLengthAt(startTime));
        }

        public static int CalculateEndTime(Beatmap beatmap, int startTime, int slides, double length)
        {
            return startTime + CalculateSliderDuration(beatmap, startTime, slides, length);
        }

        public static int CalculateSliderCombo(Beatmap beatmap, Slider slider)
        {
            int combo = 1;
            double scoringPointDistance = 100 * beatmap.Difficulty.SliderMultiplier / beatmap.Difficulty.SliderTickRate;
            double bpmMultiplier = beatmap.BpmMultiplierAt(slider.StartTime);
            double tickDistance = (beatmap.Version < 8) ? scoringPointDistance : scoringPointDistance * bpmMultiplier;

            double velocity = scoringPointDistance * beatmap.Difficulty.SliderTickRate * bpmMultiplier * (1000F / beatmap.BeatLengthAt(slider.StartTime));
            int ticks = Math.Max((int)Math.Floor((slider.Length - velocity * 0.01) / tickDistance), 0);

            combo += (ticks * slider.Slides) + slider.Slides;

            return combo;
        }

        public static int CalculateCombo(Beatmap beatmap, HitObject hitObject)
        {
            if (hitObject is Slider slider)
                return CalculateSliderCombo(beatmap, slider);
            return 1;
        }

        public static double CalculateMainBPM(Beatmap beatmap)
        {
            // beatLength, duration of that beatLength
            var beatLengthsDuration = new Dictionary<double, int>();
            var uninheritedTimingPoints = beatmap.TimingPoints.Where(t => t.Uninherited).ToList();
            for (int i = 0; i < uninheritedTimingPoints.Count - 1; i++)
            {
                var beatLength = uninheritedTimingPoints[i].BeatLength;
                if (!beatLengthsDuration.ContainsKey(beatLength))
                    beatLengthsDuration.Add(beatLength, 0);
                beatLengthsDuration[beatLength] += uninheritedTimingPoints[i + 1].Time - uninheritedTimingPoints[i].Time;
            }
            var last = uninheritedTimingPoints.Last();
            if (!beatLengthsDuration.ContainsKey(last.BeatLength))
                beatLengthsDuration.Add(last.BeatLength, 0);
            beatLengthsDuration[last.BeatLength] += Math.Max(0, beatmap.HitObjects.Last().StartTime - last.Time);

            var longestDuration = beatLengthsDuration.Values.Max();
            if (longestDuration == 0)
                return 0;
            return 60 * 1000 / beatLengthsDuration.First(b => b.Value == longestDuration).Key;
        }
    }
}
