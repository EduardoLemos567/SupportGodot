using System.Diagnostics;

namespace Support.Timers
{
    public struct TimeCounter
    {
        private float limitSeconds;
        private readonly Stopwatch stopwatch;
        public TimeCounter(float limitSeconds)
        {
            this.limitSeconds = limitSeconds;
            stopwatch = Stopwatch.StartNew();
        }
        public readonly bool IsTime => stopwatch.ElapsedMilliseconds * 0.001f >= limitSeconds;

        public readonly void Reset() => stopwatch.Restart();

        public void Reset(float limitSeconds)
        {
            this.limitSeconds = limitSeconds;
            Reset();
        }
    }
}