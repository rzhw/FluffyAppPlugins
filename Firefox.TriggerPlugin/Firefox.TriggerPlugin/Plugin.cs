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
        Supports = "0.10.1",
        TriggeredBy = "firefox.exe")]
    public class TestPlugin : IPlugin
    {
        public PluginResult OnTriggered()
        {
            DdeClient dde = new DdeClient("Firefox", "WWW_GetWindowInfo");
            dde.Connect();
            var parts = dde.Request("URL", 1000).Split(',');
            dde.Disconnect();

            // Result string is in the format "uri", "title", ...
            Uri uri = new Uri(parts[0].Trim('"').Replace("\\\"", "\""));
            string name = parts[1].Trim('"').Replace("\\\"", "\"");

            return new PluginResult(uri, name);
        }
    }
}
