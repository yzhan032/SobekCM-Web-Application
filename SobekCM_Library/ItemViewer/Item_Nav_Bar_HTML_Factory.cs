﻿#region Using directives

using System;
using System.Collections.Generic;
using SobekCM.Core.ApplicationState;
using SobekCM.Core.Navigation;
using SobekCM.Resource_Object;
using SobekCM.Resource_Object.Behaviors;
using SobekCM.Resource_Object.Bib_Info;

#endregion

namespace SobekCM.Library.ItemViewer
{
    /// <summary> Class is used to generate the HTML for the nav bar in SobekCM at the item level </summary>
    public class Item_Nav_Bar_HTML_Factory
    {
        /// <summary> Get the navigation bar html for a view, given information about the current request </summary>
        /// <param name="Item_View"> View for which to generate the html </param>
        /// <param name="Resource_Type"> Current resource type, which determines the text in several viewer's tabs</param>
        /// <param name="Skin_Code"> Code for the current web sking, which determines which tab images to use </param>
        /// <param name="Current_Mode"> Mode / navigation information for the current request</param>
        /// <param name="Page_Sequence"> Current page sequence </param>
        /// <param name="Translator"> Language support object provides support for translating common user interface elements, like the names of these tabs </param>
        /// <param name="Show_Zoomable"> Flag indicates if the zoomable server is online and should be displayable </param>
        /// <param name="Current_Item"> Current digital resource, with the viewers that should be displayed </param>
        /// <returns> Collection of the html for the navigation bar (one view could have multiple tabs)</returns>
        public static List<string> Get_Nav_Bar_HTML(View_Object Item_View, string Resource_Type, 
            string Skin_Code, Navigation_Object Current_Mode, int Page_Sequence,
            Language_Support_Info Translator, bool Show_Zoomable, SobekCM_Item Current_Item )
        {
            List<string> returnVal = new List<string>();

            switch (Item_View.View_Type)
            {
                case View_Enum.ALL_VOLUMES:
                    string allVolumeCode = "allvolumes";
                    string resource_type_upper = Resource_Type.ToUpper();
                    if (Current_Mode.ViewerCode.IndexOf("allvolumes") == 0)
                        allVolumeCode = Current_Mode.ViewerCode;
                    if (resource_type_upper.IndexOf("NEWSPAPER") >= 0)
                    {
                        returnVal.Add(HTML_Helper(allVolumeCode, Translator.Get_Translation("All Issues", Current_Mode.Language), Current_Mode));
                    }
                    else
                    {
                        if (resource_type_upper.IndexOf("MAP") >= 0)
                        {
                            returnVal.Add(HTML_Helper(allVolumeCode, Translator.Get_Translation("Related Maps", Current_Mode.Language), Current_Mode));
                        }
                        else
                        {
                            returnVal.Add(resource_type_upper.IndexOf("AERIAL") >= 0
                                              ? HTML_Helper(allVolumeCode, Translator.Get_Translation("Related Flights", Current_Mode.Language), Current_Mode)
                                              : HTML_Helper(allVolumeCode, Translator.Get_Translation("All Volumes", Current_Mode.Language), Current_Mode));
                        }
                    }
                    break;

                case View_Enum.CITATION:
                    if ((Current_Mode.ViewerCode == "citation") || ( Current_Mode.ViewerCode == "marc" ) || ( Current_Mode.ViewerCode == "metadata" ) || ( Current_Mode.ViewerCode == "usage" ))
                    {
                        returnVal.Add(HTML_Helper(Current_Mode.ViewerCode, Translator.Get_Translation("Citation", Current_Mode.Language), Current_Mode));
                    }
                    else
                    {
                        returnVal.Add(HTML_Helper("citation", Translator.Get_Translation("Citation", Current_Mode.Language), Current_Mode));
                    }
                    break;

				case View_Enum.DATASET_CODEBOOK:
					returnVal.Add(HTML_Helper("dscodebook", Translator.Get_Translation("Data Structure", Current_Mode.Language), Current_Mode));
					break;

				case View_Enum.DATASET_REPORTS:
					returnVal.Add(HTML_Helper("dsreports", Translator.Get_Translation("Reports", Current_Mode.Language), Current_Mode));
					break;

				case View_Enum.DATASET_VIEWDATA:
					returnVal.Add(HTML_Helper("dsview", Translator.Get_Translation("Explore Data", Current_Mode.Language), Current_Mode));
					break;

                case View_Enum.DOWNLOADS:
                    returnVal.Add(HTML_Helper("downloads", Translator.Get_Translation("Downloads", Current_Mode.Language), Current_Mode));
                    break;

                case View_Enum.FEATURES:
                    returnVal.Add(HTML_Helper("features", Translator.Get_Translation("Features", Current_Mode.Language), Current_Mode));
                    break;

                case View_Enum.FLASH:
                    returnVal.Add( String.IsNullOrEmpty(Item_View.Label)
                                      ? HTML_Helper("flash", Translator.Get_Translation("Flash View", Current_Mode.Language), Current_Mode)
                                      : HTML_Helper("flash", Translator.Get_Translation(Item_View.Label.ToUpper(), Current_Mode.Language), Current_Mode));
                    break;

                case View_Enum.GOOGLE_MAP:
                    if ( !String.IsNullOrEmpty(Current_Mode.Coordinates))
                    {
                        if (Current_Mode.ViewerCode == "mapsearch")
                        {
                            returnVal.Add( HTML_Helper("mapsearch", Translator.Get_Translation("Map Search", Current_Mode.Language), Current_Mode));
                        }
                        else
                        {
                            if (( Current_Item.Web.Static_PageCount > 1 ) || ( Current_Item.Bib_Info.SobekCM_Type != TypeOfResource_SobekCM_Enum.Map ))
                            {
                                returnVal.Add(HTML_Helper("map", Translator.Get_Translation("Search Results", Current_Mode.Language), Current_Mode));
                            }
                            else
                            {
                                returnVal.Add(HTML_Helper("map", Translator.Get_Translation("Map Coverage", Current_Mode.Language), Current_Mode));
                            }
 
                        }
                    }
                    else
                    {
                        returnVal.Add(HTML_Helper("map", Translator.Get_Translation("Map It!", Current_Mode.Language), Current_Mode));
                    }
                    break;

                case View_Enum.GOOGLE_MAP_BETA:
                    if (!String.IsNullOrEmpty(Current_Mode.Coordinates))
                    {
                        if (Current_Mode.ViewerCode == "mapsearchbeta")
                        {
                            returnVal.Add(HTML_Helper("mapsearchbeta", Translator.Get_Translation("Map Search", Current_Mode.Language), Current_Mode));
                        }
                        else
                        {
                            if ((Current_Item.Web.Static_PageCount > 1) || (Current_Item.Bib_Info.SobekCM_Type != TypeOfResource_SobekCM_Enum.Map_Beta))
                            {
                                returnVal.Add(HTML_Helper("mapbeta", Translator.Get_Translation("Search Results", Current_Mode.Language), Current_Mode));
                            }
                            else
                            {
                                returnVal.Add(HTML_Helper("mapbeta", Translator.Get_Translation("Map Coverage", Current_Mode.Language), Current_Mode));
                            }

                        }
                    }
                    else
                    {
                        returnVal.Add(HTML_Helper("map", Translator.Get_Translation("Map It!", Current_Mode.Language), Current_Mode));
                    }
                    break;

                case View_Enum.HTML:
                    returnVal.Add(!String.IsNullOrEmpty(Item_View.Label)
                                      ? HTML_Helper("html", Item_View.Label.ToUpper(), Current_Mode)
                                      : HTML_Helper("html", "HTML LINK", Current_Mode));
                    break;

                case View_Enum.JPEG:
                    returnVal.Add(HTML_Helper_PageView(Page_Sequence.ToString() + "j", Translator.Get_Translation("Standard", Current_Mode.Language), Current_Mode));
                    break;

				case View_Enum.JPEG_TEXT_TWO_UP:
					returnVal.Add(HTML_Helper_PageView(Page_Sequence.ToString() + "u", Translator.Get_Translation("Page Image with Text", Current_Mode.Language), Current_Mode));
					break;

                case View_Enum.JPEG2000:
                    if (Show_Zoomable)
                    {
                        returnVal.Add(HTML_Helper_PageView(Page_Sequence.ToString() + "x", Translator.Get_Translation("Zoomable", Current_Mode.Language), Current_Mode));
                    }
                    break;

				case View_Enum.PDF:
					returnVal.Add(HTML_Helper("pdf", Translator.Get_Translation("PDF Viewer", Current_Mode.Language), Current_Mode));
					break;

                case View_Enum.RELATED_IMAGES:
                    returnVal.Add(Current_Mode.ViewerCode.IndexOf("thumbs") >= 0
                                      ? HTML_Helper(Current_Mode.ViewerCode, Translator.Get_Translation("Thumbnails", Current_Mode.Language), Current_Mode)
                                      : HTML_Helper("thumbs", Translator.Get_Translation("Thumbnails", Current_Mode.Language), Current_Mode));
                    break;

				case View_Enum.SEARCH:
					returnVal.Add(HTML_Helper("search", Translator.Get_Translation("Search", Current_Mode.Language), Current_Mode));
					break;

                case View_Enum.SIMPLE_HTML_LINK:
                    returnVal.Add("<li> <a href=\"" + Item_View.Attributes + "\" target=\"_blank\" alt=\"Link to '" + Item_View.Label + "'\"> " + Translator.Get_Translation(Item_View.Label.ToUpper(), Current_Mode.Language) + " </a></li>");
                    break;

                case View_Enum.STREETS:
                    returnVal.Add(HTML_Helper("streets", Translator.Get_Translation("Streets", Current_Mode.Language), Current_Mode));
                    break;

                case View_Enum.TEXT:
                    returnVal.Add(HTML_Helper_PageView(Page_Sequence.ToString() + "t", Translator.Get_Translation("Page Text", Current_Mode.Language), Current_Mode));
                    break;

                case View_Enum.TOC:
                     // returnVal.Add(base.HTML_Helper(Skin_Code, "TC", "Table of Contents", Current_Mode));
                    break;
                    
                case View_Enum.RESTRICTED:
                    returnVal.Add(HTML_Helper("restricted", Translator.Get_Translation("Restricted", Current_Mode.Language), Current_Mode));
                    break;

                case View_Enum.EAD_CONTAINER_LIST:
                    returnVal.Add(HTML_Helper("container", Translator.Get_Translation("Container List", Current_Mode.Language), Current_Mode));
                    break;

                case View_Enum.EAD_DESCRIPTION:
                    // Return nothing, this is currently written when writing the CITATION, for 
                    // all EAD type items.
                    //returnVal.Add(HTML_Helper(Skin_Code, "description", Translator.Get_Translation("DESCRIPTION", Current_Mode.Language), Current_Mode));
                    break;

                case View_Enum.PAGE_TURNER:
                    returnVal.Add(HTML_Helper("pageturner#page/1/mode/2up", Translator.Get_Translation("Page Turner", Current_Mode.Language), Current_Mode));
                    break;

                case View_Enum.YOUTUBE_VIDEO:
                    returnVal.Add(HTML_Helper("youtube", Translator.Get_Translation("Video", Current_Mode.Language), Current_Mode));
                    break;

                case View_Enum.EMBEDDED_VIDEO:
                    returnVal.Add(HTML_Helper("videoem", Translator.Get_Translation("Video", Current_Mode.Language), Current_Mode));
                    break;

                case View_Enum.VIDEO:
                    returnVal.Add(HTML_Helper("video", Translator.Get_Translation("Video", Current_Mode.Language), Current_Mode));
                    break;

                case View_Enum.TRACKING:
                    // DO nothing in this case.. do not write any tab
                    break;

                case View_Enum.TRACKING_SHEET:
                    //DO nothing in this case.. do not write any tab
                    break;

                case View_Enum.QUALITY_CONTROL:
                    // DO nothing in this case.. do not write any tab
                    break;
            }

            return returnVal;
        }

