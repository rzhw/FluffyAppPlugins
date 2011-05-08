﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
//using Photoshop;

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
            string dir = Path.Combine(Path.GetTempPath(), "FluffyApp");
            Directory.CreateDirectory(dir);

            //Application photoshop = new Application();
            //if (photoshop != null)
            //{
            //    Document activeDocument = photoshop.ActiveDocument;
            //    string newFilename = Path.GetFileNameWithoutExtension(activeDocument.Name) + ".png";
            //    string savePath = Path.Combine(dir, newFilename);
            //    activeDocument.SaveAs(savePath, new PNGSaveOptions(), true, PsExtensionType.psLowercase);

            //    return PluginResult.FromPath(savePath);
            //}

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
