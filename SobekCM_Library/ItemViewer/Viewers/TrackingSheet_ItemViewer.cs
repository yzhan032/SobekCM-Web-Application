﻿#region Using directives
using System;
using SobekCM.Library.Settings;
using Zen.Barcode;
using System.Collections.Generic;
using System.IO;
using System.Data;
using SobekCM.Library.HTML;
using SobekCM.Library.Navigation;
using SobekCM.Library.Users;
using SobekCM.Resource_Object;
#endregion

namespace SobekCM.Library.ItemViewer.Viewers
{
    public class TrackingSheet_ItemViewer : abstractItemViewer
    {
        private SobekCM_Item track_item;
        private int itemID;
        private DataSet item_details_dataset;
        private DataTable item_details;
        private DataTable aggregation_details;
        private string image_location;
        private string username;
        private List<string> aggregation_list;
        private string aggregations;

        /// <summary>
        /// Constructor for the Tracking Sheet ItemViewer
        /// </summary>
        /// <param name="Current_Object"></param>
        /// <param name="Current_User"></param>
        /// <param name="Current_Mode"></param>
        public TrackingSheet_ItemViewer(SobekCM_Item Current_Object, User_Object Current_User, SobekCM_Navigation_Object Current_Mode)
        {
            CurrentMode = Current_Mode;
            CurrentUser = Current_User;
            
            // If there is no user, send to the login
            if (CurrentUser == null)
            {
                CurrentMode.Mode = Display_Mode_Enum.My_Sobek;
                CurrentMode.My_Sobek_Type = My_Sobek_Type_Enum.Logon;
                CurrentMode.Redirect();
                return;
            }

            // If the user cannot edit this item, go back
            if (!CurrentUser.Can_Edit_This_Item(Current_Object))
            {
                CurrentMode.ViewerCode = String.Empty;
                CurrentMode.Redirect();
                return;
            }

            //Assign the current resource object to track_item
            track_item = Current_Object;

            //Get the ItemID for this Item from the database
            itemID = Resource_Object.Database.SobekCM_Database.Get_ItemID(track_item.BibID, track_item.VID);

            //Also get the item & aggregation details from the database
            item_details_dataset = Database.SobekCM_Database.Get_Item_Details(track_item.BibID, track_item.VID, null);
            item_details = item_details_dataset.Tables[2];
            aggregation_details = item_details_dataset.Tables[1];

            //Get the list of aggregations from the data table
            aggregation_list = new List<string>();
            foreach (DataRow row in aggregation_details.Rows)
            {
                aggregation_list.Add(row["Code"].ToString());
                aggregations = aggregations + row["Code"].ToString() + ", ";
            }
            //Remove extra comma and space from the end
            aggregations = aggregations.Substring(0, aggregations.Length - 2);

            

            //Get the temporary location for saving the barcode images
            image_location = SobekCM_Library_Settings.Base_Temporary_Directory + Current_User.UserName.Replace(".", "").Replace("@", "") + "\\tsBarcodes\\" + itemID.ToString();
            username = Current_User.UserName.Replace(".", "").Replace("@", "");
            if (Current_User.UFID.Trim().Length > 0)
            {
                image_location = SobekCM_Library_Settings.Base_Temporary_Directory + "\\" + Current_User.UFID + "\\tsBarcodes\\" + itemID.ToString();
                username = Current_User.UFID;
            }

            // Make the folder for the user in the temp directory
            if (!Directory.Exists(image_location))
                Directory.CreateDirectory(image_location);

        }


        /// <summary> Write any additional values within the HTML Head of the final served page </summary>
        /// <param name="Output"> Output stream currently within the HTML head tags </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public override void Write_Within_HTML_Head(TextWriter Output, Custom_Tracer Tracer)
        {
            Output.WriteLine("  <link rel=\"stylesheet\" type=\"text/css\" href=\"" + CurrentMode.Base_URL + "default/SobekCM_TrackingSheet.css\" /> ");
            Output.WriteLine("  <link rel=\"stylesheet\" type=\"text/css\" href=\"" + CurrentMode.Base_URL + "default/scrollbars.css\" />");
            Output.WriteLine("  <link rel=\"stylesheet\" type=\"text/css\" href=\"" + CurrentMode.Base_URL + "default/scrollbars-black.css\" />");

        }