        private static string HTML_Helper_PageView(string Viewer_Code, string Display_Text, Navigation_Object Current_Mode)
        {
            string previousViewerCode = Current_Mode.ViewerCode;
            Current_Mode.ViewerCode = Viewer_Code;
            string returnValue = "<a href=\"" + UrlWriterHelper.Redirect_URL(Current_Mode) + "\">" + Display_Text + "</a>";
            Current_Mode.ViewerCode = previousViewerCode;
            return returnValue;
        }

        private static string HTML_Helper(string Viewer_Code, string Display_Text, Navigation_Object Current_Mode)
        {
            if (Current_Mode.ViewerCode == Viewer_Code)
            {
                return "<li class=\"selected-sf-menu-item\">" + Display_Text + "</li>";
            }

            // When rendering for robots, provide the text and image, but not the text
            if (Current_Mode.Is_Robot)
            {
                return "<li class=\"selected-sf-menu-item\">" + Display_Text + "</li>";
            }

            string previousViewerCode = Current_Mode.ViewerCode;
            Current_Mode.ViewerCode = Viewer_Code;
            string returnValue = "<li><a href=\"" + UrlWriterHelper.Redirect_URL(Current_Mode) + "\">" + Display_Text + "</a></li>";
            Current_Mode.ViewerCode = previousViewerCode;
            return returnValue;
        }
    }
}
