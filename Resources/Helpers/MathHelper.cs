using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MixMatch2.Resources.Helpers
{
    public static class MathHelper
    {
        public static double MapRange(Range oldRange, Range newRange, double value)
        {
            return newRange.Low + (value - oldRange.Low) * newRange.Difference / oldRange.Difference;
        }
    }

    public class Range
    {
        public double Low { get; set; }
        public double High { get; set; }

        public double Difference => High - Low;

        public Range(double low, double high)
        {
            this.Low = low;
            this.High = high;
        }
    }
}