        /// <summary> Gets the collection of special behaviors which this item viewer
        /// requests from the main HTML subwriter. </summary>
        public override List<HtmlSubwriter_Behaviors_Enum> ItemViewer_Behaviors
        {
            get
            {
                return new List<HtmlSubwriter_Behaviors_Enum> 
                    {
                        HtmlSubwriter_Behaviors_Enum.Item_Subwriter_NonWindowed_Mode,
                        HtmlSubwriter_Behaviors_Enum.Suppress_Footer,
                        HtmlSubwriter_Behaviors_Enum.Suppress_Internal_Header,
                        HtmlSubwriter_Behaviors_Enum.Item_Subwriter_Suppress_Item_Menu,
                        HtmlSubwriter_Behaviors_Enum.Item_Subwriter_Suppress_Left_Navigation_Bar,
                        HtmlSubwriter_Behaviors_Enum.Suppress_Header,
                        HtmlSubwriter_Behaviors_Enum.Suppress_Banner,
                        HtmlSubwriter_Behaviors_Enum.Item_Subwriter_Suppress_Titlebar
                    };
            }
        }


        /// <summary> Stream to which to write the HTML for this subwriter  </summary>
        /// <param name="Output"> Response stream for the item viewer to write directly to </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public override void Write_Main_Viewer_Section(TextWriter Output, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("TrackingSheet_ItemViewer.Write_Main_Viewer_Section", "");
            }

            Output.WriteLine("\t\t<!-- TRACKING SHEET VIEWER OUTPUT -->");

            //Start the outer main table
            Output.WriteLine("<table class=\"sbkTs_MainTable\"><tr><td>");

            //Add the Bib, VID, and TrackingBox numbers in the title
            Output.WriteLine("<span class = \"sbkTs_Title\">" + track_item.BibID + " : " + track_item.VID + "</span>");
            Output.WriteLine("<span class=\"sbkTs_Title_right\">"+item_details.Rows[0]["Tracking_Box"]+"</span>");

            //Start the Material Information Box
            Output.WriteLine("<table class=\"sbkTs_tblMaterialInfo\">");
            Output.WriteLine("<tr><td colspan=\"3\"><span class=\"sbkTs_tableHeader\">Material Information</span></td></tr>");

            string label_space = "&nbsp;&nbsp;&nbsp;";
            //Add the title
            Output.WriteLine("<tr><td colspan=\"3\"><span class=\"sbkTs_tableLabel\">Title:" + label_space+"</span>");
            Output.WriteLine("<span>" + item_details.Rows[0]["Title"] + "</span></td></tr>");

            //Add the Author
            Output.WriteLine("<tr><td colspan=\"3\" ><span class=\"sbkTs_tableLabel\">Author:" + label_space+"</span>");
            Output.WriteLine("<span>" + item_details.Rows[0]["Author"] + "</span></td></tr>");

            //Add the publisher
            Output.WriteLine("<tr><td colspan=\"3\"><span class=\"sbkTs_tableLabel\">Publisher:" + label_space+"</span>");
            Output.WriteLine("<span>" + item_details.Rows[0]["Publisher"] + "</span></td></tr>");

            //Add the OCLC, Aleph, Material Type info
            Output.WriteLine("<tr><td><span class=\"sbkTs_tableLabel\">OCLC:" + label_space+"</span>");
            Output.WriteLine("<span>" + item_details.Rows[0]["OCLC_Number"] + "</span></td>");
            Output.WriteLine("<td><span class=\"sbkTs_tableLabel\"> Aleph: " + label_space+"</span>");
            Output.WriteLine("<span>" + item_details.Rows[0]["ALEPH_Number"] + "</span></td>");
            Output.WriteLine("<td> <span class=\"sbkTs_tableLabel\">  Material Type:" + label_space+"</span>");
            Output.WriteLine("<span>" + item_details.Rows[0]["Type"] + "</span></td></tr>");
            
            //Add the aggregation info
            Output.WriteLine("<tr><td colspan=\"3\"><span class=\"sbkTs_tableLabel\">Aggregations:"+label_space+"</span>");
            Output.WriteLine("<span>" + aggregations + "</span></td></tr>");

