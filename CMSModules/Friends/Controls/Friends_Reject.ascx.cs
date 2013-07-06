using System;

using CMS.Community;
using CMS.GlobalHelper;
using CMS.ExtendedControls;

public partial class CMSModules_Friends_Controls_Friends_Reject : FriendsActionControl
{
    #region "Public properties"

    /// <summary>
    /// Messages placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMess;
        }
    }


    /// <summary>
    /// Indicates if control is used on live site.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return base.IsLiveSite;
        }
        set
        {
            plcMess.IsLiveSite = value;
            base.IsLiveSite = value;
        }
    }

    #endregion


    #region "Button handling"

    protected void btnReject_Click(object senderObject, EventArgs e)
    {
        RaiseOnCheckPermissions(PERMISSION_MANAGE, this);

        // Set up control
        Comment = txtComment.Text;
        SendMail = chkSendEmail.Checked;
        SendMessage = chkSendMessage.Checked;

        string errorMessage = PerformAction(FriendsActionEnum.Reject);
        if (String.IsNullOrEmpty(errorMessage))
        {
            // Register wopener script
            ScriptHelper.RegisterWOpenerScript(Page);

            btnReject.Enabled = false;
            txtComment.Enabled = false;
            chkSendEmail.Enabled = false;
            chkSendMessage.Enabled = false;
            btnCancel.ResourceString = "general.close";
            ShowConfirmation(GetString("friends.friendshiprejected"));
            btnCancel.OnClientClick = "if((wopener != null) && (wopener.refreshList != null)){wopener.refreshList();}CloseDialog();return false;";
        }
        else
        {
            ShowError(errorMessage);
        }
    }

    #endregion


    /// <summary>
    /// Formats username of sender and recipients.
    /// </summary>
    public override string GetFormattedUserName(string userName, string fullName, string nickName)
    {
        return Functions.GetFormattedUserName(userName, fullName, nickName, IsLiveSite);
    }
}