using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.GlobalHelper;
using CMS.Notifications;
using CMS.UIControls;

public partial class CMSModules_Notifications_Controls_TemplateEdit : CMSUserControl
{
    #region "Variables"

    private int mSiteID = 0;
    private int mTemplateID = 0;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Sets/Gets current SiteID
    /// </summary>
    public int SiteID
    {
        get
        {
            return mSiteID;
        }
        set
        {
            mSiteID = value;
        }
    }


    /// <summary>
    /// Sets/Gets current TemplateID
    /// </summary>
    public int TemplateID
    {
        get
        {
            return mTemplateID;
        }
        set
        {
            mTemplateID = value;
        }
    }

    #endregion


    #region "Events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Show info message info changes were saved
        if (QueryHelper.GetInteger("saved", 0) == 1)
        {
            // Show message
            ShowChangesSaved();

            // Reload header if changes were saved
            ScriptHelper.RefreshTabHeader(Page, null);
        }

        // Get resource strings
        lblDisplayName.ResourceString = "general.displayname";
        lblCodeName.ResourceString = "general.codename";
        valCodeName.Text = GetString("general.requirescodename");
        valDisplayName.Text = GetString("general.requiresdisplayname");

        // Load values
        if (!RequestHelper.IsPostBack() && (TemplateID != 0))
        {
            NotificationTemplateInfo nti = NotificationTemplateInfoProvider.GetNotificationTemplateInfo(TemplateID);
            if (nti != null)
            {
                txtCodeName.Text = nti.TemplateName;
                txtDisplayName.Text = nti.TemplateDisplayName;
            }
        }
    }


    protected void btnOK_Click(object sender, EventArgs e)
    {
        // Load or create new NotificationTemplateInfo
        NotificationTemplateInfo nti = null;

        string target = "";

        if (TemplateID > 0)
        {
            nti = NotificationTemplateInfoProvider.GetNotificationTemplateInfo(TemplateID);

            if (nti == null)
            {
                throw new Exception("Template with given ID not found!");
            }

            target = "Template_Edit_General.aspx";
        }
        else
        {
            nti = new NotificationTemplateInfo();
            nti.TemplateGUID = Guid.NewGuid();
            target = "Template_Edit.aspx";
        }

        // Validate codename
        string error = ValidateForm(nti);
        if ((error != null) && (error != ""))
        {
            // Show error message
            ShowError(GetString(error));

            return;
        }

        // Setup object
        nti.TemplateName = txtCodeName.Text.Trim();
        nti.TemplateDisplayName = txtDisplayName.Text.Trim();
        nti.TemplateSiteID = SiteID;

        // Update db
        NotificationTemplateInfoProvider.SetNotificationTemplateInfo(nti);
        TemplateID = nti.TemplateID;

        //Redirect to edit page       
        URLHelper.Redirect(target + "?saved=1&templateid=" + TemplateID.ToString() + "&siteid=" + SiteID.ToString());
    }


    private string ValidateForm(NotificationTemplateInfo nti)
    {
        string codename = txtCodeName.Text.Trim();

        // Check if display name is not empty
        string result = new Validator().NotEmpty(txtDisplayName.Text.Trim(), "general.requiresdisplayname").Result;
        if (result != "")
        {
            return result;
        }

        // Check if code name is not empty
        result = new Validator().NotEmpty(codename, "general.requiresdisplayname").Result;
        if (result != "")
        {
            return result;
        }

        // Check if code name is valid       
        result = new Validator().IsCodeName(codename, "general.invalidcodename").Result;
        if (result != "")
        {
            return result;
        }

        // Check if codename is unique
        if ((nti.TemplateName != codename))
        {
            NotificationTemplateInfo testNti = NotificationTemplateInfoProvider.GetNotificationTemplateInfo(codename, SiteID);
            if ((testNti != null) && (testNti.TemplateID != nti.TemplateID))
            {
                return "general.uniquecodenameerror";
            }
        }

        return null;
    }

    #endregion
}