using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace HookNet
{
    internal static class HookNet
    {
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private static IntPtr _hookID = IntPtr.Zero;
        private static List<int> compare;
        private static int Main(string[] args)
        {
            if (args.Length == 0)
            {
                return -1;
            }
            try
            {
                compare = args.Select(item => Convert.ToInt32(item)).ToList();
            }
            catch
            {
                return -1;
            }
            _hookID = SetHook(HookCallback);
            System.Windows.Forms.Application.Run();
            _ = UnhookWindowsHookEx(_hookID);
            return 0;
        }

        private static IntPtr SetHook(LowLevelKeyboardProc proc) =>
            SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                             GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName), 0);

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                int idx = compare.IndexOf(Marshal.ReadInt32(lParam));
                if (idx != -1)
                {
                    Environment.Exit(idx);
                }
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook,
                                                      LowLevelKeyboardProc lpfn,
                                                      IntPtr hMod,
                                                      uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk,
                                                    int nCode,
                                                    IntPtr wParam,
                                                    IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
    }
}