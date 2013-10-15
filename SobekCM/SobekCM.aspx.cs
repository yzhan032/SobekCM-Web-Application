#region Includes 

using System;
using System.Web;
using SobekCM.Library.Settings;
using SobekCM.Library.Navigation;

#endregion

public partial class SobekMain : System.Web.UI.Page
{
    private SobekCM_Page_Globals pageGlobals;

    #region Page_Load method does the final checks and creates the writer type

    protected void Page_Load(object Sender, EventArgs E)
    {
        pageGlobals.tracer.Add_Trace("sobekcm(.aspx).Page_Load", String.Empty);

        try
        {
            // Process this page request by building the main writer and 
            // analyzing the request's URL
            pageGlobals.On_Page_Load();

            // Is the response completed already?
            if ((pageGlobals.currentMode == null) || (pageGlobals.currentMode.Request_Completed))
            {
                return;
            }


            if (HttpContext.Current.Items.Contains("Original_URL"))
            {
                string original_url = HttpContext.Current.Items["Original_URL"].ToString();
                itemNavForm.Action = original_url;

                // Save this as the return spot, if it is not preferences
                if ((pageGlobals.currentMode.Mode != Display_Mode_Enum.Preferences) && (pageGlobals.currentMode.Mode != Display_Mode_Enum.Contact))
                {
                    Session["Last_Mode"] = original_url;
                }
            }
            else
            {
                // Save this as the return spot, if it is not preferences
                if ((pageGlobals.currentMode.Mode != Display_Mode_Enum.Preferences) && (pageGlobals.currentMode.Mode != Display_Mode_Enum.Contact))
                {
                    string url = HttpContext.Current.Request.Url.ToString();
                    Session["Last_Mode"] = url;
                }
            }

            if (SobekCM_Library_Settings.Web_Output_Caching_Minutes > 0)
            {
                if ((pageGlobals.currentMode.Mode != Display_Mode_Enum.Error) &&
                    (pageGlobals.currentMode.Mode != Display_Mode_Enum.My_Sobek) &&
                    (pageGlobals.currentMode.Mode != Display_Mode_Enum.Administrative) &&
                    (pageGlobals.currentMode.Mode != Display_Mode_Enum.Contact) &&
                    (pageGlobals.currentMode.Mode != Display_Mode_Enum.Contact_Sent) &&
                    (pageGlobals.currentMode.Mode != Display_Mode_Enum.Item_Print) &&
                    (pageGlobals.currentMode.Mode != Display_Mode_Enum.Item_Cache_Reload) &&
                    (pageGlobals.currentMode.Mode != Display_Mode_Enum.Reset) &&
                    (pageGlobals.currentMode.Mode != Display_Mode_Enum.Internal) &&
                    (pageGlobals.currentMode.Mode != Display_Mode_Enum.Public_Folder) &&
                    ((pageGlobals.currentMode.Mode != Display_Mode_Enum.Aggregation_Home) || (pageGlobals.currentMode.Home_Type != Home_Type_Enum.Personalized)) &&
                    (pageGlobals.currentMode.Result_Display_Type != Result_Display_Type_Enum.Export) &&
                    ((pageGlobals.currentMode.Mode != Display_Mode_Enum.Item_Display) || ((pageGlobals.currentMode.ViewerCode.Length > 0) && (pageGlobals.currentMode.ViewerCode.ToUpper().IndexOf("citation") < 0) && (pageGlobals.currentMode.ViewerCode.ToUpper().IndexOf("allvolumes3") < 0))))
                {
                    Response.Cache.SetCacheability(HttpCacheability.Private);
                    Response.Cache.SetMaxAge(new TimeSpan(0, SobekCM_Library_Settings.Web_Output_Caching_Minutes, 0));
                }
                else
                {
                    Response.Cache.SetCacheability(HttpCacheability.NoCache);
                }
            }

            // Check if the item nav form should be shown
            if (!pageGlobals.mainWriter.Include_Navigation_Form)
            {
                itemNavForm.Visible = false;
            }
            else
            {
                if (!pageGlobals.mainWriter.Include_Main_Place_Holder)
                    mainPlaceHolder.Visible = false;
                if (!pageGlobals.mainWriter.Include_TOC_Place_Holder)
                    tocPlaceHolder.Visible = false;
            }

            // The file upload form is only shown in these cases
            if (( pageGlobals.mainWriter != null ) && ( pageGlobals.mainWriter.File_Upload_Possible ))
            {
                itemNavForm.Enctype = "multipart/form-data";
            }

            // Add the controls now
            pageGlobals.mainWriter.Add_Controls(tocPlaceHolder, mainPlaceHolder, pageGlobals.tracer);
        }
        catch (OutOfMemoryException ee)
        {
            pageGlobals.tracer.Add_Trace("sobekcm(.aspx).Page_Load", "OutOfMemoryException caught!");

            pageGlobals.Email_Information("SobekCM Out of Memory Exception", ee);
        }
        catch (Exception ee)
        {
            pageGlobals.tracer.Add_Trace("sobekcm(.aspx).Page_Load", "Exception caught!", SobekCM.Library.Custom_Trace_Type_Enum.Error);
            pageGlobals.tracer.Add_Trace("sobekcm(.aspx).Page_Load", ee.Message, SobekCM.Library.Custom_Trace_Type_Enum.Error);
            pageGlobals.tracer.Add_Trace("sobekcm(.aspx).Page_Load", ee.StackTrace, SobekCM.Library.Custom_Trace_Type_Enum.Error);

	        if (pageGlobals.currentMode != null)
	        {
		        pageGlobals.currentMode.Mode = Display_Mode_Enum.Error;
		        pageGlobals.currentMode.Error_Message = "Unknown error caught while executing your request";
		        pageGlobals.currentMode.Caught_Exception = ee;
	        }
        }
    }


