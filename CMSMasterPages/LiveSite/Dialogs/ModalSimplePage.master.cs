using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.UIControls;
using CMS.ExtendedControls;

public partial class CMSMasterPages_LiveSite_Dialogs_ModalSimplePage : CMSLiveMasterPage
{
    /// <summary>
    /// PageTitle control.
    /// </summary>
    public override PageTitle Title
    {
        get
        {
            return titleElem;
        }
    }


    /// <summary>
    /// HeaderActions control.
    /// </summary>
    public override HeaderActions HeaderActions
    {
        get
        {
            if (base.HeaderActions != null)
            {
                return base.HeaderActions;
            }
            return actionsElem;
        }
    }


    /// <summary>
    /// Body panel.
    /// </summary>
    public override Panel PanelBody
    {
        get
        {
            return pnlBody;
        }
    }


    /// <summary>
    /// Body object.
    /// </summary>
    public override HtmlGenericControl Body
    {
        get
        {
            return bodyElem;
        }
    }


    /// <summary>
    /// Gets the labels container.
    /// </summary>
    public override PlaceHolder PlaceholderLabels
    {
        get
        {
            return plcLabels;
        }
    }


    /// <summary>
    /// Prepared for specifying the additional HEAD elements.
    /// </summary>
    public override Literal HeadElements
    {
        get
        {
            return ltlHeadElements;
        }
        set
        {
            ltlHeadElements = value;
        }
    }


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        PageStatusContainer = plcStatus;

        // Set dialog CSS class
        SetDialogClass();
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Display panel with additional controls place holder if required
        if (DisplayControlsPanel)
        {
            pnlAdditionalControls.Visible = true;
        }
        bodyElem.Attributes["class"] = mBodyClass;
    }


    protected override void Render(HtmlTextWriter writer)
    {
        // Hide actions panel if no actions are present and DisplayActionsPanel is false
        if (!DisplayActionsPanel)
        {
            if (!actionsElem.Visible)
            {
                pnlActions.Visible = false;
            }
        } 
        
        base.Render(writer);
    }
}