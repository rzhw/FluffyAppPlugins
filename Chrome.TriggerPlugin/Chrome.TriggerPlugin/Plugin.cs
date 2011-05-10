/**
 * FluffyApp Google Chrome plugin
 * Copyright (c) 2011, Richard Z.H. Wang
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
        Version = "0.1",
        Description = "(Doesn't work) Shorterns the URL in the currently active tab.",
        Author = "Richard Z.H. Wang",
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

        public PluginResult OnTriggered(PluginTriggerEventArgs e)
        {
            IntPtr ownerHandle = e.Handle;
            IntPtr tabHandle = FindWindowEx(ownerHandle, IntPtr.Zero, "Chrome_WidgetWin_0", null);
            IntPtr omniboxHandle = FindWindowEx(ownerHandle, IntPtr.Zero, "Chrome_AutocompleteEditView", null);

            // The tab window's title gives the page's title - perfect!
            string tabTitle = GetTitleFromHandle(tabHandle);

            // Omnibox contents can be changed at any time, so it's *really* unreliable. But it's the only thing available.
            // Also, it will drop "http://" off URLs if that's the protocol of the URL.
            string omniboxText = GetTitleFromHandle(omniboxHandle);
            if (!Uri.IsWellFormedUriString(omniboxText, UriKind.Absolute))
            {
                omniboxText = "http://" + omniboxText;
                if (!Uri.IsWellFormedUriString(omniboxText, UriKind.Absolute))
                    return null;
            }
            throw new Exception(); // PLUGIN CURRENTLY DOESN'T WORK; OMNIBOX WINDOW TEXT CAN'T BE RETRIEVED

            return PluginResult.FromUrl(omniboxText, tabTitle);
        }

        string GetTitleFromHandle(IntPtr handle)
        {
            int length = GetWindowTextLength(handle);
            StringBuilder sb = new StringBuilder(length + 1);
            GetWindowText(handle, sb, sb.Capacity);
            throw new Exception(sb.ToString());
        }
    }
}
