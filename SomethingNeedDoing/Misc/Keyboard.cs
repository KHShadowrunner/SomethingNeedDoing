﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

using Dalamud.Game.ClientState.Keys;
using Dalamud.Logging;

namespace SomethingNeedDoing.Misc;

/// <summary>
/// Simulate pressing keyboard input.
/// </summary>
internal static class Keyboard
{
    private static IntPtr? handle = null;

    /// <summary>
    /// Send a virtual key.
    /// </summary>
    /// <param name="key">Key to send.</param>
    public static void Send(VirtualKey key) => Send(key, null);

    /// <summary>
    /// Hold Down a virtual key.
    /// </summary>
    /// <param name="key">Key to send.</param>
    public static void SendDown(VirtualKey key) => SendDown(key, null);

    /// <summary>
    /// Life Up a virtual key.
    /// </summary>
    /// <param name="key">Key to send.</param>
    public static void SendUp(VirtualKey key) => SendUp(key, null);

    /// <summary>
    /// Send a virtual key with modifiers.
    /// </summary>
    /// <param name="key">Key to send.</param>
    /// <param name="mods">Modifiers to press.</param>
    public static void Send(VirtualKey key, IEnumerable<VirtualKey>? mods)
    {
        const int WM_KEYDOWN = 0x100;
        const int WM_KEYUP = 0x101;

        if (key != 0)
        {
            var hWnd = handle ??= Process.GetCurrentProcess().MainWindowHandle;

            if (mods != null)
            {
                foreach (var mod in mods)
                    _ = SendMessage(hWnd, WM_KEYDOWN, (IntPtr)mod, IntPtr.Zero);
            }
            PluginLog.Debug("Entered The normal route");

            _ = SendMessage(hWnd, WM_KEYDOWN, (IntPtr)key, IntPtr.Zero);
            Thread.Sleep(100);
            _ = SendMessage(hWnd, WM_KEYUP, (IntPtr)key, IntPtr.Zero);

            if (mods != null)
            {
                foreach (var mod in mods)
                    _ = SendMessage(hWnd, WM_KEYUP, (IntPtr)mod, IntPtr.Zero);
            }
        }
    }
    // Experimental

    /// <summary>
    /// Send a virtual key with modifiers for a selected hold time.
    /// </summary>
    /// <param name="key">Key to send.</param>
    /// <param name="mods">Modifiers to press.</param>
    /// <param name="seconds">How long to hold key down.</param>
    public static void Send(VirtualKey key, IEnumerable<VirtualKey>? mods, int seconds)
    {
        const int WM_KEYDOWN = 0x100;
        const int WM_KEYUP = 0x101;

        if (key != 0)
        {
            var hWnd = handle ??= Process.GetCurrentProcess().MainWindowHandle;

            if (mods != null)
            {
                foreach (var mod in mods)
                    _ = SendMessage(hWnd, WM_KEYDOWN, (IntPtr)mod, IntPtr.Zero);
            }
            PluginLog.Debug("Entered The experiment route");
            _ = SendMessage(hWnd, WM_KEYDOWN, (IntPtr)key, IntPtr.Zero);
            Thread.Sleep(100 * seconds);
            _ = SendMessage(hWnd, WM_KEYUP, (IntPtr)key, IntPtr.Zero);

            if (mods != null)
            {
                foreach (var mod in mods)
                    _ = SendMessage(hWnd, WM_KEYUP, (IntPtr)mod, IntPtr.Zero);
            }
        }
    }

    /// <summary>
    /// Hold a virtual key down with modifiers.
    /// </summary>
    /// <param name="key">Key to send.</param>
    /// <param name="mods">Modifiers to press.</param>
    public static void SendDown(VirtualKey key, IEnumerable<VirtualKey>? mods)
    {
        const int WM_KEYDOWN = 0x100;

        if (key != 0)
        {
            var hWnd = handle ??= Process.GetCurrentProcess().MainWindowHandle;

            if (mods != null)
            {
                foreach (var mod in mods)
                    _ = SendMessage(hWnd, WM_KEYDOWN, (IntPtr)mod, IntPtr.Zero);
            }

            PluginLog.Debug("Entered The normal route");

            _ = SendMessage(hWnd, WM_KEYDOWN, (IntPtr)key, IntPtr.Zero);
            Thread.Sleep(100);
        }
    }

    /// <summary>
    /// Lift a virtual key up with modifiers.
    /// </summary>
    /// <param name="key">Key to send.</param>
    /// <param name="mods">Modifiers to press.</param>
    public static void SendUp(VirtualKey key, IEnumerable<VirtualKey>? mods)
    {
        const int WM_KEYUP = 0x101;

        if (key != 0)
        {
            var hWnd = handle ??= Process.GetCurrentProcess().MainWindowHandle;
            _ = SendMessage(hWnd, WM_KEYUP, (IntPtr)key, IntPtr.Zero);

            if (mods != null)
            {
                foreach (var mod in mods)
                    _ = SendMessage(hWnd, WM_KEYUP, (IntPtr)mod, IntPtr.Zero);
            }
        }
    }

    [DllImport("user32.dll")]
    private static extern int SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);
}
