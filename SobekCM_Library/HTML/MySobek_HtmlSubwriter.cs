﻿#region Using directives

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web;
using System.Web.UI.WebControls;
using SobekCM.Core.Items;
using SobekCM.Core.MemoryMgmt;
using SobekCM.Core.Navigation;
using SobekCM.Engine_Library.Database;
using SobekCM.Library.MainWriters;
using SobekCM.Library.MySobekViewer;
using SobekCM.Library.Settings;
using SobekCM.Tools;

#endregion

namespace SobekCM.Library.HTML
{
    /// <summary> My Sobek html subwriter is used for registration and authentication with mySobek, as well as performing any task which requires
    /// authentication, such as online submittal, metadata editing, and system administrative tasks </summary>
    /// <remarks> This class extends the <see cref="abstractHtmlSubwriter"/> abstract class. <br /><br />
    /// During a valid html request, the following steps occur:
    /// <ul>
    /// <li>Application state is built/verified by the Application_State_Builder </li>
    /// <li>Request is analyzed by the QueryString_Analyzer and output as a <see cref="Navigation_Object"/>  </li>
    /// <li>Main writer is created for rendering the output, in his case the <see cref="Html_MainWriter"/> </li>
    /// <li>The HTML writer will create this necessary subwriter since this action requires authentication. </li>
    /// <li>This class will create a mySobek subwriter (extending <see cref="MySobekViewer.abstract_MySobekViewer"/> ) for the specified task.The mySobek subwriter creates an instance of this viewer to view and edit existing item aggregationPermissions in this digital library</li>
    /// </ul></remarks>
    public class MySobek_HtmlSubwriter : abstractHtmlSubwriter
    {
        private readonly abstract_MySobekViewer mySobekViewer;
 
        #region Constructor, which also creates the applicable MySobekViewer object

