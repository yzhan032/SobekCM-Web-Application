﻿#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Web.UI.WebControls;
using SobekCM.Core.Navigation;
using SobekCM.Library.AdminViewer;
using SobekCM.Library.HTML;
using SobekCM.Library.MainWriters;
using SobekCM.Library.UI;
using SobekCM.Resource_Object;
using SobekCM.Tools;

#endregion

namespace SobekCM.Library.MySobekViewer
{
	/// <summary> Enumeration indicates which type of main menu navigation
	/// to include </summary>
    public enum MySobek_Included_Navigation_Enum : byte
    {
        /// <summary> Suppress the standard mySobek navigational elements.  This viewer will
        /// utilize its own navigational elements at the top of the page </summary>
        NONE = 1,

        /// <summary> Standard mySobek navigation menu </summary>
        Standard,

		/// <summary> Special navigation menu for the logon screen </summary>
		LogOn
    }

    /// <summary> Abstract class which all mySobek viewer classes extend </summary>
    /// <remarks> MySobek Viewers are used for registration and authentication with mySobek, as well as performing any task which requires
    /// authentication, such as online submittal, metadata editing, and system administrative tasks.<br /><br />
    /// During a valid html request, the following steps occur:
    /// <ul>
    /// <li>Application state is built/verified by the Application_State_Builder </li>
    /// <li>Request is analyzed by the QueryString_Analyzer and output as a <see cref="Navigation_Object"/> </li>
    /// <li>Main writer is created for rendering the output, in his case the <see cref="Html_MainWriter"/> </li>
    /// <li>The HTML writer will create the necessary subwriter.  If the action requires authentication, an instance of the  <see cref="MySobek_HtmlSubwriter"/> class is created. </li>
    /// <li>To allow the requested action, the mySobek subwriter will create one of the mySobek viewers( implementing this class )</li>
    /// </ul></remarks>
    public abstract class abstract_MySobekViewer : iMySobek_Admin_Viewer
    {
        /// <summary> Empty list of behaviors, returned by default </summary>
        /// <remarks> This just prevents an empty set from having to be created over and over </remarks>
        protected static List<HtmlSubwriter_Behaviors_Enum> emptybehaviors = new List<HtmlSubwriter_Behaviors_Enum>();

        /// <summary> Protected field contains all the necessary, non-global data specific to the current request </summary>
        protected RequestCache RequestSpecificValues;

        /// <summary> Constructor for a new instance of the abstract_MySobekViewer class </summary>
        /// <param name="RequestSpecificValues"> All the necessary, non-global data specific to the current request </param>
        protected abstract_MySobekViewer(RequestCache RequestSpecificValues)
        {
            this.RequestSpecificValues = RequestSpecificValues;
        }

        /// <summary> Title for the page that displays this viewer, this is shown in the search box at the top of the page, just below the banner </summary>
        /// <remarks> Abstract property must be implemented by all extending classes </remarks>
        public abstract string Web_Title { get; }

        /// <summary> Gets the URL for the icon related to this mySobek task </summary>
        /// <remarks> Abstract property must be implemented by all extending classes </remarks>
        public virtual string Viewer_Icon { get { return String.Empty; }}

        /// <summary> Property indicates the standard navigation to be included at the top of the page by the
        /// main MySobek html subwriter. </summary>
        /// <value> This defaults to STANDARD, but can be overwritte by any mySobek viewer </value>
        /// <remarks> This is set to NONE if the viewer will write its own navigation and ADMIN if the standard
        /// administrative tabs should be included as well.  </remarks>
        public virtual MySobek_Included_Navigation_Enum Standard_Navigation_Type
        {
            get
            {
                return MySobek_Included_Navigation_Enum.Standard;
            } 
        }
    
        /// <summary> Property indicates if this mySobek viewer can contain pop-up forms</summary>
        /// <remarks> If the mySobek viewer contains pop-up forms the overall page renders differently, 
        /// allowing for the blanket division and the popup forms near the top of the rendered HTML </remarks>
        ///<value> This defaults to FALSE but is overwritten by the mySobek viewers which use pop-up forms </value>
        public virtual bool Contains_Popup_Forms
        {
            get { return false; }
        }


        /// <summary> Gets the collection of special behaviors which this admin or mySobek viewer
        /// requests from the main HTML subwriter. </summary>
        /// <remarks> By default, this returns an empty list </remarks>
        public virtual List<HtmlSubwriter_Behaviors_Enum> Viewer_Behaviors
        {
            get { return emptybehaviors; }
        }

        /// <summary> Add the HTML to be displayed in the main SobekCM viewer area (outside of any form) </summary>
        /// <param name="Output"> Textwriter to write the HTML for this viewer</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> Abstract method must be implemented by all extending classes </remarks>
        public abstract void Write_HTML(TextWriter Output, Custom_Tracer Tracer);

