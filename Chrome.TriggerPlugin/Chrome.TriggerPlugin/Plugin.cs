/**
 * FluffyApp Google Chrome plugin
 * Copyright (c) 2011, 2014 Richard Z.H. Wang
 * Licensed under New BSD
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Automation;

namespace Uploadinator.TriggerPlugins.Chrome
{
    [Plugin("Chrome",
        Version = "1.3",
        Description = "Shorterns the URL in the currently active tab. Requires Chrome 28+",
        Author = "Richard Z.H. Wang",
        Supports = "0.10.2",
        TriggeredBy = "chrome.exe")]
    public class Plugin : IPlugin
    {
        public PluginResult OnTriggered(PluginTriggerEventArgs e)
        {
            var windowElm = AutomationElement.FromHandle(e.Handle);

            var omniboxElm = windowElm.FindFirst(
                TreeScope.Descendants,
                new PropertyCondition(AutomationElement.NameProperty, "Address and search bar"));

            // Omnibox contents can be changed at any time, so it's not the most reliable. But it's the only thing available
            var omniboxValuePattern = (ValuePattern)omniboxElm.GetCurrentPattern(ValuePattern.Pattern);
            string omniboxText = omniboxValuePattern.Current.Value;

            // The Omnibox drops "http://" off URLs if that's the protocol of the URL
            if (!Uri.IsWellFormedUriString(omniboxText, UriKind.Absolute))
            {
                omniboxText = "http://" + omniboxText;
                if (!Uri.IsWellFormedUriString(omniboxText, UriKind.Absolute))
                    return null;
            }

            // The tab window's title gives the page's title
            // Chrome's automation stuff doesn't seem to give any indicators as to which one's active
            // so look for one that's a substring of the window title
            string windowTitle = (string)windowElm.GetCurrentPropertyValue(AutomationElement.NameProperty);

            var tabElms = windowElm.FindAll(
                TreeScope.Descendants,
                new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.TabItem));

            string currentTabTitle = omniboxText;

            foreach (var tabElm in tabElms.Cast<AutomationElement>())
            {
                string tabTitle = (string)tabElm.GetCurrentPropertyValue(AutomationElement.NameProperty);
                if (windowTitle.Contains(tabTitle))
                    currentTabTitle = tabTitle;
            }

            return PluginResult.FromUrl(omniboxText, currentTabTitle);
        }
    }
}
