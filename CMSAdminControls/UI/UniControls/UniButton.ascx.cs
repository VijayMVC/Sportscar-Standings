﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.UIControls;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.IO;
using CMS.ExtendedControls;

public partial class CMSAdminControls_UI_UniControls_UniButton : UniButton, IPostBackEventHandler
{
    #region "Events"

    /// <summary>
    /// Fires when the button is clicked
    /// </summary>
    public event EventHandler Click;

    #endregion


    #region "Properties"

    /// <summary>
    /// Button control
    /// </summary>
    public CMSButton ButtonControl
    {
        get
        {
            return btn;
        }
    }


    /// <summary>
    /// Specifies whether the control is enabled or not
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return base.Enabled;
        }
        set
        {
            btn.Enabled = value;
            hyperLink.Enabled = value;

            base.Enabled = value;
        }
    }


    #endregion


    #region "Methods"

    protected void ReloadData()
    {
        if (ShowAsButton)
        {
            // Setup as button
            btn.Text = GetText();
            btn.OnClientClick = OnClientClick;
            btn.CssClass = CssClass;

            hyperLink.Visible = false;
        }
        else
        {
            btn.Visible = false;

            // URL
            string url = LinkUrl;
            if (!String.IsNullOrEmpty(url))
            {
                url = URLHelper.ResolveUrl(url);
            }
            hyperLink.NavigateUrl = url;
            hyperLink.CssClass = CssClass;
            hyperLink.Target = LinkTarget;

            string imageUrl = ImageUrl;
            bool isImageUrl = !string.IsNullOrEmpty(imageUrl);

            // Link text
            string text = GetText();
            lblText.Text = (!isImageUrl && string.IsNullOrEmpty(text)) ? url : text;

            // Link javascript
            string javascript = OnClientClick;
            string ev = LinkEvent;
            if (!String.IsNullOrEmpty(ev) || (Click != null))
            {
                javascript += this.Page.ClientScript.GetPostBackEventReference(this, null) + "; return false;";
            }
            if (!String.IsNullOrEmpty(javascript))
            {
                hyperLink.Attributes.Add("onclick", javascript);
            }

            // Image
            if (isImageUrl)
            {
                image.ImageUrl = UIHelper.ResolveImageUrl(imageUrl);
                image.AlternateText = ImageAltText;
                image.ToolTip = ImageAltText;
                image.CssClass = ImageCssClass;
            }
            else
            {
                image.Visible = false;
            }
        }
    }


    /// <summary>
    /// Gets the button text
    /// </summary>
    protected string GetText()
    {
        if (!String.IsNullOrEmpty(LinkText))
        {
            return LinkText;
        }
        else
        {
            return ResHelper.GetString(ResourceString);
        }
    }


    /// <summary>
    /// PreRender event handler
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        ReloadData();
    }


    /// <summary>
    /// Button click handler
    /// </summary>
    public void btn_Click(object sender, EventArgs e)
    {
        RaisePostBackEvent(null);
    }


    /// <summary>
    /// Raises the postback event
    /// </summary>
    /// <param name="eventArgument">Event argument</param>
    public void RaisePostBackEvent(string eventArgument)
    {
        var e = new EventArgs();

        // Raise the click event
        if (Click != null)
        {
            Click(this, e);
        }

        // Raise the requested event
        string ev = this.LinkEvent;
        if (!String.IsNullOrEmpty(ev))
        {
            ComponentEvents.RequestEvents.RaiseEvent(this, e, ev);
        }
    }
    
    #endregion
}