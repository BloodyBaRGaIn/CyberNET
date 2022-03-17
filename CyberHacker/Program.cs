using AutoIt;
using CyberHacker.BeepClasses;
using CyberHacker.BitmapProcClasses;
using CyberHacker.ExtensionClasses;
using CyberHacker.HookClasses;
using CyberHacker.SolverClasses;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CyberHacker
{
    internal static class Program
    {
        #region Global variables
        private const float AdditionalSequenceBitmapScale = 1.2f;
        private static readonly string[] files = new string[3]
        {
            "matrix",
            "sequence",
            "buffer"
        };
        private static readonly Size wanted = new(1920, 1080);
        private static readonly Rectangle[] BitmapRegions = new Rectangle[3]
        {
            new Rectangle(210, 350, 560, 560),
            new Rectangle(820, 340, 290, 550),
            new Rectangle(850, 180, 880, 70)
            /*
            new Rectangle(Point.Empty, wanted),
            new Rectangle(Point.Empty, wanted),
            new Rectangle(Point.Empty, wanted)
            */
        };
        #endregion

        [STAThread]
        private static int Main()
        {
            if (Process.GetProcessesByName(Constants.FriendlyName).Length > 1)
            {
                _ = MessageBox.Show("Program is already running");
                return 1;
            }
            HookProvider.KillHook();
            BeepPlayer player = null;
            while (true)
            {
                string output_info = string.Empty;
                Bitmap custom = null;
                player = new(player == null ? BeepMelodies.WelcomeBeeps : BeepMelodies.ProceedBeeps);
                player.Play();
                Bitmap[] bitmaps = GetBitmaps();
                player.Stop();
                if (bitmaps == null)
                {
                    (player = new(BeepMelodies.ByeBeeps)).Play();
                    return 0;
                }
                try
                {
                    _ = AutoItX.MouseMove(int.MinValue, int.MinValue);
                    byte buffer_size = 0;
                    string[] Results = new string[3];
                    ParsedBitmap parsed_matrix = null, parsed_sequence = null, parsed_buffer = null;
                    #region Recognition process
                    using (Task<List<TimeSpan>> Recognition_task = Task.Run(() =>
                    {
                        Stopwatch stopwatch = Stopwatch.StartNew();
                        List<TimeSpan> times = new();
                        if (!byte.TryParse(Results[2] = (parsed_buffer = new(ref bitmaps[2], Constants.FriendlyName, files[2])).Result, out buffer_size) || buffer_size == 0)
                        {
                            throw new ArgumentOutOfRangeException("Cannot define buffer size");
                        }
                        times.Add(stopwatch.Elapsed);
                        void ParseWatch(in int idx, ref ParsedBitmap bitmap, in bool is_sq, in float scale = 1f)
                        {
                            stopwatch.Restart();
                            Results[idx] = (bitmap = new(ref bitmaps[idx], Constants.FriendlyName, files[idx], is_sq, scale)).Result;
                            stopwatch.Stop();
                            times.Insert(idx, stopwatch.Elapsed);
                        }
                        ParseWatch(0, ref parsed_matrix, true);
                        ParseWatch(1, ref parsed_sequence, false, AdditionalSequenceBitmapScale);
                        stopwatch.Stop();
                        return times;
                    }))
                    {
                        try
                        {
                            if (Recognition_task.Exception != null)
                            {
                                throw Recognition_task.Exception.InnerException;
                            }
                            if (!Recognition_task.Wait(30000))
                            {
                                throw new TimeoutException("Recognition process time out");
                            }
                            List<TimeSpan> times = Recognition_task.Result;
                            for (int i = 0; i < times.Count; i++)
                            {
                                output_info += times[i].TimeReport(files[i]);
                            }
                            output_info += new TimeSpan(times.Sum(item => item.Ticks)).TimeReport("Total");
                        }
                        catch (AggregateException)
                        {
                            throw;
                        }
                    }
                    GC.Collect();
                    #endregion
                    output_info += PathFinder.Find(Results, ref parsed_matrix,
                                                   out List<(bool, int)> completed);
                    List<Point> Points;
                    #region Bitmap post-processing
                    {
                        BitmapExtensions.Scale(ref bitmaps[1], AdditionalSequenceBitmapScale);
                        Stopwatch stopwatch = Stopwatch.StartNew();
                        BitmapExtensions.GetPoints(parsed_matrix.KeyCoords, BitmapRegions[0].Location, out Points);
                        {
                            Point max_loc = new Point((parsed_sequence.Cells.Max(item => (item.X, item.Y))).StructSize());
                            Rectangle LastRect = parsed_sequence.Cells.LastOrDefault();
                            parsed_sequence.AddCell(new(max_loc + new Size(LastRect.Width * 2, 0),
                                                        LastRect.Size));
                        }
                        GC.Collect();
                        #region Custom bitmap forming
                        custom = CustomBitmap.MakeFinal(new object[]
                        {
                            parsed_matrix.GetFullInfo(),
                            parsed_sequence.GetFullInfo(),
                            parsed_buffer.Cells
                        },
                        (from Bitmap b in bitmaps select b.PixelFormat).ToArray(),
                        new object[]
                        {
                            parsed_matrix.Solution_path,
                            completed,
                            parsed_matrix.KeyText.ToArray()
                        });
                        #endregion
                        stopwatch.Stop();
                        GC.Collect();
                        output_info += stopwatch.Elapsed.TimeReport("Bitmaps", "post-processing");
                    }
                    #endregion
                    DoAuto(Points);
                }
                catch (Exception ex)
                {
                    _ = MessageBox.Show(ex.Message);
                    output_info += $"{ex.Message}{Constants.NL}";
                }
                finally
                {
                    #region Saving results
                    DirectoryInfo dir = Directory.CreateDirectory(@$"{Constants.CurrentDirectory}\LastSolution\");
                    using (StreamWriter writer = new($"{dir.FullName}result.txt", false,
                                                     System.Text.Encoding.Default))
                    {
                        writer.WriteLine(output_info);
                        writer.Close();
                    }
                    custom?.Save($"{dir.FullName}RES.png", ImageFormat.Png);
                    for (int i = 0; i < bitmaps.Length; i++)
                    {
                        bitmaps[i]?.Clear();
                        bitmaps[i]?.Dispose();
                        bitmaps[i] = null;
                    }
                    #endregion
                    GC.Collect();
                }
            }
        }

        /// <summary>
        /// Makes an array of images from the screenshot
        /// </summary>
        /// <returns>An array of images</returns>
        private static Bitmap[] GetBitmaps()
        {
            while (true)
            {
                try
                {
                    int code = HookProvider.GetHook();
                    Bitmap fromclip = code switch
                    {
                        0 => BitmapExtensions.Capture(),
                        1 => throw new ProcessExitException(),
                        2 => Properties.DebugResources.RawInput,
                        _ => throw new ProcessFailureException($"Hook process returned code {code}")
                    };
                    if (fromclip == null)
                    {
                        throw new ArgumentNullException(nameof(fromclip));
                    }
                    if (fromclip.Size != wanted)
                    {
                        fromclip = new(fromclip, wanted);
                    }
                    return (from Rectangle rects in BitmapRegions
                            select fromclip.Clone(rects, fromclip.PixelFormat)).ToArray();
                }
                catch (ProcessExitException)
                {
                    return null;
                }
                catch (Exception e)
                {
                    switch (MessageBox.Show(e.Message,
                                            e is ProcessFailureException ? "Hook process failure" : Constants.FriendlyName,
                                            MessageBoxButtons.RetryCancel, MessageBoxIcon.Stop))
                    {
                        case DialogResult.Retry:
                            break;
                        default:
                            return null;
                    }
                }
            }
        }

        /// <summary>
        /// Performs a LMB clicks by the point collection
        /// </summary>
        /// <param name="Points">The point collection</param>
        /// <param name="delay">The delay between mouse move and mouse click actions</param>
        private static void DoAuto(in IEnumerable<Point> Points, int delay = 1)
        {
            AutoItX.MouseMove(int.MinValue, int.MinValue, 1);
            Thread.Sleep(delay);
            foreach (Point point in Points)
            {
                AutoItX.MouseMove(point.X, point.Y, 1);
                Thread.Sleep(delay);
                AutoItX.MouseClick("LEFT", point.X, point.Y, 10, 0);
                Thread.Sleep(delay);
            }
        }
    }
}
