﻿#region Using directives

using System;
using System.Diagnostics;
using System.IO;
using SobekCM.Library.Settings;

#endregion

namespace SobekCM.Builder_Library.Modules.Items
{
    public class OcrTiffsModule : abstractSubmissionPackageModule
    {
        public override void DoWork(Incoming_Digital_Resource Resource)
        {
            string resourceFolder = Resource.Resource_Folder;

            // Run OCR for any TIFF files that do not have any corresponding TXT files
            if (InstanceWide_Settings_Singleton.Settings.OCR_Command_Prompt.Length > 0)
            {
                string[] ocr_tiff_files = Directory.GetFiles(resourceFolder, "*.tif");
                foreach (string thisTiffFile in ocr_tiff_files)
                {
                    FileInfo thisTiffFileInfo = new FileInfo(thisTiffFile);
                    string text_file = resourceFolder + "\\" + thisTiffFileInfo.Name.Replace(thisTiffFileInfo.Extension, "") + ".txt";
                    if (!File.Exists(text_file))
                    {
                        try
                        {
                            string command = String.Format(InstanceWide_Settings_Singleton.Settings.OCR_Command_Prompt, thisTiffFile, text_file);
                            Process ocrProcess = new Process { StartInfo = { FileName = command } };
                            ocrProcess.Start();
                            ocrProcess.WaitForExit();
                        }
                        catch
                        {
                            OnError("Error launching OCR on (" + thisTiffFileInfo.Name + ")", Resource.BibID + ":" + Resource.VID, Resource.METS_Type_String, Resource.BuilderLogId);
                        }
                    }
                }
            }
        }
    }
}
