using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MixMatch2.Resources.Helpers
{
    public static class MathHelper
    {
        /// <summary>
        /// Maps a value from one range to another.
        /// </summary>
        /// <param name="oldRange"> The old range</param>
        /// <param name="newRange"> The new range</param>
        /// <param name="value">The value to map</param>
        /// <example>
        /// This show how to map the number 0.7 from the range of 0-1 to the range of 10-20.
        /// <code>
        /// var newValue = MapRange(new Range(0, 1), new Range(10, 20), 0.7); // newValue = 17.
        /// </code>
        /// </example>
        /// <returns>The equivalent value in the new range</returns>
        public static double MapRange(Range oldRange, Range newRange, double value)
        {
            return newRange.Low + (value - oldRange.Low) * newRange.Difference / oldRange.Difference;
        }
    }

    /// <summary>
    /// Provides a range of numbers, as well as a few helper functions.
    /// </summary>
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