            //End the table
            Output.WriteLine("</table>");
            Output.WriteLine("<br/><br/><br/>");

            //Write the serial hierarchy info, if there is any data available
            if (!((String.IsNullOrEmpty(item_details.Rows[0]["Level1_Text"].ToString())) && String.IsNullOrEmpty(item_details.Rows[0]["Level2_Text"].ToString()) && String.IsNullOrEmpty(item_details.Rows[0]["Level3_Text"].ToString())))
            {
                //Start the Serial Hierarchy Table
                Output.WriteLine("<table class=\"sbkTs_tblSerialHierarchy\">");
                Output.WriteLine("<tr><td><span class=\"sbkTs_tableHeader\">Serial Hierarchy</span></td></tr>");

                Output.WriteLine("<tr><td><span class=\"sbkTs_tableLabel\">Level 1:"+label_space+"</span>");
                Output.WriteLine( item_details.Rows[0]["Level1_Text"] + "</td></tr>");

                Output.WriteLine("<tr><td><span class=\"sbkTs_tableLabel\">Level 2:"+label_space+"</span>");
                Output.WriteLine(item_details.Rows[0]["Level2_Text"] + "</td></tr>");

                Output.WriteLine("<tr><td><span class=\"sbkTs_tableLabel\">Level 3:"+label_space+"</span>");
                Output.WriteLine(item_details.Rows[0]["Level3_Text"] + "</td></tr>");
                
                //End this table
                Output.WriteLine("</table>");
            }
            Output.WriteLine("<br/><br/><br/>");

            //Start the Imaging Progress Table
            Output.WriteLine("<table class=\"sbkTs_tblImagingProgress\">");
            Output.WriteLine("<tr><td colspan=\"8\"><span class=\"sbkTs_tableHeader\">Imaging Progress</span></td></tr>");
           
            //Long space to use as empty fields in the tables
            string label1_space = "&nbsp;";
            for (int i = 0; i < 17; i++)
                label1_space += "&nbsp;";
          
            for (int rowCount = 0; rowCount < 4; rowCount++)
            {
                Output.WriteLine("<tr><td><span class=\"sbkTs_tableLabel\">Name:</span></td><td class=\"sbkTs_tblCellUnderline\">" + label1_space+label1_space + label1_space + label1_space + "</td>");
                Output.WriteLine("<td><span class=\"sbkTs_tableLabel\">Date:</span></td><td class=\"sbkTs_tblCellUnderline\">" + label_space + label_space + "/" + label_space + label_space + " / " + label_space + "</td>");
                Output.WriteLine("<td><span class=\"sbkTs_tableLabel\">Page Range:</span></td><td class=\"sbkTs_tblCellUnderline\">"+label_space+label_space+label_space +"</td>");
                Output.WriteLine("<td><span class=\"sbkTs_tableLabel\">Duration:</span></td><td class=\"sbkTs_tblCellUnderline\">" + label_space +label_space+label_space+ "</td></tr>");
            }
            Output.WriteLine("</table>");
            Output.WriteLine("<br/><br/><br/>");

            //Add the Disposition Notes (only if  there is any data available)
            if (!String.IsNullOrEmpty(item_details.Rows[0][33].ToString()))
            {
                Output.WriteLine("<table class=\"sbkTs_tblDisposition\">");
                Output.WriteLine("<tr><td><span class=\"sbkTs_tableHeader\">Disposition Notes:</span>" + label_space + item_details.Rows[0][33] + "</td></tr>");
                Output.WriteLine("</table>");
                Output.WriteLine("<br/><br/><br/>");
            }
            
            //Add the Additional Notes table
            Output.WriteLine("<table class=\"sbkTs_tblAdditionalNotes\">");
            Output.WriteLine("<tr><td class=\"sbkTs_tableHeader\">Additional Notes:</td></tr>");
            Output.WriteLine("<tr><td  class=\"sbkTs_tblCellUnderline\">&nbsp;&nbsp;&nbsp;&nbsp; </td></tr>");
            Output.WriteLine("<tr><td  class=\"sbkTs_tblCellUnderline\">&nbsp;&nbsp;&nbsp;&nbsp; </td></tr>");
            Output.WriteLine("<tr><td  class=\"sbkTs_tblCellUnderline\">&nbsp;&nbsp;&nbsp;&nbsp; </td></tr>");
            Output.WriteLine("</table>");
            Output.WriteLine("<br/><br/><br/>");

