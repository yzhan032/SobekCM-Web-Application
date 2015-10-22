﻿#region Using directives

using System;
using System.IO;
using SobekCM.Builder_Library.Settings;
using SobekCM.Builder_Library.Tools;

#endregion

namespace SobekCM.Builder_Library.Modules.Items
{
    /// <summary> Item-level submission package module extracts a thumbnail image from a PDF file </summary>
    /// <remarks> This class implements the <see cref="abstractSubmissionPackageModule" /> abstract class and implements the <see cref="iSubmissionPackageModule" /> interface. </remarks>
    public class CreatePdfThumbnailModule : abstractSubmissionPackageModule
    {
        /// <summary> Extracts a thumbnail image from a PDF file </summary>
        /// <param name="Resource"> Incoming digital resource object </param>
        /// <returns> TRUE if processing can continue, FALSE if a critical error occurred which should stop all processing </returns>
        public override bool DoWork(Incoming_Digital_Resource Resource)
        {
            string resourceFolder = Resource.Resource_Folder;

            // Get the executable path/file for ghostscript and imagemagick
            string ghostscript_executable = MultiInstance_Builder_Settings.Ghostscript_Executable;
            string imagemagick_executable = MultiInstance_Builder_Settings.ImageMagick_Executable;

            // Preprocess each PDF
            string[] pdfs = Directory.GetFiles(resourceFolder, "*.pdf");
            foreach (string thisPdf in pdfs)
            {
                // Get the fileinfo and the name
                FileInfo thisPdfInfo = new FileInfo(thisPdf);
                string fileName = thisPdfInfo.Name.Replace(thisPdfInfo.Extension, "");

                // Does the thumbnail exist for this item?
                if (( !String.IsNullOrEmpty(ghostscript_executable)) && (!String.IsNullOrEmpty(imagemagick_executable)))
                {
                    if (!File.Exists(resourceFolder + "\\" + fileName + "thm.jpg"))
                    {
                        PDF_Tools.Create_Thumbnail(resourceFolder, thisPdf, resourceFolder + "\\" + fileName + "thm.jpg", ghostscript_executable, imagemagick_executable);
                    }
                }
            }

            return true;
        }
    }
}