        /// <summary> Add the HTML to be added near the top of the page for those viewers that implement pop-up forms for data retrieval </summary>
        /// <param name="Output"> Textwriter to write the pop-up form HTML for this viewer </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        ///  <remarks> No html is added here, although some children class override this virtual method to add pop-up form HTML </remarks>
        public virtual void Add_Popup_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("abstract_MySobekViewer.Add_Popup_HTML", "No html added");
            }

            // No html to be added here
        }

		/// <summary> This is an opportunity to write HTML directly into the main form before any controls are placed in the main place holder </summary>
		/// <param name="Output"> Textwriter to write the pop-up form HTML for this viewer </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
		/// <remarks> This text will appear within the ItemNavForm form tags </remarks>
		public virtual void Write_ItemNavForm_Opening(TextWriter Output, Custom_Tracer Tracer)
		{
			if (Tracer != null)
			{
				Tracer.Add_Trace("abstract_MySobekViewer.Write_ItemNavForm_Opening", "No HTML Added");
			}
		}

        /// <summary> This is an opportunity to write HTML directly into the main form, without
        /// using the pop-up html form architecture </summary>
        /// <param name="Output"> Textwriter to write the pop-up form HTML for this viewer </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This text will appear within the ItemNavForm form tags </remarks>
        public virtual void Write_ItemNavForm_Closing(TextWriter Output, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("abstract_MySobekViewer.Write_ItemNavForm_Closing", "No HTML Added");
            }
        }

		/// <summary> Add controls directly to the form in the main control area placeholder </summary>
        /// <param name="MainPlaceHolder"> Main place holder to which all main controls are added </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering</param>
        ///  <remarks> No controls are added here, although some children class override this virtual method to add controls </remarks>
        public virtual void Add_Controls(PlaceHolder MainPlaceHolder, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("abstract_MySobekViewer.Add_Controls", "No controls added");
            }

            // No controls to be added here
        }

		/// <summary> Writes the top part of the page, mimicing the item viewer </summary>
		/// <param name="Output"> Stream to write the item-level top to </param>
		/// <param name="Item"> Item with all the information necessary to write the top </param>
		protected void Write_Item_Type_Top(TextWriter Output, SobekCM_Item Item )
		{
			Output.WriteLine("<div id=\"sbkIsw_Titlebar\">");

			string final_title = Item.Bib_Info.Main_Title.Title;
			if (Item.Bib_Info.Main_Title.NonSort.Length > 0)
			{
				if (Item.Bib_Info.Main_Title.NonSort[Item.Bib_Info.Main_Title.NonSort.Length - 1] == ' ')
					final_title = Item.Bib_Info.Main_Title.NonSort + Item.Bib_Info.Main_Title.Title;
				else
				{
					if (Item.Bib_Info.Main_Title.NonSort[Item.Bib_Info.Main_Title.NonSort.Length - 1] == '\'')
					{
						final_title = Item.Bib_Info.Main_Title.NonSort + Item.Bib_Info.Main_Title.Title;
					}
					else
					{
						final_title = Item.Bib_Info.Main_Title.NonSort + " " + Item.Bib_Info.Main_Title.Title;
					}
				}
			}

			// Add the Title if there is one
			if (final_title.Length > 0)
			{
				// Is this a newspaper?
				bool newspaper = Item.Behaviors.GroupType.ToUpper() == "NEWSPAPER";

				// Does a custom setting override the default behavior to add a date?
				if ((newspaper) && (UI_ApplicationCache_Gateway.Settings.Additional_Settings.ContainsKey("Item Viewer.Include Date In Title")) && (UI_ApplicationCache_Gateway.Settings.Additional_Settings["Item Viewer.Include Date In Title"].ToUpper() == "NEVER"))
					newspaper = false;

				// Add the date if it should be added
				if ((newspaper) && ((Item.Bib_Info.Origin_Info.Date_Created.Length > 0) || (Item.Bib_Info.Origin_Info.Date_Issued.Length > 0)))
				{
					string date = Item.Bib_Info.Origin_Info.Date_Created;
					if (Item.Bib_Info.Origin_Info.Date_Created.Length == 0)
						date = Item.Bib_Info.Origin_Info.Date_Issued;


					if (final_title.Length > 125)
					{
						Output.WriteLine("\t<h1 itemprop=\"name\"><abbr title=\"" + final_title + "\">" + final_title.Substring(0, 120) + "...</abbr> ( " + date + " )</h1>");
					}
					else
					{
						Output.WriteLine("\t<h1 itemprop=\"name\">" + final_title + " ( " + date + " )</h1>");
					}
				}
				else
				{
					if (final_title.Length > 125)
					{
						Output.WriteLine("\t<h1 itemprop=\"name\"><abbr title=\"" + final_title + "\">" + final_title.Substring(0, 120) + "...</abbr></h1>");
					}
					else
					{
						Output.WriteLine("\t<h1 itemprop=\"name\">" + final_title + "</h1>");
					}
				}
			}
			Output.WriteLine("</div>");
			Output.WriteLine("<div class=\"sbkMenu_Bar\" id=\"sbkIsw_MenuBar\" style=\"height:20px\">&nbsp;</div>");


		}
    }
}