    #endregion

    #region Methods called during execution of the HTML from UFDC.aspx

    protected void Write_Page_Title()
    {
        // If the was a very basic error, or the request was complete, do nothing here
        if ((pageGlobals.currentMode == null) || (pageGlobals.currentMode.Request_Completed ))
            return;

        pageGlobals.tracer.Add_Trace("sobekcm(.aspx).Write_Page_Title", String.Empty);

        // Allow the html writer to add its own title 
        if ((pageGlobals.mainWriter.Writer_Type == Writer_Type_Enum.HTML) || (pageGlobals.mainWriter.Writer_Type == Writer_Type_Enum.HTML_LoggedIn))
        {
            Response.Output.Write(((SobekCM.Library.MainWriters.Html_MainWriter)pageGlobals.mainWriter).Get_Page_Title(pageGlobals.tracer));
        }

        // For robot crawlers using the HTML ECHO writer, the title is alway in the info browse mode
        if (pageGlobals.mainWriter.Writer_Type == Writer_Type_Enum.HTML_Echo)
        {
            Response.Output.Write(pageGlobals.currentMode.Info_Browse_Mode);
        }
    }

    protected void Write_Within_HTML_Head()
    {
        // If the was a very basic error, or the request was complete, do nothing here
        if ((pageGlobals.currentMode == null) || (pageGlobals.currentMode.Request_Completed))
            return;

        pageGlobals.tracer.Add_Trace("sobekcm(.aspx).Write_Within_HTML_Head", String.Empty);

        // Only bother writing the style references if this is writing HTML (either logged out or logged in via myUFDC)
        if ((pageGlobals.mainWriter.Writer_Type == Writer_Type_Enum.HTML) || (pageGlobals.mainWriter.Writer_Type == Writer_Type_Enum.HTML_LoggedIn))
        {
            ((SobekCM.Library.MainWriters.Html_MainWriter)pageGlobals.mainWriter).Write_Within_HTML_Head(Response.Output, pageGlobals.tracer);
        }

        // If this is for the robots, add some generic style statements
        if (pageGlobals.mainWriter.Writer_Type == Writer_Type_Enum.HTML_Echo)
        {
            ((SobekCM.Library.MainWriters.Html_Echo_MainWriter)pageGlobals.mainWriter).Write_Within_HTML_Head(Response.Output, pageGlobals.tracer);
        }
    }

    protected void Write_Body_Attributes()
    {
        // If the was a very basic error, or the request was complete, do nothing here
        if ((pageGlobals.currentMode == null) || (pageGlobals.currentMode.Request_Completed))
            return;

        pageGlobals.tracer.Add_Trace("sobekcm(.aspx).Write_Body_Attributes", String.Empty);


        if ((pageGlobals.mainWriter.Writer_Type == Writer_Type_Enum.HTML) || (pageGlobals.mainWriter.Writer_Type == Writer_Type_Enum.HTML_LoggedIn))
        {
            Response.Output.Write(((SobekCM.Library.MainWriters.Html_MainWriter)pageGlobals.mainWriter).Get_Body_Attributes(pageGlobals.tracer));
        }
    }

