using System.Diagnostics;

namespace Support.Diagnostics;

public readonly struct TimingProfile
{
    private readonly Stopwatch timing;
    private readonly ILogger logger;
    public TimingProfile(ILogger logger)
    {
        timing = Stopwatch.StartNew();
        this.logger = logger;
    }
    public void RestartSector(string sector)
    {
        logger.Lap(timing, sector);
        timing.Restart();
    }
    public void EndSector(string sector)
    {
        logger.Lap(timing, sector);
        timing.Stop();
    }
}