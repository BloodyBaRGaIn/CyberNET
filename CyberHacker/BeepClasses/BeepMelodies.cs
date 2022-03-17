namespace CyberHacker.BeepClasses
{
    /// <summary>
    /// Melodies presets
    /// </summary>
    internal static class BeepMelodies
    {
        internal static readonly BeepData[] WelcomeBeeps = new[]
        {
            new BeepData(frequency: 3400, duration: 100, latency: 70, repeats: 4),

            new BeepData(frequency: 3000, duration: 200, latency: 100, repeats: 2),

            new BeepData(frequency: 2700, duration: 200, latency: 100, repeats: 2),

            new BeepData(frequency: 3000, duration: 300)
        };

        internal static readonly BeepData[] ProceedBeeps = new[]
        {
            new BeepData(frequency: 3400, duration: 200, latency: 100, repeats: 2),

            new BeepData(frequency: 3000, duration: 200)
        };

        internal static readonly BeepData[] ByeBeeps = new[]
        {
            new BeepData(frequency: 3000, duration: 100),

            new BeepData(frequency: 2600, duration: 100)
        };
    }
}