	    /// <summary> Constructor for a new instance of the MySobek_HtmlSubwriter class </summary>
        /// <param name="RequestSpecificValues"> All the necessary, non-global data specific to the current request </param>
        public MySobek_HtmlSubwriter(RequestCache RequestSpecificValues) : base(RequestSpecificValues) 
        {

            RequestSpecificValues.Tracer.Add_Trace("MySobek_HtmlSubwriter.Constructor", "Saving values and geting user object back from the session");



            if (RequestSpecificValues.Current_Mode.My_Sobek_Type == My_Sobek_Type_Enum.Log_Out)
            {
                RequestSpecificValues.Tracer.Add_Trace("MySobek_HtmlSubwriter.Constructor", "Performing logout");

                HttpContext.Current.Session["user"] = null;
                HttpContext.Current.Response.Redirect("?", false);
                HttpContext.Current.ApplicationInstance.CompleteRequest();
                RequestSpecificValues.Current_Mode.Request_Completed = true;
                return;
            }

            if ((RequestSpecificValues.Current_Mode.My_Sobek_Type != My_Sobek_Type_Enum.Logon) && (RequestSpecificValues.Current_User != null) && (RequestSpecificValues.Current_User.Is_Temporary_Password))
            {
                RequestSpecificValues.Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.New_Password;
            }

            if (RequestSpecificValues.Current_Mode.Logon_Required)
                RequestSpecificValues.Current_Mode.My_Sobek_Type = My_Sobek_Type_Enum.Logon;

            RequestSpecificValues.Tracer.Add_Trace("MySobek_HtmlSubwriter.Constructor", "Building the my sobek viewer object");
            switch (RequestSpecificValues.Current_Mode.My_Sobek_Type)
            {
                case My_Sobek_Type_Enum.Home:
                    mySobekViewer = new Home_MySobekViewer(RequestSpecificValues);
                    break;

                case My_Sobek_Type_Enum.New_Item:
                    mySobekViewer = new New_Group_And_Item_MySobekViewer(RequestSpecificValues);
                    break;

                case My_Sobek_Type_Enum.Folder_Management:
                    mySobekViewer = new Folder_Mgmt_MySobekViewer(RequestSpecificValues);
                    break;

                case My_Sobek_Type_Enum.Saved_Searches:
                    mySobekViewer = new Saved_Searches_MySobekViewer(RequestSpecificValues);
                    break;

                case My_Sobek_Type_Enum.Preferences:
                    mySobekViewer = new Preferences_MySobekViewer(RequestSpecificValues);
                    break;

                case My_Sobek_Type_Enum.Logon:
                    mySobekViewer = new Logon_MySobekViewer(RequestSpecificValues);
                    break;

                case My_Sobek_Type_Enum.New_Password:
                    mySobekViewer = new NewPassword_MySobekViewer(RequestSpecificValues);
                    break;

                case My_Sobek_Type_Enum.Delete_Item:
                    mySobekViewer = new Delete_Item_MySobekViewer(RequestSpecificValues);
                    break;

                case My_Sobek_Type_Enum.Edit_Item_Behaviors:
                    mySobekViewer = new Edit_Item_Behaviors_MySobekViewer(RequestSpecificValues);
                    break;

                case My_Sobek_Type_Enum.Edit_Item_Metadata:
                    mySobekViewer = new Edit_Item_Metadata_MySobekViewer( RequestSpecificValues.Current_Item, RequestSpecificValues);
                    break;

                case My_Sobek_Type_Enum.Edit_Item_Permissions:
                    mySobekViewer = new Edit_Item_Permissions_MySobekViewer(RequestSpecificValues);
                    break;

                case My_Sobek_Type_Enum.File_Management:
                    mySobekViewer = new File_Management_MySobekViewer(RequestSpecificValues);
                    break;

                case My_Sobek_Type_Enum.Edit_Group_Behaviors:
                    mySobekViewer = new Edit_Group_Behaviors_MySobekViewer(RequestSpecificValues);
                    break;



                case My_Sobek_Type_Enum.Edit_Group_Serial_Hierarchy:
                    mySobekViewer = new Edit_Serial_Hierarchy_MySobekViewer(RequestSpecificValues);
                    break;

                case My_Sobek_Type_Enum.Item_Tracking:
                    mySobekViewer = new Track_Item_MySobekViewer(RequestSpecificValues);
                    break;

                case My_Sobek_Type_Enum.Group_Add_Volume:
                    // Pull the list of items tied to this group
                    SobekCM_Items_In_Title itemsInTitle = CachedDataManager.Retrieve_Items_In_Title(RequestSpecificValues.Current_Item.BibID, RequestSpecificValues.Tracer);
                    if (itemsInTitle == null)
                    {
                        // Get list of information about this item group and save the item list
                        DataSet itemDetails = Engine_Database.Get_Item_Group_Details(RequestSpecificValues.Current_Item.BibID, RequestSpecificValues.Tracer);
                        itemsInTitle = new SobekCM_Items_In_Title(itemDetails.Tables[1]);

                        // Store in cache if retrieved
                        CachedDataManager.Store_Items_In_Title(RequestSpecificValues.Current_Item.BibID, itemsInTitle, RequestSpecificValues.Tracer);
                    }
                    mySobekViewer = new Group_Add_Volume_MySobekViewer(RequestSpecificValues, itemsInTitle );
                    break;

                case My_Sobek_Type_Enum.Group_AutoFill_Volumes:
                    mySobekViewer = new Group_AutoFill_Volume_MySobekViewer(RequestSpecificValues);
                    break;

                case My_Sobek_Type_Enum.Group_Mass_Update_Items:
                    mySobekViewer = new Mass_Update_Items_MySobekViewer(RequestSpecificValues);
                    break;

                case My_Sobek_Type_Enum.Page_Images_Management:
                    mySobekViewer = new Page_Image_Upload_MySobekViewer(RequestSpecificValues);
                    break;

                case My_Sobek_Type_Enum.User_Tags:
                    mySobekViewer = new User_Tags_MySobekViewer(RequestSpecificValues);
                    break;

                case My_Sobek_Type_Enum.User_Usage_Stats:
                    mySobekViewer = new User_Usage_Stats_MySobekViewer(RequestSpecificValues);
                    break;
            }
        }

        #endregion

