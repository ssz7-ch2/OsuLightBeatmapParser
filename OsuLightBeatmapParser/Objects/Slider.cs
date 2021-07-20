using OsuLightBeatmapParser.Enums;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace OsuLightBeatmapParser.Objects
{
    public class Slider : HitObject
    {
        public CurveType CurveType { get; set; }
        public List<Vector2> CurvePoints { get; set; }
        public int Slides { get; set; }
        public double Length { get; set; }
        public List<HitSoundType> EdgeSounds { get; set; }
        public List<Tuple<SampleSet, SampleSet>> EdgeSets { get; set; }
    }
}
