﻿// HTML5 10/15/2013

#region Using directives

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Web;
using SobekCM.Core.Settings;
using SobekCM.Library.Application_State;
using SobekCM.Library.Database;
using SobekCM.Library.HTML;
using SobekCM.Library.Items;
using SobekCM.Library.MainWriters;
using SobekCM.Library.MemoryMgmt;
using SobekCM.Library.Navigation;
using SobekCM.Library.Settings;
using SobekCM.Core.Users;
using SobekCM.Resource_Object;
using SobekCM.Tools;

#endregion

namespace SobekCM.Library.MySobekViewer
{
    /// <summary> Class allows an authenticated system administrator to delete an item from this digital library  </summary>
    /// <remarks> This class extends the <see cref="abstract_MySobekViewer"/> class.<br /><br />
    /// MySobek Viewers are used for registration and authentication with mySobek, as well as performing any task which requires
    /// authentication, such as online submittal, metadata editing, and system administrative tasks.<br /><br />
    /// During a valid html request, the following steps occur:
    /// <ul>
    /// <li>Application state is built/verified by the <see cref="Application_State.Application_State_Builder"/> </li>
    /// <li>Request is analyzed by the <see cref="Navigation.SobekCM_QueryString_Analyzer"/> and output as a <see cref="Navigation.SobekCM_Navigation_Object"/> </li>
    /// <li>Main writer is created for rendering the output, in his case the <see cref="Html_MainWriter"/> </li>
    /// <li>The HTML writer will create the necessary subwriter.  Since this action requires authentication, an instance of the  <see cref="MySobek_HtmlSubwriter"/> class is created. </li>
    /// <li>The mySobek subwriter creates an instance of this viewer to delete the item requested </li>
    /// </ul></remarks>
    public class Delete_Item_MySobekViewer : abstract_MySobekViewer
    {
        private int errorCode;
		private readonly SobekCM_Item item;


        /// <summary> Constructor for a new instance of the Delete_Item_MySobekViewer class </summary>
        /// <param name="User"> Authenticated user information </param>
        /// <param name="Current_Mode"> Mode / navigation information for the current request</param>
		/// <param name="Current_Item"> Individual digital resource to be deleted by the user </param>
        /// <param name="All_Items_Lookup"> Allows individual items to be retrieved by various methods as <see cref="SobekCM.Library.Application_State.Single_Item"/> objects.</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        public Delete_Item_MySobekViewer(User_Object User,
            SobekCM_Navigation_Object Current_Mode, 
			SobekCM_Item Current_Item,
            Item_Lookup_Object All_Items_Lookup,
            Custom_Tracer Tracer)
            : base(User)
        {
            Tracer.Add_Trace("Delete_Item_MySobekViewer.Constructor", "Delete this item");

            // Save mode and set defaults
            currentMode = Current_Mode;
	        item = Current_Item;
            errorCode = -1;

            // Second, ensure this is a logged on user and system administrator before continuing
            Tracer.Add_Trace("Delete_Item_MySobekViewer.Constructor", "Validate user permissions" );
            if ((User == null)  || ( !User.LoggedOn ))
			{
                Tracer.Add_Trace("Delete_Item_MySobekViewer.Constructor", "User does not have delete permissions", Custom_Trace_Type_Enum.Error );
                errorCode = 1;
            }
            else
            {
	            bool canDelete = false;
				if ((User.Can_Delete_All) || (User.Is_System_Admin))
				{
					canDelete = true;
				}
				else
				{
					// In this case, we actually need to build this!
					try
					{
	//					SobekCM_Item testItem = SobekCM_Item_Factory.Get_Item(Current_Mode.BibID, Current_Mode.VID, null, Tracer);
                        if (User.Can_Edit_This_Item(item.BibID, item.Bib_Info.SobekCM_Type_String, item.Bib_Info.Source.Code, item.Bib_Info.HoldingCode, item.Behaviors.Aggregation_Code_List))
							canDelete = true;
					}
					catch
					{
						canDelete = false;
					}
				}

				if (!canDelete)
				{
					Tracer.Add_Trace("Delete_Item_MySobekViewer.Constructor", "User does not have delete permissions", Custom_Trace_Type_Enum.Error);
					errorCode = 1;
				}
            }

			// Ensure the item is valid
			if (errorCode == -1)
			{
				Tracer.Add_Trace("Delete_Item_MySobekViewer.Constructor", "Validate item exists");
				if (!All_Items_Lookup.Contains_BibID_VID(Current_Mode.BibID, Current_Mode.VID))
				{
					Tracer.Add_Trace("Delete_Item_MySobekViewer.Constructor", "Item indicated is not valid", Custom_Trace_Type_Enum.Error);
					errorCode = 2;
				}
			}
    

             // If this is a postback, handle any events first
            if ((currentMode.isPostBack) && ( errorCode < 0 ))
            {
                Debug.Assert(User != null, "User != null");

                // Pull the standard values
                string save_value = HttpContext.Current.Request.Form["admin_delete_item"];
                string text_value = HttpContext.Current.Request.Form["admin_delete_confirm"];

                // Better say "DELETE", or just send back to the item
                if (( save_value == null ) || ( save_value.ToUpper() != "DELETE" ) || ( text_value.ToUpper() != "DELETE"))
                {
                    HttpContext.Current.Response.Redirect(Current_Mode.Base_URL + currentMode.BibID + "/" + currentMode.VID, false);
                    HttpContext.Current.ApplicationInstance.CompleteRequest();
                    currentMode.Request_Completed = true;
                }
                else
                {
					if (currentMode.BibID.ToUpper() == "TEMP000001")
					{
						for (int deleteVID = 2124; deleteVID <= 2134; deleteVID++)
						{
							currentMode.VID = deleteVID.ToString().PadLeft(5, '0');
							Delete_Item(User, All_Items_Lookup, Tracer);
						}
					}
					else
					{
						Delete_Item(User, All_Items_Lookup, Tracer);
					}

                }
            }
        }

