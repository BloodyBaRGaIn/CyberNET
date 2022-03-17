namespace CyberHacker.BeepClasses
{
    /// <summary>
    /// Contains all the data for <see cref="BeepPlayer"/>
    /// </summary>
    struct BeepData
    {
        internal readonly int frequency;
        internal readonly int duration;
        internal readonly int latency;
        internal readonly int repeats;
        internal BeepData(int frequency = 3000, int duration = 1000, int latency = 1, int repeats = 1)
        {
            if (frequency is < 37 or > 32767)
            {
                throw new BeepException(nameof(frequency));
            }
            if (duration < 1)
            {
                throw new BeepException(nameof(duration));
            }
            if (latency < 1)
            {
                throw new BeepException(nameof(latency));
            }
            if (repeats < 1)
            {
                throw new BeepException(nameof(repeats));
            }
            this.frequency = frequency;
            this.duration = duration;
            this.latency = latency;
            this.repeats = repeats;
        }
    }
}
