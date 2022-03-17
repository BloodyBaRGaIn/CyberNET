using System;
using System.Collections.Generic;
using System.Threading;

namespace CyberHacker.BeepClasses
{
    /// <summary>
    /// Provides playing a melodies in a separate thread
    /// </summary>
    internal sealed class BeepPlayer
    {
        /// <summary>
        /// A beep data collection
        /// </summary>
        private readonly IEnumerable<BeepData> beeps = null;
        /// <summary>
        /// A player thread
        /// </summary>
        private Thread Player = null;
        internal BeepPlayer(IEnumerable<BeepData> beeps)
        {
            if (beeps is null)
            {
                throw new ArgumentNullException(nameof(beeps));
            }
            if (!System.Linq.Enumerable.Any(beeps))
            {
                throw new ArgumentException(nameof(beeps));
            }
            this.beeps = beeps;
        }
        /// <summary>
        /// Plays a melody that given on constructor
        /// </summary>
        internal void Play()
        {
            try
            {
                Stop();
                (Player = new Thread(PlayTarget)).Start();
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// Stops the player thread
        /// </summary>
        internal void Stop()
        {
            if (Player is null)
            {
                return;
            }
            try
            {
                Player.Interrupt();
            }
            catch
            {

            }
            finally
            {
                Player = null;
            }
        }
        /// <summary>
        /// The target for the thread
        /// </summary>
        private void PlayTarget()
        {
            try
            {
                foreach (BeepData data in beeps)
                {
                    Beep(data);
                }
            }
            catch (ThreadInterruptedException)
            {
                return;
            }
            catch (Exception e)
            {
                throw new BeepException(e?.Message);
            }
        }
        /// <summary>
        /// Invokes <see cref="Console.Beep"/> by given data
        /// </summary>
        /// <param name="data"></param>
        internal static void Beep(BeepData data)
        {
            try
            {
                for (int i = 0; i < data.repeats; i++)
                {
                    Console.Beep(data.frequency, data.duration);
                    Thread.Sleep(data.latency);
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// The destructor
        /// </summary>
        ~BeepPlayer()
        {
            Stop();
        }
    }
}