        /// <summary> Gets the collection of special behaviors which this subwriter
        /// requests from the main HTML subwriter. </summary>
        /// <remarks> By default, this returns an empty list </remarks>
        public override List<HtmlSubwriter_Behaviors_Enum> Subwriter_Behaviors
        {
            get {
	            return mySobekViewer != null ? mySobekViewer.Viewer_Behaviors : emptybehaviors;
            }
        }

        /// <summary> Property indicates if the current mySobek viewer can contain pop-up forms</summary>
        /// <remarks> If the mySobek viewer contains pop-up forms the overall page renders differently, 
        /// allowing for the blanket division and the popup forms near the top of the rendered HTML </remarks>
        public bool Contains_Popup_Forms
        {
            get
            {
                return mySobekViewer.Contains_Popup_Forms;
            }
        }

		/// <summary> Returns a flag indicating whether the file upload specific holder in the itemNavForm form will be utilized 
		/// for the current request, or if it can be hidden. </summary>
		public override bool Upload_File_Possible
		{
			get
			{
				if (RequestSpecificValues.Current_User == null)
					return false;

				if (RequestSpecificValues.Current_Mode.My_Sobek_Type == My_Sobek_Type_Enum.File_Management)
					return false;

				if (((RequestSpecificValues.Current_Mode.My_Sobek_Type == My_Sobek_Type_Enum.New_Item) && (RequestSpecificValues.Current_Mode.My_Sobek_SubMode.Length > 0) && (RequestSpecificValues.Current_Mode.My_Sobek_SubMode[0] == '8')) ||
				    (RequestSpecificValues.Current_Mode.My_Sobek_Type == My_Sobek_Type_Enum.File_Management) || (RequestSpecificValues.Current_Mode.My_Sobek_Type == My_Sobek_Type_Enum.Page_Images_Management))
					return true;

				return false;
			}
		}


		/// <summary> Write any additional values within the HTML Head of the
		/// final served page </summary>
		/// <param name="Output"> Output stream currently within the HTML head tags </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
		public override void Write_Within_HTML_Head(TextWriter Output, Custom_Tracer Tracer)
		{
			Output.WriteLine("  <meta name=\"robots\" content=\"index, nofollow\" />");
			Output.WriteLine("  <link href=\"" + Static_Resources.Sobekcm_Metadata_Css + "\" rel=\"stylesheet\" type=\"text/css\" />");
            Output.WriteLine("  <link href=\"" + Static_Resources.Sobekcm_Mysobek_Css + "\" rel=\"stylesheet\" type=\"text/css\" title=\"standard\" />");



			// If we are currently uploading files, add those specific upload styles 
			if (((RequestSpecificValues.Current_Mode.My_Sobek_Type == My_Sobek_Type_Enum.New_Item) && (RequestSpecificValues.Current_Mode.My_Sobek_SubMode.Length > 0) && (RequestSpecificValues.Current_Mode.My_Sobek_SubMode[0] == '8')) || (RequestSpecificValues.Current_Mode.My_Sobek_Type == My_Sobek_Type_Enum.File_Management) || (RequestSpecificValues.Current_Mode.My_Sobek_Type == My_Sobek_Type_Enum.Page_Images_Management))
			{

                Output.WriteLine("  <script src=\"" + Static_Resources.Jquery_Uploadifive_Js + "\" type=\"text/javascript\"></script>");
                Output.WriteLine("  <script src=\"" + Static_Resources.Jquery_Uploadify_Js + "\" type=\"text/javascript\"></script>");

				Output.WriteLine("  <link rel=\"stylesheet\" type=\"text/css\" href=\"" + Static_Resources.Uploadifive_Css + "\">");
				Output.WriteLine("  <link rel=\"stylesheet\" type=\"text/css\" href=\"" + Static_Resources.Uploadify_Css + "\">");
			}

			if (( mySobekViewer != null ) && ( mySobekViewer.Viewer_Behaviors.Contains(HtmlSubwriter_Behaviors_Enum.MySobek_Subwriter_Mimic_Item_Subwriter)))
			{
                Output.WriteLine("  <link href=\"" + Static_Resources.Sobekcm_Item_Css + "\" rel=\"stylesheet\" type=\"text/css\" />");
			}
		}


