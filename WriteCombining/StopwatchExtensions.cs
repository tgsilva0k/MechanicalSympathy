using System.Diagnostics;

namespace WriteCombining
{
    public static class StopwatchExtensions
    {
        private const double NanosecondsPerSecond = 1E9;
        private static readonly double NanoSecondTickFrequency = NanosecondsPerSecond / Stopwatch.Frequency;
        
        public static long ElapsedNanoSeconds(this Stopwatch watch)
        {
            return (long)(watch.ElapsedTicks * NanoSecondTickFrequency);
        }
    }
}
