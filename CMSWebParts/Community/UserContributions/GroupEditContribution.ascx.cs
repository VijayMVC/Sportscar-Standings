using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.CMSHelper;
using CMS.Community;
using CMS.GlobalHelper;
using CMS.PortalControls;
using CMS.DocumentEngine;
using CMS.UIControls;
using CMS.URLRewritingEngine;

using TreeNode = CMS.DocumentEngine.TreeNode;

public partial class CMSWebParts_Community_UserContributions_GroupEditContribution : CMSAbstractWebPart
{
    #region "Public properties"

    /// <summary>
    /// Gets or sets the value that indicates whether deleting document is allowed.
    /// </summary>
    public bool AllowDelete
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AllowDelete"), editForm.AllowDelete);
        }
        set
        {
            SetValue("AllowDelete", value);
            editForm.AllowDelete = value;
        }
    }


    /// <summary>
    /// Gets or sets group of users which can work with the documents.
    /// </summary>
    public UserContributionAllowUserEnum AllowUsers
    {
        get
        {
            return (UserContributionAllowUserEnum)(ValidationHelper.GetInteger(GetValue("AllowUsers"), 2));
        }
        set
        {
            SetValue("AllowUsers", Convert.ToInt32(value));
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether the permissions are checked.
    /// </summary>
    public bool CheckPermissions
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("CheckPermissions"), true);
        }
        set
        {
            SetValue("CheckPermissions", value);
            editForm.CheckPermissions = value;
        }
    }


    /// <summary>
    /// Indicates whether group permissions should be checked.
    /// </summary>
    public bool CheckGroupPermissions
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("CheckGroupPermissions"), true);
        }
        set
        {
            SetValue("CheckGroupPermissions", value);
        }
    }


    /// <summary>
    /// Gets or sets alternative form name.
    /// </summary>
    public string AlternativeFormName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("AlternativeFormName"), editForm.AlternativeFormName);
        }
        set
        {
            SetValue("AlternativeFormName", value);
        }
    }


    /// <summary>
    /// Gets or sets the message which is displayed after validation failed.
    /// </summary>
    public string ValidationErrorMessage
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ValidationErrorMessage"), editForm.ValidationErrorMessage);
        }
        set
        {
            SetValue("ValidationErrorMessage", value);
        }
    }


    /// <summary>
    /// Gets or sets edit button label.
    /// </summary>
    public string EditButtonText
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("EditButtonText"), "general.edit");
        }
        set
        {
            SetValue("EditButtonText", value);
            btnEdit.ResourceString = value;
        }
    }


    /// <summary>
    /// Gets or sets delete button label.
    /// </summary>
    public string DeleteButtonText
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("DeleteButtonText"), "general.delete");
        }
        set
        {
            SetValue("DeleteButtonText", value);
            btnDelete.ResourceString = value;
        }
    }


    /// <summary>
    /// Gets or sets close edit mode button label.
    /// </summary>
    public string CloseEditModeButtonText
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("CloseEditModeButtonText"), "EditContribution.CloseButton");
        }
        set
        {
            SetValue("CloseEditModeButtonText", value);
        }
    }


    /// <summary>
    /// Gets or sets value that indicates whether logging activity is performed.
    /// </summary>
    public bool LogActivity
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("LogActivity"), false);
        }
        set
        {
            SetValue("LogActivity", value);
            editForm.LogActivity = value;
        }
    }

    #endregion


    #region "Document properties"

    /// <summary>
    /// Gets or sets the culture version of the displayed content.
    /// </summary>
    public string CultureCode
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("CultureCode"), ""), CMSContext.CurrentUser.PreferredCultureCode);
        }
        set
        {
            SetValue("CultureCode", value);
        }
    }


    /// <summary>
    /// Gets or sets the path to the document.
    /// </summary>
    public string Path
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("Path"), ""), CMSContext.CurrentAliasPath);
        }
        set
        {
            SetValue("Path", value);
        }
    }


    /// <summary>
    /// Gets or sets the codename of the site from which you want to display the content.
    /// </summary>
    public string SiteName
    {
        get
        {
            return DataHelper.GetNotEmpty(ValidationHelper.GetString(GetValue("SiteName"), ""), CMSContext.CurrentSiteName);
        }
        set
        {
            SetValue("SiteName", value);
        }
    }

    #endregion


    /// <summary>
    /// Content loaded event handler.
    /// </summary>
    public override void OnContentLoaded()
    {
        base.OnContentLoaded();
        SetupControl();
    }


    /// <summary>
    /// Initializes the control properties.
    /// </summary>
    protected void SetupControl()
    {
        if (StopProcessing)
        {
            editForm.StopProcessing = true;
            editForm.Visible = false;
        }
        else
        {
            pnlEdit.Visible = false;

            CurrentUserInfo currentUser = CMSContext.CurrentUser;

            // Get the document
            TreeNode node = TreeHelper.GetDocument(SiteName, CMSContext.ResolveCurrentPath(Path), CultureCode, false, null, false, CheckPermissions, currentUser);
            if (node != null)
            {
                bool authorized = false;

                // Check allowed users
                switch (AllowUsers)
                {
                    case UserContributionAllowUserEnum.All:
                        authorized = true;
                        break;

                    case UserContributionAllowUserEnum.Authenticated:
                        authorized = currentUser.IsAuthenticated();
                        break;

                    case UserContributionAllowUserEnum.DocumentOwner:
                        authorized = (node.NodeOwner == currentUser.UserID);
                        break;
                }

                bool authorizedDelete = authorized;

                // Check control access permission
                if (authorized && CheckPermissions)
                {
                    // Node owner has always permission
                    if (node.NodeOwner != currentUser.UserID)
                    {
                        authorized &= (currentUser.IsAuthorizedPerDocument(node, new NodePermissionsEnum[] { NodePermissionsEnum.Read, NodePermissionsEnum.Modify }) == AuthorizationResultEnum.Allowed);
                        authorizedDelete &= (currentUser.IsAuthorizedPerDocument(node, new NodePermissionsEnum[] { NodePermissionsEnum.Read, NodePermissionsEnum.Delete }) == AuthorizationResultEnum.Allowed);
                    }
                }

                // Check group permissions
                authorized &= CheckGroupPermission("editpages");
                authorizedDelete &= CheckGroupPermission("deletepages");
                // Global admin has always permission
                authorized |= currentUser.IsGlobalAdministrator;
                authorizedDelete |= currentUser.IsGlobalAdministrator;

                // Do not allow edit for virtual user
                if (currentUser.IsVirtual)
                {
                    authorized = false;
                    authorizedDelete = false;
                }

                // Display form if authorized
                if (authorized || authorizedDelete)
                {
                    pnlEdit.Visible = true;

                    // Set visibility of edit and delete buttons
                    btnEdit.Visible = btnEdit.Visible && authorized;
                    btnDelete.Visible = btnDelete.Visible && AllowDelete && authorizedDelete;

                    if ((!RequestHelper.IsPostBack()) && ((btnEdit.Text.Trim() == string.Empty) || (btnDelete.Text.Trim() == string.Empty)))
                    {
                        // Initialize labels and css classes
                        btnEdit.ResourceString = EditButtonText;
                        btnEdit.CssClass = "EditContributionEdit";
                        btnDelete.ResourceString = DeleteButtonText;
                        btnDelete.CssClass = "EditContributionDelete";
                    }

                    editForm.ComponentName = WebPartID;
                    editForm.LogActivity = LogActivity;

                    if (pnlForm.Visible)
                    {
                        editForm.StopProcessing = false;
                        editForm.AllowDelete = AllowDelete && CheckGroupPermission("deletepages");
                        editForm.CheckPermissions = CheckPermissions;
                        editForm.NodeID = node.NodeID;
                        editForm.SiteName = SiteName;
                        editForm.CultureCode = CultureCode;
                        editForm.AlternativeFormName = AlternativeFormName;
                        editForm.ValidationErrorMessage = ValidationErrorMessage;
                        editForm.CMSForm.IsLiveSite = true;

                        editForm.OnAfterApprove += editForm_OnAfterChange;
                        editForm.OnAfterReject += editForm_OnAfterChange;
                        editForm.OnAfterDelete += editForm_OnAfterChange;
                        editForm.CMSForm.OnAfterSave += CMSForm_OnAfterSave;

                        // Reload data
                        editForm.ReloadData(false);
                    }
                }
            }
        }
    }


    /// <summary>
    /// Reloads the data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();
        SetupControl();
    }


    /// <summary>
    /// On btnEdit click event handler.
    /// </summary>
    protected void btnEdit_Click(object sender, EventArgs e)
    {
        // Close edit form
        if (pnlForm.Visible)
        {
            pnlForm.Visible = false;
            btnDelete.Visible = true;

            // Refresh current page
            URLHelper.Redirect(URLRewriter.RawUrl);
        }
        // Show edit form
        else
        {
            editForm.Action = "edit";
            pnlForm.Visible = true;
            btnDelete.Visible = false;

            btnEdit.ResourceString = CloseEditModeButtonText;
            btnEdit.CssClass = "EditContributionClose";
        }

        ReloadData();
    }


    /// <summary>
    /// On btnDelete click event handler.
    /// </summary>
    protected void btnDelete_Click(object sender, EventArgs e)
    {
        // Close delete form
        if (pnlForm.Visible)
        {
            pnlForm.Visible = false;
            btnEdit.Visible = true;

            btnDelete.ResourceString = DeleteButtonText;
            btnDelete.CssClass = "EditContributionDelete";
            btnEdit.ResourceString = EditButtonText;
            btnEdit.CssClass = "EditContributionEdit";
        }
        // Show delete form
        else
        {
            editForm.Action = "delete";
            pnlForm.Visible = true;

            btnEdit.Visible = false;
            btnDelete.ResourceString = CloseEditModeButtonText;
            btnDelete.CssClass = "EditContributionClose";
        }

        ReloadData();
    }


    /// <summary>
    /// Returns true if group permissions should be checked and specified permission is allowed in current group.
    /// Also returns true if group permissions should not be checked.
    /// </summary>
    /// <param name="permissionName">Permission to check (createpages, editpages, deletepages)</param>
    protected bool CheckGroupPermission(string permissionName)
    {
        if (CheckGroupPermissions)
        {
            if (CommunityContext.CurrentGroup != null)
            {
                return (GroupInfoProvider.CheckPermission(permissionName, CommunityContext.CurrentGroup.GroupID) || CMSContext.CurrentUser.IsGroupAdministrator(CommunityContext.CurrentGroup.GroupID));
            }

            return false;
        }

        return true;
    }


    /// <summary>
    /// EditForm after change event handler.
    /// </summary>
    private void editForm_OnAfterChange(object sender, EventArgs e)
    {
        CMSForm_OnAfterSave(sender, e);
    }


    /// <summary>
    /// CMSForm after save event handler.
    /// </summary>
    private void CMSForm_OnAfterSave(object sender, EventArgs e)
    {
        if (!StandAlone)
        {
            // Reload data after saving the document
            PagePlaceholder.ClearCache();
            PagePlaceholder.ReloadData();
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        if (!pnlEdit.Visible)
        {
            // Hide control
            Visible = false;
        }
        base.OnPreRender(e);
    }
}