        /// <summary> Writes the HTML generated by this my sobek html subwriter directly to the response stream </summary>
        /// <param name="Output"> Stream to which to write the HTML for this subwriter </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> Value indicating if html writer should finish the page immediately after this, or if there are other controls or routines which need to be called first </returns>
        public override bool Write_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("MySobek_HtmlSubwriter.Write_HTML", "Rendering HTML");

            if ((HttpContext.Current.Session["agreement_date"] == null) && (RequestSpecificValues.Current_Mode.My_Sobek_Type == My_Sobek_Type_Enum.New_Item ) && ((RequestSpecificValues.Current_Mode.My_Sobek_SubMode.Length == 0) || (RequestSpecificValues.Current_Mode.My_Sobek_SubMode[0] != '1')))
            {
                RequestSpecificValues.Current_Mode.My_Sobek_SubMode = "1";
            }
                // A few cases skip the view selectors at the top entirely
	        if (mySobekViewer.Standard_Navigation_Type == MySobek_Included_Navigation_Enum.Standard)
	        {
		        // Add the user-specific main menu
		        MainMenus_Helper_HtmlSubWriter.Add_UserSpecific_Main_Menu(Output, RequestSpecificValues);

		        // Start the page container
		        Output.WriteLine("<div id=\"pagecontainer\">");
		        Output.WriteLine("<br />");
	        }
			else if (mySobekViewer.Standard_Navigation_Type == MySobek_Included_Navigation_Enum.LogOn)
			{
				// Add the item views
				Output.WriteLine("<!-- Add the main user-specific menu -->");
				Output.WriteLine("<div id=\"sbkUsm_MenuBar\" class=\"sbkMenu_Bar\">");
				Output.WriteLine("<ul class=\"sf-menu\">");

				// Get ready to draw the tabs
				string sobek_home_text = RequestSpecificValues.Current_Mode.Instance_Abbreviation + " Home";

				// Add the 'SOBEK HOME' first menu option and suboptions
				RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.Aggregation;
				RequestSpecificValues.Current_Mode.Aggregation_Type = Aggregation_Type_Enum.Home;
				RequestSpecificValues.Current_Mode.Home_Type = Home_Type_Enum.List;
                Output.WriteLine("\t\t<li id=\"sbkUsm_Home\" class=\"sbkMenu_Home\"><a href=\"" + UrlWriterHelper.Redirect_URL(RequestSpecificValues.Current_Mode) + "\" class=\"sbkMenu_NoPadding\"><img src=\"" + Static_Resources.Home_Png + "\" /> <div class=\"sbkMenu_HomeText\">" + sobek_home_text + "</div></a></li>");
				Output.WriteLine("\t</ul></div>");

				Output.WriteLine("<!-- Initialize the main user menu -->");
				Output.WriteLine("<script>");
				Output.WriteLine("  jQuery(document).ready(function () {");
				Output.WriteLine("     jQuery('ul.sf-menu').superfish();");
				Output.WriteLine("  });");
				Output.WriteLine("</script>");
				Output.WriteLine();

				// Restore the current view information type
				RequestSpecificValues.Current_Mode.Mode = Display_Mode_Enum.My_Sobek;

				// Start the page container
				Output.WriteLine("<div id=\"pagecontainer\">");
				Output.WriteLine("<br />");


			}
			else if ( !Subwriter_Behaviors.Contains(HtmlSubwriter_Behaviors_Enum.MySobek_Subwriter_Mimic_Item_Subwriter))
			{
				// Start the page container
				Output.WriteLine("<div id=\"pagecontainer\">");
			}

            // Add the text here
            mySobekViewer.Write_HTML(Output, Tracer);

            return false;
        }



		/// <summary> Writes the html to the output stream open the itemNavForm, which appears just before the TocPlaceHolder </summary>
		/// <param name="Output"> Stream to which to write the text for this main writer </param>
		/// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
		public override void Write_ItemNavForm_Opening(TextWriter Output, Custom_Tracer Tracer)
		{
			Tracer.Add_Trace("MySobek_HtmlSubwriter.Write_ItemNavForm_Closing", "");

			// Also, add any additional stuff here
			mySobekViewer.Write_ItemNavForm_Opening(Output, Tracer);
		}

