using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.Ecommerce;
using CMS.GlobalHelper;
using CMS.UIControls;
using CMS.SettingsProvider;

// Edited object
[EditedObject(PredefinedObjectType.INTERNALSTATUS, "statusid")]
// Title
[Title("Objects/Ecommerce_InternalStatus/object.png", "InternalStatus_Edit.HeaderCaption", "newedit_internal_status", ExistingObject = true)]
[Title("Objects/Ecommerce_InternalStatus/new.png", "InternalStatus_New.HeaderCaption", "newedit_internal_status", NewObject = true)]
public partial class CMSModules_Ecommerce_Pages_Tools_Configuration_InternalStatus_InternalStatus_Edit : CMSInternalStatusesPage
{
    protected int mStatusId = 0;


    protected void Page_Load(object sender, EventArgs e)
    {
        rfvCodeName.ErrorMessage = GetString("InternalStatus_Edit.errorCodeName");
        rfvDisplayName.ErrorMessage = GetString("InternalStatus_Edit.errorDisplayName");

        // Control initializations				
        lblInternalStatusName.Text = GetString("InternalStatus_Edit.InternalStatusNameLabel");
        lblInternalStatusDisplayName.Text = GetString("InternalStatus_Edit.InternalStatusDisplayNameLabel");

        string currentInternalStatus = GetString("InternalStatus_Edit.NewItemCaption");

        // Get internalStatus id from querystring		
        mStatusId = QueryHelper.GetInteger("statusid", 0);
        if (mStatusId > 0)
        {
            InternalStatusInfo internalStatusObj = EditedObject as InternalStatusInfo;

            if (internalStatusObj != null)
            {
                CheckEditedObjectSiteID(internalStatusObj.InternalStatusSiteID);
                currentInternalStatus = internalStatusObj.InternalStatusDisplayName;

                // fill editing form
                if (!RequestHelper.IsPostBack())
                {
                    LoadData(internalStatusObj);

                    // Show that the internalStatus was created or updated successfully
                    if (QueryHelper.GetString("saved", "") == "1")
                    {
                        // Show message
                        ShowChangesSaved();
                    }
                }
            }
        }

        // Initializes page title breadcrumbs control		
        string[,] pageTitleTabs = new string[2, 3];
        pageTitleTabs[0, 0] = GetString("InternalStatus_Edit.ItemListLink");
        pageTitleTabs[0, 1] = "~/CMSModules/Ecommerce/Pages/Tools/Configuration/InternalStatus/InternalStatus_List.aspx?siteId=" + SiteID;
        pageTitleTabs[0, 2] = "";
        pageTitleTabs[1, 0] = currentInternalStatus;
        pageTitleTabs[1, 1] = "";
        pageTitleTabs[1, 2] = "";
        CurrentMaster.Title.Breadcrumbs = pageTitleTabs;
    }


    /// <summary>
    /// Load data of editing internalStatus.
    /// </summary>
    /// <param name="internalStatusObj">InternalStatus object</param>
    protected void LoadData(InternalStatusInfo internalStatusObj)
    {
        chkInternalStatusEnabled.Checked = internalStatusObj.InternalStatusEnabled;
        txtInternalStatusName.Text = internalStatusObj.InternalStatusName;
        txtInternalStatusDisplayName.Text = internalStatusObj.InternalStatusDisplayName;
    }


    /// <summary>
    /// Sets data to database.
    /// </summary>
    protected void btnOK_Click(object sender, EventArgs e)
    {
        CheckConfigurationModification();

        string errorMessage = new Validator()
            .NotEmpty(txtInternalStatusDisplayName.Text.Trim(), GetString("InternalStatus_Edit.errorDisplayName"))
            .NotEmpty(txtInternalStatusName.Text.Trim(), GetString("InternalStatus_Edit.errorCodeName")).Result;

        if (!ValidationHelper.IsCodeName(txtInternalStatusName.Text.Trim()))
        {
            errorMessage = GetString("General.ErrorCodeNameInIdentifierFormat");
        }

        if (errorMessage == "")
        {
            // Check unique name for configured site
            DataSet ds = InternalStatusInfoProvider.GetInternalStatuses("InternalStatusName = '" + txtInternalStatusName.Text.Trim().Replace("'", "''") + "' AND ISNULL(InternalStatusSiteID, 0) = " + ConfiguredSiteID, null);
            InternalStatusInfo internalStatusObj = null;
            if (!DataHelper.DataSourceIsEmpty(ds))
            {
                internalStatusObj = new InternalStatusInfo(ds.Tables[0].Rows[0]);
            }

            // if internalStatusName value is unique														
            if ((internalStatusObj == null) || (internalStatusObj.InternalStatusID == mStatusId))
            {
                // if internalStatusName value is unique -> determine whether it is update or insert 
                if ((internalStatusObj == null))
                {
                    // get InternalStatusInfo object by primary key
                    internalStatusObj = InternalStatusInfoProvider.GetInternalStatusInfo(mStatusId);
                    if (internalStatusObj == null)
                    {
                        // create new item -> insert
                        internalStatusObj = new InternalStatusInfo();
                        internalStatusObj.InternalStatusSiteID = ConfiguredSiteID;
                    }
                }

                internalStatusObj.InternalStatusEnabled = chkInternalStatusEnabled.Checked;
                internalStatusObj.InternalStatusName = txtInternalStatusName.Text.Trim();
                internalStatusObj.InternalStatusDisplayName = txtInternalStatusDisplayName.Text.Trim();

                InternalStatusInfoProvider.SetInternalStatusInfo(internalStatusObj);

                URLHelper.Redirect("InternalStatus_Edit.aspx?statusid=" + Convert.ToString(internalStatusObj.InternalStatusID) + "&saved=1&siteId=" + SiteID);
            }
            else
            {
                // Show error message
                ShowError(GetString("InternalStatus_Edit.InternalStatusNameExists"));
            }
        }
        else
        {
            // Show error message
            ShowError(errorMessage);
        }
    }
}