/**
 * FluffyApp Photoshop plugin
 * Copyright (c) 2011-2012 Richard Z.H. Wang
 * Licensed under New BSD
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Uploadinator.TriggerPlugins.Photoshop
{
    [Plugin("Photoshop",
        Version = "0.1.1",
        Description = "Uploads an image of the currently active tab's state.",
        Author = "Richard Z.H. Wang",
        Supports = "1.1.0",
        TriggeredBy = "Photoshop.exe")]
    public class Plugin : IPlugin
    {
        public PluginResult OnTriggered(PluginTriggerEventArgs e)
        {
            string dir = Path.Combine(Path.GetTempPath(), "FluffyApp");
            Directory.CreateDirectory(dir);

            Type photoshopType = Type.GetTypeFromProgID("Photoshop.Application");
            if (photoshopType != null)
            {
                object photoshop = Activator.CreateInstance(photoshopType);

                object activeDocument = photoshopType.InvokeMember("ActiveDocument", BindingFlags.GetProperty, null, photoshop, null);
                string activeFilename = (string)activeDocument.GetType().InvokeMember("Name", BindingFlags.GetProperty, null, activeDocument, null);

                string savePath = Path.Combine(dir, Path.GetFileNameWithoutExtension(activeFilename) + ".png");

                Type saveOptionsType = Type.GetTypeFromProgID("Photoshop.PNGSaveOptions");
                object saveOptions = Activator.CreateInstance(saveOptionsType);

                activeDocument.GetType().InvokeMember("SaveAs", BindingFlags.InvokeMethod, null, activeDocument, new object[]
                {
                    savePath,
                    saveOptions,
                    true,
                    2 // lowercase
                });

                return PluginResult.FromPath(savePath);
            }

            return null;
        }
    }
}
