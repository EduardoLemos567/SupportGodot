namespace Support
{
    public struct CallCounter
    {
        private readonly int callRate;
        private int counter;
        public CallCounter(int call_rate, int initial_counter = 0)
        {
            callRate = call_rate;
            counter = initial_counter;
        }
        public bool TryCall() => ++counter > callRate;
        public void Rollback() => counter--;
        public void Reset() => counter = 0;
    }
}