		private void Delete_Item(User_Object User, Item_Lookup_Object All_Items_Lookup, Custom_Tracer Tracer)
		{
			errorCode = 0;

			// Get the current item details
			string vid_location = item.Source_Directory;
			string bib_location = (new DirectoryInfo(vid_location)).Parent.FullName;
			//if (errorCode == -1)
			//{
			//	// Get item details
			//	DataSet itemDetails = SobekCM_Database.Get_Item_Details(currentMode.BibID, currentMode.VID, Tracer);

			//	// If the itemdetails was null, this item is somehow invalid item then
			//	if (itemDetails == null)
			//	{
			//		Tracer.Add_Trace("Delete_Item_MySobekViewer.Constructor", "Item indicated is not valid", Custom_Trace_Type_Enum.Error);
			//		errorCode = 2;
			//	}
			//	else
			//	{
			//		// Get the location for this METS file from the returned value
			//		DataRow mainItemRow = itemDetails.Tables[2].Rows[0];
			//		bib_location = InstanceWide_Settings_Singleton.Settings.Image_Server_Network + mainItemRow["File_Location"].ToString().Replace("/", "\\");
			//		vid_location = bib_location + "\\" + currentMode.VID;
			//	}
			//}     

			// Perform the database delete
			Tracer.Add_Trace("Delete_Item_MySobekViewer.Constructor", "Perform database update");
			bool database_result2 = SobekCM_Database.Delete_SobekCM_Item(currentMode.BibID, currentMode.VID, User.Is_System_Admin, String.Empty);

			// Perform the SOLR delete
			Tracer.Add_Trace("Delete_Item_MySobekViewer.Constructor", "Perform solr delete");
			Solr.Solr_Controller.Delete_Resource_From_Index(InstanceWide_Settings_Singleton.Settings.Document_Solr_Index_URL, InstanceWide_Settings_Singleton.Settings.Page_Solr_Index_URL, currentMode.BibID, currentMode.VID);

			if (!database_result2)
			{
				Tracer.Add_Trace("Delete_Item_MySobekViewer.Constructor", "Error performing delete in the database", Custom_Trace_Type_Enum.Error);
				errorCode = 3;
			}
			else
			{
				// Move the folder to deletes
				try
				{
					Tracer.Add_Trace("Delete_Item_MySobekViewer.Constructor", "Move resource files to RECYCLE BIN folder");

					// Make sure upper RECYCLE BIN folder exists, or create it
					string delete_folder = InstanceWide_Settings_Singleton.Settings.Image_Server_Network + "RECYCLE BIN";
					if (!Directory.Exists(delete_folder))
						Directory.CreateDirectory(delete_folder);

					// Create the bib level folder next
					string bib_folder = InstanceWide_Settings_Singleton.Settings.Image_Server_Network + "RECYCLE BIN\\" + currentMode.BibID;
					if (!Directory.Exists(bib_folder))
						Directory.CreateDirectory(bib_folder);

					// Ensure the VID folder does not exist
					string vid_folder = InstanceWide_Settings_Singleton.Settings.Image_Server_Network + "RECYCLE BIN\\" + currentMode.BibID + "\\" + currentMode.VID;
					if (Directory.Exists(vid_folder))
						Directory.Move(vid_folder, vid_folder + "_OLD");

					// Move the VID folder over now
					Directory.Move(vid_location, vid_folder);

					// Check if this was the last VID under this BIB
					if (Directory.GetDirectories(bib_location).Length == 0)
					{
						// Move all files over to the bib folder then
						string[] bib_files = Directory.GetFiles(bib_location);
						foreach (string thisFile in bib_files)
						{
							string fileName = (new FileInfo(thisFile)).Name;
							string new_file = bib_folder + "\\" + fileName;
							File.Move(thisFile, new_file);
						}
					}
				}
				catch (Exception ee)
				{
					Tracer.Add_Trace("Delete_Item_MySobekViewer.Constructor", "Error moving the folder and files to the RECYCLE BIN folder", Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("Delete_Item_MySobekViewer.Constructor", ee.Message, Custom_Trace_Type_Enum.Error);
					Tracer.Add_Trace("Delete_Item_MySobekViewer.Constructor", ee.StackTrace, Custom_Trace_Type_Enum.Error);
					errorCode = 4;
				}

				// Remove from the item list
				All_Items_Lookup.Remove_Item(currentMode.BibID, currentMode.VID);

				// Also remove from the cache
				Cached_Data_Manager.Remove_Digital_Resource_Object(currentMode.BibID, currentMode.VID, Tracer);
			}
		}

		/// <summary> Property indicates the standard navigation to be included at the top of the page by the
		/// main MySobek html subwriter. </summary>
		/// <value> This returns none since this viewer writes all the necessary navigational elements </value>
		/// <remarks> This is set to NONE if the viewer will write its own navigation and ADMIN if the standard
		/// administrative tabs should be included as well.  </remarks>
		public override MySobek_Included_Navigation_Enum Standard_Navigation_Type
		{
			get
			{
				return MySobek_Included_Navigation_Enum.NONE;
			}
		}

        /// <summary> Title for the page that displays this viewer, this is shown in the search box at the top of the page, just below the banner </summary>
        /// <value> This always returns the value 'Delete Item' </value>
        public override string Web_Title
        {
            get
            {
                return "Delete Item";
            }
        }

        /// <summary> Write the text for this delete request directly into the main form </summary>
        /// <param name="Output"> Textwriter to write the pop-up form HTML for this viewer </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This text will appear within the ItemNavForm form tags </remarks>
        public override void Write_ItemNavForm_Closing(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Delete_Item_MySobekViewer.Write_ItemNavForm_Closing", String.Empty);

            if (errorCode == -1)
            {
                // Add the hidden field
                Output.WriteLine("<!-- Hidden field is used for postbacks to indicate what to save and reset -->");
                Output.WriteLine("<input type=\"hidden\" id=\"admin_delete_item\" name=\"admin_delete_item\" value=\"\" />");
				Output.WriteLine();

				// Write the top item mimic html portion
				Write_Item_Type_Top(Output, item);

				Output.WriteLine("<div id=\"container-inner\">");
				Output.WriteLine("<div id=\"pagecontainer\">");

				Output.WriteLine("<div class=\"sbkMySobek_HomeText\" >");
                Output.WriteLine("  <br /><br />");
                Output.WriteLine("  <p>Enter DELETE in the textbox below and select GO to complete this deletion.</p>");
				Output.WriteLine("  <div id=\"sbkDimv_VerifyDiv\">");
				Output.WriteLine("    <input class=\"sbkDimv_input sbkMySobek_Focusable\" name=\"admin_delete_confirm\" id=\"admin_delete_confirm\" type=\"text\" value=\"\" /> &nbsp; &nbsp; ");
				Output.WriteLine("    <button title=\"Confirm delete of this item\" class=\"sbkMySobek_RoundButton\" onclick=\"delete_item(); return false;\">CONFIRM <img src=\"" + currentMode.Base_URL + "default/images/button_next_arrow.png\" class=\"sbkMySobek_RoundButton_RightImg\" alt=\"\" /></button>");
                Output.WriteLine("  </div>");
                Output.WriteLine("</div>");
				Output.WriteLine();
				Output.WriteLine("</div>");
				Output.WriteLine("</div>");
				Output.WriteLine();
				Output.WriteLine("<!-- Focus on confirm box -->");
				Output.WriteLine("<script type=\"text/javascript\">focus_element('admin_delete_confirm');</script>");
				Output.WriteLine();
            }
        }

        /// <summary> Add the HTML to be displayed in the main SobekCM viewer area </summary>
        /// <param name="Output"> Textwriter to write the HTML for this viewer</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        public override void Write_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Delete_Item_MySobekViewer.Write_HTML", String.Empty);

            if (errorCode >= 0)
            {
				// Write the top item mimic html portion
				Write_Item_Type_Top(Output, item);

				Output.WriteLine("<div id=\"container-inner\">");
				Output.WriteLine("<div id=\"pagecontainer\">");

				Output.WriteLine("<div class=\"sbkMySobek_HomeText\" >");
                Output.WriteLine("  <br /><br />");
                Output.WriteLine("  <p>");

                switch (errorCode)
                {
                    case 0:
						Output.WriteLine("    <div class=\"sbkDimv_SuccessMsg\">DELETE SUCCESSFUL</div>");
                        break;

                    case 1:
						Output.WriteLine("    <div class=\"sbkDimv_ErrorMsg\">DELETE FAILED<br /><br />Insufficient user permissions to perform delete</div>");
                        break;

                    case 2:
						Output.WriteLine("    <div class=\"sbkDimv_ErrorMsg\">DELETE FAILED<br /><br />Item indicated does not exists</div>");
                        break;

                    case 3:
						Output.WriteLine("    <div class=\"sbkDimv_ErrorMsg\">DELETE FAILED<br /><br />Error while performing delete in database</div>");
                        break;

                    case 4:
						Output.WriteLine("    <div class=\"sbkDimv_ErrorMsg\">DELETE PARTIALLY SUCCESSFUL<br /><br />Unable to move all files to the RECYCLE BIN folder</div>");
                        break;
                }

                Output.WriteLine("  </p>");
                Output.WriteLine("</div>");
                Output.WriteLine("<br /><br />");
				Output.WriteLine("</div>");
				Output.WriteLine("</div>");
            }
        }

		/// <summary> Gets the collection of special behaviors which this admin or mySobek viewer
		/// requests from the main HTML subwriter. </summary>
		/// <value> This tells the HTML and mySobek writers to mimic the item viewer </value>
		public override List<HtmlSubwriter_Behaviors_Enum> Viewer_Behaviors
		{
			get
			{
				return new List<HtmlSubwriter_Behaviors_Enum>
				{
					HtmlSubwriter_Behaviors_Enum.MySobek_Subwriter_Mimic_Item_Subwriter,
					HtmlSubwriter_Behaviors_Enum.Suppress_Banner
				};
			}
		}
    }
}

