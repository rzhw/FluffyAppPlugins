/**
 * FluffyApp Firefox plugin
 * Copyright (c) 2011, Richard Z.H. Wang
 * Licensed under New BSD
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NDde.Client;

namespace Uploadinator.TriggerPlugins.Firefox
{
    [Plugin("Firefox",
        Version = "0.1",
        Description = "Shorterns the URL in the currently active tab.",
        Author = "Richard Z.H. Wang",
        Supports = "0.10.2",
        TriggeredBy = "firefox.exe")]
    public class Plugin : IPlugin
    {
        public PluginResult OnTriggered(PluginTriggerEventArgs e)
        {
            DdeClient dde = new DdeClient("Firefox", "WWW_GetWindowInfo");
            dde.Connect();
            string result = dde.Request("URL", 1000);
            dde.Disconnect();

            // Result string is in the format "uri", "title", ...
            var parts = result.Trim('"').Split(new string[] { "\",\"" }, StringSplitOptions.None);
            string url = parts[0].Replace("\\\"", "\"");
            string name = parts[1].Replace("\\\"", "\"");

            return PluginResult.FromUrl(url, name);
        }
    }
}
