﻿#region Using directives

using System;
using System.Collections.Generic;
using System.IO;

#endregion

namespace SobekCM.Builder_Library.Modules.Folders
{
    /// <summary> Folder-level builder module checks for appropriately aged folders and moves them into the related processing folder </summary>
    /// <remarks> This class implements the <see cref="abstractFolderModule" /> abstract class and implements the <see cref="iFolderModule" /> interface. </remarks>
    public class MoveAgedPackagesToProcessModule : abstractFolderModule
    {
        /// <summary> Check for appropriately aged folders and moves them into the related processing folder </summary>
        /// <param name="BuilderFolder"> Builder folder upon which to perform all work </param>
        /// <param name="IncomingPackages"> List of valid incoming packages, which may be modified by this process </param>
        /// <param name="Deletes"> List of valid deletes, which may be modifyed by this process </param>
        public override void DoWork(Actionable_Builder_Source_Folder BuilderFolder, List<Incoming_Digital_Resource> IncomingPackages, List<Incoming_Digital_Resource> Deletes)
        {

            try
            {
                // Move all eligible packages from the FTP folders to the processing folders
                if (Settings.Builder_Verbose_Flag)
                    OnProcess("Worker_BulkLoader.Move_Appropriate_Inbound_Packages_To_Processing: Checking incoming folder " + BuilderFolder.Inbound_Folder, String.Empty, String.Empty, String.Empty, -1);

                if (BuilderFolder.Items_Exist_In_Inbound)
                {
                    if (Settings.Builder_Verbose_Flag)
                        OnProcess("Worker_BulkLoader.Move_Appropriate_Inbound_Packages_To_Processing: Found either files or subdirectories in " + BuilderFolder.Inbound_Folder, String.Empty, String.Empty, String.Empty, -1);

                    if (Settings.Builder_Verbose_Flag)
                        OnProcess("Checking inbound packages for aging and possibly moving to processing", String.Empty, String.Empty, String.Empty, -1);

                    String outMessage;
                    if (!BuilderFolder.Move_From_Inbound_To_Processing(out outMessage))
                    {
                        if (outMessage.Length > 0) OnError(outMessage, String.Empty, String.Empty, -1);
                        OnError("Unspecified error moving files from inbound to processing", String.Empty, String.Empty, -1);
                    }
                    else
                    {
                        if ((Settings.Builder_Verbose_Flag) && (outMessage.Length > 0))
                            OnProcess(outMessage, String.Empty, String.Empty, String.Empty, -1);
                    }

                    // Try to get rid of any empty folders
                    try
                    {
                        string[] subdirs = Directory.GetDirectories(BuilderFolder.Inbound_Folder);
                        foreach (string thisSubdir in subdirs)
                        {
                            if ((Directory.GetFiles(thisSubdir).Length == 0) && (Directory.GetDirectories(thisSubdir).Length == 0))
                            {
                                Directory.Delete(thisSubdir);
                            }
                        }
                    }
                    catch {  }

                }
                else if (Settings.Builder_Verbose_Flag)
                    OnProcess("Worker_BulkLoader.Move_Appropriate_Inbound_Packages_To_Processing: No subdirectories or files found in incoming folder " + BuilderFolder.Inbound_Folder, String.Empty, String.Empty, String.Empty, -1);
            }
            catch (Exception ee)
            {
                OnError("Error in harvesting packages from inbound folders to processing\n" + ee.Message, String.Empty, String.Empty, -1);
            }
        }
    }
}