            //Is this item born digital?
            bool born_digital = false;
            Boolean.TryParse(item_details.Rows[0]["Born_Digital"].ToString(), out born_digital);

            if (!born_digital)
            {
                //Get the barcode images for the events
                string imageUrl1 = Get_BarcodeImageUrl_from_string(int_to_base26(itemID), "A", track_item.BibID + track_item.VID + "A");
                string imageUrl2 = Get_BarcodeImageUrl_from_string(int_to_base26(itemID), "B", track_item.BibID + track_item.VID + "B");

                //Start the table for the event barcodes
                Output.WriteLine("<table class=\"sbkTs_tblBarcodes\">");
                Output.WriteLine("<tr><td><img id=\"barcode1\" src=\"" + imageUrl1 + "\"/></td>");
                Output.WriteLine("<td><img id=\"barcode2\" src=\"" + imageUrl2 + "\"/></td></tr>");
                Output.WriteLine("</table>");
            }
            
            //Close the outer table
            Output.WriteLine("</td></tr></table>");

        }

        /// <summary> Converts a base-10 integer to the base-26 equivalent </summary>
        /// <param name="input_number"></param>
        /// <returns>returnValue</returns>
        public string int_to_base26(int input_number)
        {
            string returnValue = String.Empty;

            input_number = Math.Abs(input_number);

            do
            {
                int remainder = input_number % 26;
                returnValue = (char)(remainder + 'A') + returnValue;
                input_number = (input_number - remainder) / 26;
            } while (input_number > 0);

            return returnValue;
        }


        /// <summary> Generates a barcode(with checksum) for a given string </summary>
        /// <param name="inputString">This is the itemID, in base 26 number system</param>
        /// <param name="action">Indicates the Imaging action represented by this barcode</param>
        /// <param name="filename_to_save"></param>
        /// <returns>The url of the generated barcode GIF image</returns>
        public string Get_BarcodeImageUrl_from_string(string inputString, string action, string filename_to_save)
        {
            string returnUrl = String.Empty;

            string barcodeString = (inputString + action).ToUpper();

            Code39BarcodeDraw barcode39 = BarcodeDrawFactory.Code39WithChecksum;
            System.Drawing.Image barcode_image = barcode39.Draw(barcodeString, 40);

            string image_save_location = image_location + "\\" + filename_to_save + ".gif";
            


            //Save the image
            barcode_image.Save(@image_save_location, System.Drawing.Imaging.ImageFormat.Gif);
            returnUrl = "/sobekcm/temp/"+username+"/tsBarcodes/" + itemID.ToString() + "/" + filename_to_save + ".gif";

            return returnUrl;
        }



        /// <summary> Gets the number of pages for this viewer </summary>
        /// <remarks> Always returns 1</remarks>
        public override int PageCount
        {
            get
            {
                return 1;
            }
        }

        /// <summary> Gets the type of item viewer this object represents </summary>
        /// <value> This property always returns the enumerational value <see cref="ItemViewer_Type_Enum.Tracking_Sheet"/>. </value>
        public override ItemViewer_Type_Enum ItemViewer_Type
        {
            get { return ItemViewer_Type_Enum.Tracking_Sheet; }
        }


        /// <summary> Flag indicates if this view should be overriden if the item is checked out by another user </summary>
        /// <remarks> This always returns the value TRUE for this viewer </remarks>
        public override bool Override_On_Checked_Out
        {
            get
            {
                return true;
            }
        }

        /// <summary> Width for the main viewer section to adjusted to accomodate this viewer</summary>
        /// <value> This always returns the value -1</value>
        public override int Viewer_Width
        {
            get
            {
                return -1;
            }
        }

        /// <summary> Height for the main viewer section to adjusted to accomodate this viewer</summary>
        public override int Viewer_Height
        {
            get
            {
                return -1;
            }
        }

    }
}
