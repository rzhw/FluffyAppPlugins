/**
 * FluffyApp Internet Explorer plugin
 * Copyright (c) 2012 Richard Z.H. Wang
 * Licensed under New BSD
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NDde.Client;

namespace Uploadinator.TriggerPlugins.InternetExplorer
{
    [Plugin("Internet Explorer",
        Version = "0.1",
        Description = "Shorterns the URL in the currently active tab.",
        Author = "Richard Z.H. Wang",
        Supports = "0.10.2",
        TriggeredBy = "iexplore.exe")]
    public class Plugin : IPlugin
    {
        public PluginResult OnTriggered(PluginTriggerEventArgs e)
        {
            DdeClient dde = new DdeClient("IExplore", "WWW_GetWindowInfo");
            dde.Connect();
            string result = dde.Request("0xFFFFFFFF,sURL,sTitle", 1000);
            dde.Disconnect();

            // Result string is in the format "uri", "title"\0
            var parts = result.Trim('\0').Trim('"').Split(new string[] { "\",\"" }, StringSplitOptions.None);
            string url = parts[0].Replace("\\\"", "\"");
            string name = parts[1].Replace("\\\"", "\"");

            return PluginResult.FromUrl(url, name);
        }
    }
}
