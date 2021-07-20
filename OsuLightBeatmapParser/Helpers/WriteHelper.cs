using OsuLightBeatmapParser.Enums;
using OsuLightBeatmapParser.Objects;
using System;
using System.Collections.Generic;

namespace OsuLightBeatmapParser.Helpers
{
    public static class WriteHelper
    {
        public static string TimingPoint(TimingPoint timingPoint)
        {
            var offset = timingPoint.Time;
            var beatLength = timingPoint.BeatLength;
            var meter = timingPoint.Meter;
            var sampleSet = (int)timingPoint.SampleSet;
            var sampleIndex = timingPoint.SampleIndex;
            var volume = timingPoint.Volume;
            var uninherited = Convert.ToInt32(timingPoint.Uninherited);
            var effects = (int)timingPoint.Effects;

            return $"{offset},{beatLength},{meter},{sampleSet},{sampleIndex},{volume},{uninherited},{effects}";
        }

        public static string Colour(byte[] colour)
        {
            return $"{colour[0]},{colour[1]},{colour[2]}";
        }

        public static string HitObject(HitObject hitObject)
        {
            var list = new List<object>
            {
                hitObject.Position.X,
                hitObject.Position.Y,
                hitObject.StartTime,
                TypeByte(hitObject),
                (int)hitObject.HitSound
            };
            var hitSample = HitSample(hitObject.HitSample);

            if (hitObject is Slider slider)
                list.AddRange(SliderParams(slider));
            if (hitObject is Spinner spinner)
                list.Add(spinner.EndTime);
            if (hitObject is Hold hold)
            {
                list.Add(hold.EndTime);
                return string.Join(',', list) + ":" + hitSample;
            }

            if (hitSample != null)
                list.Add(hitSample);

            return string.Join(',', list);
        }

        public static List<object> SliderParams(Slider slider)
        {
            var list = new List<object>
            {
                CurveType(slider.CurveType) +
                string.Join(string.Empty, slider.CurvePoints.ConvertAll(pt => $"|{pt.X}:{pt.Y}")),
                slider.Slides,
                slider.Length
            };

            if (slider.EdgeSounds != null)
                list.Add(string.Join('|', slider.EdgeSounds.ConvertAll(s => (int)s)));
            if (slider.EdgeSets != null)
                list.Add(string.Join('|', slider.EdgeSets.ConvertAll(s => $"{(int)s.Item1}:{(int)s.Item2}")));

            return list;
        }

        public static char CurveType(CurveType value)
        {
            return value switch
            {
                Enums.CurveType.Bezier => 'B',
                Enums.CurveType.Catmull => 'C',
                Enums.CurveType.Linear => 'L',
                Enums.CurveType.PerfectCurve => 'P',
                _ => throw new InvalidCastException()
            };
        }

        public static int TypeByte(HitObject hitObject)
        {
            var i = 0;
            if (hitObject is HitCircle)
                i += (int)HitObjectType.Circle;
            if (hitObject is Slider)
                i += (int)HitObjectType.Slider;
            if (hitObject is Spinner)
                i += (int)HitObjectType.Spinner;
            if (hitObject is Hold)
                i += (int)HitObjectType.Hold;
            i += hitObject.NewCombo ? 1 << 2 : 0;
            i += hitObject.ComboColourOffset << 4;
            return i;
        }

        public static string HitSample(HitSample hitSample)
        {
            if (hitSample is null)
                return null;

            var normalSet = (int)hitSample.NormalSet;
            var additionSet = (int)hitSample.AdditionSet;
            var index = hitSample.Index;
            var volume = hitSample.Volume;
            var filename = hitSample.Filename ?? string.Empty;
            return $"{normalSet}:{additionSet}:{index}:{volume}:{filename}";
        }
    }
}
