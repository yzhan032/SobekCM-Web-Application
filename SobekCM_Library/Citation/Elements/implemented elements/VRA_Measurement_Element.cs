﻿#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using SobekCM.Core.ApplicationState;
using SobekCM.Core.Configuration;
using SobekCM.Core.Users;
using SobekCM.Resource_Object;
using SobekCM.Resource_Object.Metadata_Modules;
using SobekCM.Resource_Object.Metadata_Modules.VRACore;

#endregion

namespace SobekCM.Library.Citation.Elements
{
    /// <summary> Element allows entry of the learning object metadata resource type field </summary>
    /// <remarks> This class extends the <see cref="textBox_TextBox_Element"/> class. </remarks>
    public class VRA_Measurement_Element : textBox_TextBox_Element
    {
        /// <summary> Constructor for a new instance of the VRA_Measurement_Element class </summary>
        public VRA_Measurement_Element() : base("Measurement:", "vra_measurement")
        {
            second_label = "Units";
            Repeatable = true;
            Type = Element_Type.VRA_Measurement;
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
                Acronym = "Measurement related to this item, whether it is length, volume, duration, weight, etc..";
            }

            List<string> terms = new List<string>();
            List<string> schemes = new List<string>();

            // Try to get any existing learning object metadata module
            VRACore_Info vraInfo = Bib.Get_Metadata_Module(GlobalVar.VRACORE_METADATA_MODULE_KEY) as VRACore_Info;
            if (vraInfo != null)
            {
                foreach ( VRACore_Measurement_Info thisMeasurement in vraInfo.Measurements )
                {
                    terms.Add(thisMeasurement.Measurements);
                    schemes.Add(thisMeasurement.Units);
                }
            }

            render_helper(Output, terms, schemes, Skin_Code, Current_User, CurrentLanguage, Translator, Base_URL);
        }

        /// <summary> Prepares the bib object for the save, by clearing any existing data in this element's related field(s) </summary>
        /// <param name="Bib"> Existing digital resource object which may already have values for this element's data field(s) </param>
        /// <param name="Current_User"> Current user, who's rights may impact the way an element is rendered </param>
        /// <remarks> This clears any preexisting measurement information </remarks>
        public override void Prepare_For_Save(SobekCM_Item Bib, User_Object Current_User)
        {
            // Try to get any existing VRAcore metadata module
            VRACore_Info vraInfo = Bib.Get_Metadata_Module(GlobalVar.VRACORE_METADATA_MODULE_KEY) as VRACore_Info;
            if (vraInfo != null)
                vraInfo.Clear_Measurements();
        }

        /// <summary> Saves the data rendered by this element to the provided bibliographic object during postback </summary>
        /// <param name="Bib"> Object into which to save the user's data, entered into the html rendered by this element </param>
        public override void Save_To_Bib(SobekCM_Item Bib)
        {
            // Try to get any existing VRAcore metadata module
            VRACore_Info vraInfo = Bib.Get_Metadata_Module(GlobalVar.VRACORE_METADATA_MODULE_KEY) as VRACore_Info;

            Dictionary<string, string> terms = new Dictionary<string, string>();
            Dictionary<string, string> schemes = new Dictionary<string, string>();

            string[] getKeys = HttpContext.Current.Request.Form.AllKeys;
            foreach (string thisKey in getKeys)
            {
                if (thisKey.IndexOf(html_element_name.Replace("_", "") + "_first") == 0)
                {
                    string term = HttpContext.Current.Request.Form[thisKey];
                    string index = thisKey.Replace(html_element_name.Replace("_", "") + "_first", "");
                    terms[index] = term;
                }

                if (thisKey.IndexOf(html_element_name.Replace("_", "") + "_second") == 0)
                {
                    string scheme = HttpContext.Current.Request.Form[thisKey];
                    string index = thisKey.Replace(html_element_name.Replace("_", "") + "_second", "");
                    schemes[index] = scheme;
                }
            }

            // Were values found?
            if (terms.Count > 0)
            {
                // There is a value, so ensure VRAcore metadata does exist
                if (vraInfo == null)
                {
                    vraInfo = new VRACore_Info();
                    Bib.Add_Metadata_Module(GlobalVar.VRACORE_METADATA_MODULE_KEY, vraInfo);
                }

                // Add each value
                foreach (string index in terms.Keys)
                {
                    vraInfo.Add_Measurement( terms[index], schemes.ContainsKey(index) ? schemes[index] : String.Empty);
                }
            }
        }
    }
}