		/// <summary> Writes additional HTML needed in the main form before the main place holder but after the other place holders.  </summary>
		/// <param name="Output">Stream to directly write to</param>
		/// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
		public override void Write_Additional_HTML(TextWriter Output, Custom_Tracer Tracer)
		{
			Tracer.Add_Trace("MySobek_HtmlSubwriter.Write_Additional_HTML", "Adding any form elements popup divs");
			if ((RequestSpecificValues.Current_Mode.Logon_Required) || (mySobekViewer.Contains_Popup_Forms))
			{
				mySobekViewer.Add_Popup_HTML(Output, Tracer);
			}
		}


	    /// <summary> Adds any necessary controls to one of two place holders on the main ASPX page </summary>
	    /// <param name="MainPlaceHolder"> Main place holder ( &quot;mainPlaceHolder&quot; ) in the itemNavForm form, widely used throughout the application</param>
	    /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
	    public void Add_Controls(PlaceHolder MainPlaceHolder, Custom_Tracer Tracer)
	    {
		    Tracer.Add_Trace("MySobek_HtmlSubwriter.Add_Controls", "Build my sobek viewer and add controls");

		    // Add any controls needed
		    if (mySobekViewer != null)
			    mySobekViewer.Add_Controls(MainPlaceHolder, Tracer);
	    }

	    /// <summary> Writes final HTML to the output stream after all the placeholders and just before the itemNavForm is closed.  </summary>
		/// <param name="Output"> Stream to which to write the text for this main writer </param>
		/// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
		public override void Write_ItemNavForm_Closing(TextWriter Output, Custom_Tracer Tracer)
		{
			Tracer.Add_Trace("MySobek_HtmlSubwriter.Write_ItemNavForm_Closing", "");

			// Also, add any additional stuff here
			mySobekViewer.Write_ItemNavForm_Closing(Output, Tracer);
		}

        /// <summary> Writes final HTML after all the forms </summary>
        /// <param name="Output">Stream to directly write to</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        public override void Write_Final_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            
	        if (!Subwriter_Behaviors.Contains(HtmlSubwriter_Behaviors_Enum.MySobek_Subwriter_Mimic_Item_Subwriter))
	        {
				Output.WriteLine("<!-- Close the pagecontainer div -->");
				Output.WriteLine("</div>");
				Output.WriteLine();
	        }

        }

		/// <summary> Gets the CSS class of the container that the page is wrapped within </summary>
		public override string Container_CssClass
		{
			get
			{
				if ((RequestSpecificValues.Current_Mode.My_Sobek_Type == My_Sobek_Type_Enum.Edit_Item_Metadata) && (!String.IsNullOrEmpty(RequestSpecificValues.Current_Mode.My_Sobek_SubMode)) && (RequestSpecificValues.Current_Mode.My_Sobek_SubMode.IndexOf("0.2") == 0))
					return "container-inner1000";

				if ((RequestSpecificValues.Current_Mode.My_Sobek_Type == My_Sobek_Type_Enum.Edit_Group_Behaviors) || (RequestSpecificValues.Current_Mode.My_Sobek_Type == My_Sobek_Type_Enum.Edit_Item_Behaviors) ||
				    (RequestSpecificValues.Current_Mode.My_Sobek_Type == My_Sobek_Type_Enum.Edit_Item_Metadata) || (RequestSpecificValues.Current_Mode.My_Sobek_Type == My_Sobek_Type_Enum.Group_Add_Volume) ||
				    (RequestSpecificValues.Current_Mode.My_Sobek_Type == My_Sobek_Type_Enum.Group_Mass_Update_Items) || (RequestSpecificValues.Current_Mode.My_Sobek_Type == My_Sobek_Type_Enum.New_Item))
				{
					return "container-inner1000";
				}

				return base.Container_CssClass;
			}
		}
    }
}
