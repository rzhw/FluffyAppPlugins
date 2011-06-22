/**
 * FluffyApp Google Chrome plugin
 * Copyright (c) 2011, Richard Z.H. Wang & Sebastian Müller
 * Licensed under New BSD
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Uploadinator.TriggerPlugins.Chrome
{
    [Plugin("Chrome",
        Version = "0.2",
        Description = "Shorterns the URL in the currently active tab.",
        Author = "Richard Z.H. Wang & Sebastian Müller",
        Supports = "0.10.2",
        TriggeredBy = "chrome.exe")]
    public class Plugin : IPlugin
    {
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern int GetWindowTextLength(IntPtr hWnd);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        static extern IntPtr SendMessage(IntPtr hWnd, Int32 Msg,
        IntPtr wParam, StringBuilder lParam);
        private const int WM_GETTEXT = 0x000D;
        private const uint MAX_PATH = 255;

        public PluginResult OnTriggered(PluginTriggerEventArgs e)
        {
            IntPtr ownerHandle = e.Handle;
            IntPtr tabHandle = FindWindowEx(ownerHandle, IntPtr.Zero, "Chrome_WidgetWin_0", null);
            //I don't know why you used the class 'Chrome_AutocompleteEditView' first - it should be 'Chrome_OmniboxView'
            IntPtr omniboxHandle = FindWindowEx(ownerHandle, IntPtr.Zero, "Chrome_OmniboxView", null);

            // The tab window's title gives the page's title - perfect!
            string tabTitle = GetTitleFromHandle(tabHandle);

            // Omnibox contents can be changed at any time, so it's *really* unreliable. But it's the only thing available.
            // Also, it will drop "http://" off URLs if that's the protocol of the URL.

            // As we can't get the URL by GetTitleFromHandle, we need to use the SendMessage way using WM_GETTEXT
            StringBuilder sb = new StringBuilder(256);
            SendMessage(omniboxHandle, WM_GETTEXT, (IntPtr)MAX_PATH, sb);
            string omniboxText = sb.ToString().Trim(new Char[] { ' ', '\0', '\n' });

            if (!Uri.IsWellFormedUriString(omniboxText, UriKind.Absolute))
            {
                omniboxText = "http://" + omniboxText;
                if (!Uri.IsWellFormedUriString(omniboxText, UriKind.Absolute)) return null;
            }
            return PluginResult.FromUrl(omniboxText, tabTitle);
        }

        string GetTitleFromHandle(IntPtr handle)
        {
            int length = GetWindowTextLength(handle);
            StringBuilder sb = new StringBuilder(length + 1);
            GetWindowText(handle, sb, sb.Capacity);
            return sb.ToString();
        }
    }
}
