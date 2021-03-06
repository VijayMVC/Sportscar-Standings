﻿using System;

using CMS.UIControls;
using CMS.GlobalHelper;
using CMS.PortalEngine;
using CMS.CMSHelper;

public partial class CMSModules_Content_Controls_DeviceView : CMSAdminControl
{
    #region "Variables"

    protected string mViewPage = null;
    protected string framescroll = "auto";

    #endregion


    #region "Properties"

    /// <summary>
    /// If true, device should be displayed rotated.
    /// </summary>
    public bool RotateDevice
    {
        get;
        set;
    }


    /// <summary>
    /// URL of page in device's iframe.
    /// </summary>
    public String ViewPage
    {
        get
        {
            return mViewPage ?? (mViewPage = QueryHelper.GetString("viewpage", string.Empty));
        }
        set
        {
            mViewPage = value;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Register JS
        ScriptHelper.RegisterJQuery(Page);
        ScriptHelper.RegisterJQueryCookie(Page);
        ScriptHelper.RegisterScriptFile(Page, "jquery/jquery-jscrollpane.js");
        CSSHelper.RegisterCSSLink(Page, "~/CMSScripts/jquery/jquery-jscrollpane.css");
        ScriptHelper.RegisterScriptFile(Page, "~/CMSModules/Content/CMSDesk/View/ViewValidate.js");

        // Init preview
        InitializeDevicePreview();
    }


    /// <summary>
    /// Initialize device preview frame.
    /// </summary>
    private void InitializeDevicePreview()
    {
        // Get device ID from query string
        String deviceName = QueryHelper.GetString(DeviceProfileInfoProvider.DEVICENAME_QUERY_PARAM, String.Empty);

        // If device profile not set, use current device profile
        DeviceProfileInfo deviceProfile = CMSContext.CurrentDeviceProfile;

        // Add hash control for X-Frame-Option
        if (deviceName != String.Empty)
        {
            ViewPage = URLHelper.UpdateParameterInUrl(ViewPage, DeviceProfileInfoProvider.DEVICENAME_QUERY_PARAM, deviceName);
            String query = URLHelper.GetQuery(ViewPage);
            string hash = ValidationHelper.GetHashString(query, false);
            ViewPage += String.Format("&clickjackinghash={0}", hash);
        }

        // If device's boundaries are set, use iframe
        if ((deviceProfile != null) && (deviceProfile.ProfilePreviewWidth > 0) && (deviceProfile.ProfilePreviewHeight > 0))
        {
            // Remove frame scrolling
            framescroll = "no";

            // Register device css from site name folder or design folder
            DeviceProfileInfoProvider.RegisterDeviceProfileCss(Page, deviceProfile.ProfileName);

            pnlDevice.CssClass += " " + deviceProfile.ProfileName;
            string deviceScript = String.Format("jQuery(CMSViewValidate.InitializeFrame({0}, {1}, {2}));",
                     deviceProfile.ProfilePreviewWidth,
                     deviceProfile.ProfilePreviewHeight,
                     (RotateDevice ? "true" : "false"));

            ScriptHelper.RegisterStartupScript(this, typeof(string), "InitializeDeviceFrame", deviceScript, true);
        }
        else
        {
            // Hide all device frame divs
            pnlTop.Visible = false;
            pnlBottom.Visible = false;
            pnlLeft.Visible = false;
            pnlRight.Visible = false;
            pnlCenter.CssClass = String.Empty;
            pnlCenterLine.CssClass = String.Empty;
        }
    }

    #endregion
}
