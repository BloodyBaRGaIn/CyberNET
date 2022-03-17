using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace CyberHacker.HookClasses
{
    /// <summary>
    /// Provides a wrap around a global hook process
    /// </summary>
    internal static class HookProvider
    {
        /// <summary>
        /// A default name of the executable
        /// </summary>
        private const string HookName = "HookNet";
        /// <summary>
        /// A hot keys array
        /// </summary>
        private static readonly string[] HookArgs = new Keys[3]
        {
            //Main key
            Keys.PrintScreen,
            //Exit key
            Keys.End,
            //Debug key (takes resource screenshot as program input)
            Keys.NumPad0
        }.Select(item => ((int)item).ToString()).ToArray();
        /// <summary>
        /// Starts a global keyboard hook process with <see cref="HookArgs"/> as arguments
        /// </summary>
        /// <param name="processname">A name of the executable to start (default: <see cref="HookName"/>)</param>
        /// <param name="target_extension">An extension of the executable (default: <c>exe</c>)</param>
        /// <returns>An index of hot key hooked</returns>
        internal static int GetHook(string processname = HookName, string target_extension = "exe")
        {
            string path = $"{processname}.{target_extension}";
            if (!File.Exists(path))
            {
                throw new FileNotFoundException(@$"Cannot find file ""{path}""");
            }
            try
            {
                File.Copy(Constants.FriendlyName.ConfigPath(), processname.ConfigPath(), overwrite: true);
                using Process process = Process.Start(path, HookArgs);
                process.WaitForExit();
                return process.ExitCode;
            }
            catch (Exception e)
            {
                throw new ProcessFailureException(e.Message);
            }
        }
        /// <summary>
        /// Gets the runtime config file name from the name of the executable file
        /// </summary>
        /// <param name="filename">Source file name without extension</param>
        /// <returns>A runtime config file name</returns>
        private static string ConfigPath(this string filename)
        {
            if (string.IsNullOrWhiteSpace(filename))
            {
                throw new ArgumentNullException(nameof(filename));
            }
            else
            {
                return $"{filename}.runtimeconfig.json";
            }
        }
        /// <summary>
        /// Kills all processes by given name
        /// </summary>
        /// <param name="processname">A process name</param>
        internal static void KillHook(string processname = HookName)
        {
            foreach (Process p in Process.GetProcessesByName(processname))
            {
                p?.Kill();
                p?.Dispose();
            }
        }
    }
}
