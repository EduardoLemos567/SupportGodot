namespace Support.Diagnostics;

#if DEBUG
public readonly struct TimingProfile
{
    private readonly System.Diagnostics.Stopwatch timing;
    public TimingProfile() => timing = System.Diagnostics.Stopwatch.StartNew();
    public void RestartSector(string sector)
    {
        Debug.Lap(timing, sector);
        timing.Restart();
    }
    public void EndSector(string sector)
    {
        Debug.Lap(timing, sector);
        timing.Stop();
    }
}
#endif