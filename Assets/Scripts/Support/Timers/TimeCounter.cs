namespace Support.Timers
{
    public struct TimeCounter
    {
        private float limitSeconds;
        private readonly System.Diagnostics.Stopwatch stopwatch;
        public TimeCounter(float limitSeconds)
        {
            this.limitSeconds = limitSeconds;
            stopwatch = System.Diagnostics.Stopwatch.StartNew();
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