using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.UIControls;

public partial class CMSModules_Groups_CMSPages_ForumPostApprove : CMSGroupForumPage
{
    #region "Variables"

    private int postId = 0;
    private int groupId = 0;

    #endregion


    #region "Page events"

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        // Setup the modal dialog
        SetCulture();
        RegisterEscScript();

        // Get post ID
        postId = QueryHelper.GetInteger("postid", 0);
        groupId = QueryHelper.GetInteger("groupid", 0);

        // Set the post ID
        PostApprove.PostID = postId;
        PostApproveFooter.PostID = postId;

        // Set methods which check the permissions
        PostApprove.OnCheckPermissions += new CMSAdminControl.CheckPermissionsEventHandler(PostApprove_OnCheckPermissions);
        PostApproveFooter.OnCheckPermissions += new CMSAdminControl.CheckPermissionsEventHandler(PostApprove_OnCheckPermissions);

        // Page title
        CurrentMaster.Title.TitleText = GetString("forums_forumnewpost_header.preview");
        CurrentMaster.Title.TitleImage = GetImageUrl("Objects/Forums_ForumPost/object.png");
    }


    /// <summary>
    /// Raises the <see cref="E:PreRender"/> event.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Setup the modal dialog
        RegisterModalPageScripts();
    }


    /// <summary>
    /// Check whether user is group administrator or has manage permission.
    /// </summary>
    /// <param name="groupId">Comunnity group ID</param>
    /// <param name="permissionName">Permission name</param>
    private void PostApprove_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        if (permissionType.EqualsCSafe("modify", true))
        {
            permissionType = "Manage";
        }

        // Check permissions
        CheckPermissions(groupId, permissionType);
    }

    #endregion
}