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
            string result = dde.Request("URL", 1000);
            dde.Disconnect();

            var parts = result.Split(',');
            Uri uri = new Uri(parts[0].Trim('"'));
            return new PluginResult(uri);
        }
    }
}