    protected void Write_Html()
    {
        // If the was a very basic error, or the request was complete, do nothing here
        if ((pageGlobals.currentMode == null) || (pageGlobals.currentMode.Request_Completed))
            return;

        // Add the HTML to the main section (which sits outside any of the standard fors)
        pageGlobals.tracer.Add_Trace("sobekcm(.aspx).Write_HTML", String.Empty);
        pageGlobals.mainWriter.Write_Html(Response.Output, pageGlobals.tracer);
    }

    protected void Write_ItemNavForm_Opening()
    {
        // If the was a very basic error, or the request was complete, do nothing here
        if ((pageGlobals.currentMode == null) || (pageGlobals.currentMode.Request_Completed))
            return;

        if ((pageGlobals.mainWriter.Writer_Type == Writer_Type_Enum.HTML) || (pageGlobals.mainWriter.Writer_Type == Writer_Type_Enum.HTML_LoggedIn))
        {
            pageGlobals.tracer.Add_Trace("sobekcm(.aspx).Write_Additional_HTML", String.Empty);
            ((SobekCM.Library.MainWriters.Html_MainWriter)pageGlobals.mainWriter).Write_ItemNavForm_Opening(Response.Output, pageGlobals.tracer);
        }
    }

    protected void Write_ItemNavForm_Additional_HTML()
    {
        // If the was a very basic error, or the request was complete, do nothing here
        if ((pageGlobals.currentMode == null) || (pageGlobals.currentMode.Request_Completed))
            return;

        if ((pageGlobals.mainWriter.Writer_Type == Writer_Type_Enum.HTML) || (pageGlobals.mainWriter.Writer_Type == Writer_Type_Enum.HTML_LoggedIn))
        {
            pageGlobals.tracer.Add_Trace("sobekcm(.aspx).Write_Additional_HTML", String.Empty);
            ((SobekCM.Library.MainWriters.Html_MainWriter)pageGlobals.mainWriter).Write_Additional_HTML(Response.Output, pageGlobals.tracer);
        }
    }

    protected void Write_ItemNavForm_Closing()
    {


        // If the was a very basic error, or the request was complete, do nothing here
        if ((pageGlobals.currentMode == null) || (pageGlobals.currentMode.Request_Completed))
            return;

        if ((pageGlobals.mainWriter.Writer_Type == Writer_Type_Enum.HTML) || (pageGlobals.mainWriter.Writer_Type == Writer_Type_Enum.HTML_LoggedIn))
        {
            pageGlobals.tracer.Add_Trace("sobekcm(.aspx).Write_Additional_HTML", String.Empty);
            ((SobekCM.Library.MainWriters.Html_MainWriter)pageGlobals.mainWriter).Write_ItemNavForm_Closing(Response.Output, pageGlobals.tracer);
        }
    }

    protected void Write_Final_HTML()
    {
        // If the was a very basic error, or the request was complete, do nothing here
        if ((pageGlobals.currentMode == null) || (pageGlobals.currentMode.Request_Completed))
            return;

        if ((pageGlobals.mainWriter.Writer_Type == Writer_Type_Enum.HTML) || (pageGlobals.mainWriter.Writer_Type == Writer_Type_Enum.HTML_LoggedIn))
        {
            pageGlobals.tracer.Add_Trace("sobekcm(.aspx).Write_Final_HTML", String.Empty);
            ((SobekCM.Library.MainWriters.Html_MainWriter)pageGlobals.mainWriter).Write_Final_HTML(Response.Output, pageGlobals.tracer);
        }
    }


    protected override void OnUnload(EventArgs E)
    {
        if ( HttpContext.Current.Session["Last_Exception"] == null )
            SobekCM.Library.Database.SobekCM_Database.Verify_Item_Lookup_Object(true, ref Global.Item_List, null);

        base.OnUnload(E);
    }

    #endregion



    protected override void OnInit(EventArgs E)
    {
        pageGlobals = new SobekCM_Page_Globals(IsPostBack, "SOBEKCM");

        base.OnInit(E);
    }

    protected void Repository_Title()
    {
        // If the was a very basic error, or the request was complete, do nothing here
        if ((pageGlobals.currentMode == null) || (pageGlobals.currentMode.Request_Completed))
            return;

        if ( !String.IsNullOrEmpty( SobekCM_Library_Settings.System_Name))
            Response.Output.Write(SobekCM_Library_Settings.System_Name + " : SobekCM Digital Repository");
        else
            Response.Output.Write("SobekCM Digital Repository");
    }
}

