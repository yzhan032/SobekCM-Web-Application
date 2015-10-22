﻿#region Using directives

using System;
using SobekCM.Engine_Library.Solr;

#endregion

namespace SobekCM.Builder_Library.Modules.Items
{
    /// <summary> Item-level submission package module saves all of the digital resource information to solr/lucene </summary>
    /// <remarks> This class implements the <see cref="abstractSubmissionPackageModule" /> abstract class and implements the <see cref="iSubmissionPackageModule" /> interface. </remarks>
    public class SaveToSolrLuceneModule : abstractSubmissionPackageModule
    {
        /// <summary> Saves all of the digital resource information to solr/lucene </summary>
        /// <param name="Resource"> Incoming digital resource object </param>
        /// <returns> TRUE if processing can continue, FALSE if a critical error occurred which should stop all processing </returns>
        public override bool DoWork(Incoming_Digital_Resource Resource)
        {

            // Save this to the Solr/Lucene database
            if (Settings.Document_Solr_Index_URL.Length > 0)
            {
                try
                {
                    Solr_Controller.Update_Index(Settings.Document_Solr_Index_URL, Settings.Page_Solr_Index_URL, Resource.Metadata, true);
                }
                catch (Exception ee)
                {
                    OnError("Error saving data to the Solr/Lucene index.  The index may not reflect the most recent data in the METS.", Resource.BibID + ":" + Resource.VID, Resource.METS_Type_String, Resource.BuilderLogId);
                    OnError("Solr Error: " + ee.Message, Resource.BibID + ":" + Resource.VID, Resource.METS_Type_String, Resource.BuilderLogId);

                }
            }

            return true;
        }
    }
}
