using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Uploadinator.TriggerPlugins.Photoshop
{
    [Plugin("Photoshop",
        Version = "0.1",
        Description = "Uploads an image of the currently active tab's state.",
        Author = "Richard Z.H. Wang",
        Supports = "0.10.1",
        TriggeredBy = "photoshop.exe")]
    public class Plugin : IPlugin
    {
        public PluginResult OnTriggered()
        {
            // This doesn't work, oh well

            Type photoshopType = Type.GetTypeFromProgID("Photoshop.Application");

            if (photoshopType != null)
            {
                object photoshop = Activator.CreateInstance(photoshopType);

                object activeDocument = photoshopType.InvokeMember("ActiveDocument", BindingFlags.InvokeMethod, null, photoshop, null);
                string activeFilename = (string)activeDocument.GetType().InvokeMember("Name", BindingFlags.InvokeMethod, null, activeDocument, null);

                string savePath = Path.Combine(
                    Path.Combine(Path.GetTempPath(), "FluffyApp"),
                    Path.GetFileNameWithoutExtension(activeFilename) + ".png");
                
                Type saveOptionsType = Type.GetTypeFromProgID("Photoshop.PNGSaveOptions");
                object saveOptions = Activator.CreateInstance(saveOptionsType);

                photoshopType.InvokeMember("SaveAs", BindingFlags.InvokeMethod, null, photoshop, new object[]
                {
                    savePath,
                    saveOptions,
                    true,
                    3 // lowercase
                });

                return PluginResult.FromPath(savePath);
            }
            return null;
        }
    }
}
