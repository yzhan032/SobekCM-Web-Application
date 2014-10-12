﻿#region Using directives

using System;
using System.IO;
using System.Text;
using System.Web;
using SobekCM.Core.Configuration;
using SobekCM.Resource_Object;
using SobekCM.Library.Application_State;
using SobekCM.Library.Configuration;
using SobekCM.Core.Users;

#endregion

namespace SobekCM.Library.Citation.Elements
{
    /// <summary> Element allows entry of the date the material was received and any associated notes </summary>
    /// <remarks> This class extends the <see cref="textBox_TextBox_Element"/> class. </remarks>
    public class Material_Received_Date : textBox_TextBox_Element
    {
        /// <summary> Constructor for a new instance of the Material_Received_Date class </summary>
        public Material_Received_Date()
            : base("Material Recd:", "material_recd")
        {
            first_label = "Date";
            second_label = "Notes";
            Repeatable = false;
            Type = Element_Type.Material_Received_Date;
        }


        /// <summary> Renders the HTML for this element </summary>
        /// <param name="Output"> Textwriter to write the HTML for this element </param>
        /// <param name="Bib"> Object to populate this element from </param>
        /// <param name="Skin_Code"> Code for the current skin </param>
        /// <param name="IsMozilla"> Flag indicates if the current browse is Mozilla Firefox (different css choices for some elements)</param>
        /// <param name="PopupFormBuilder"> Builder for any related popup forms for this element </param>
        /// <param name="Current_User"> Current user, who's rights may impact the way an element is rendered </param>
        /// <param name="CurrentLanguage"> Current user-interface language </param>
        /// <param name="Translator"> Language support object which handles simple translational duties </param>
        /// <param name="Base_URL"> Base URL for the current request </param>
        /// <remarks> This simple element does not append any popup form to the popup_form_builder</remarks>
        public override void Render_Template_HTML(TextWriter Output, SobekCM_Item Bib, string Skin_Code, bool IsMozilla, StringBuilder PopupFormBuilder, User_Object Current_User, Web_Language_Enum CurrentLanguage, Language_Support_Info Translator, string Base_URL)
        {
            // Check that an acronym exists
            if (Acronym.Length == 0)
            {
                const string defaultAcronym = "Enter the date the material was received and any notes";
                switch (CurrentLanguage)
                {
                    case Web_Language_Enum.English:
                        Acronym = defaultAcronym;
                        break;

                    case Web_Language_Enum.Spanish:
                        Acronym = defaultAcronym;
                        break;

                    case Web_Language_Enum.French:
                        Acronym = defaultAcronym;
                        break;

                    default:
                        Acronym = defaultAcronym;
                        break;
                }
            }

            string dateString = String.Empty;
            if (Bib.Tracking.Material_Received_Date.HasValue)
                dateString = Bib.Tracking.Material_Received_Date.Value.ToShortDateString();
            render_helper(Output, dateString, Bib.Tracking.Material_Received_Notes, Skin_Code, Current_User, CurrentLanguage, Translator, Base_URL);
        }

        /// <summary> Prepares the bib object for the save, by clearing any existing data in this element's related field(s) </summary>
        /// <param name="Bib"> Existing digital resource object which may already have values for this element's data field(s) </param>
        /// <param name="Current_User"> Current user, who's rights may impact the way an element is rendered </param>
        /// <remarks> This clears the notes field and date field </remarks>
        public override void Prepare_For_Save(SobekCM_Item Bib, User_Object Current_User)
        {
            Bib.Tracking.Material_Received_Date = null;
            Bib.Tracking.Material_Received_Notes = String.Empty;
        }

        /// <summary> Saves the data rendered by this element to the provided bibliographic object during postback </summary>
        /// <param name="Bib"> Object into which to save the user's data, entered into the html rendered by this element </param>
        public override void Save_To_Bib(SobekCM_Item Bib)
        {
            string[] getKeys = HttpContext.Current.Request.Form.AllKeys;
            string dateString = String.Empty;
            string notes = String.Empty;
            foreach (string thisKey in getKeys)
            {
                if (thisKey.IndexOf( "materialrecd_first") == 0)
                {
                    dateString = HttpContext.Current.Request.Form[thisKey];
                }

                if (thisKey.IndexOf( "materialrecd_second") == 0)
                {
                    notes = HttpContext.Current.Request.Form[thisKey];
                }
            }
            
            if (dateString.Length > 0)
            {
                Bib.Tracking.Material_Received_Notes = notes;
                try
                {
                    Bib.Tracking.Material_Received_Date = Convert.ToDateTime(dateString);
                }
                catch
                {
                    Bib.Tracking.Material_Received_Date = DateTime.Now;
                }
            }
        }
    }
}



