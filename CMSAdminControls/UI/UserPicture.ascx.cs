using System;
using System.Web.UI;

using CMS.GlobalHelper;
using CMS.IO;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.CMSHelper;

public partial class CMSAdminControls_UI_UserPicture : CMSUserControl
{
    #region "Variables"

    private int mWidth = 0;
    private int mHeight = 0;
    private bool mDisplayPicture = true;
    private int mUserId = 0;
    private int mGroupId = 0;
    private int mAvatarID = 0;
    private bool mKeepAspectRatio = false;
    private bool mRenderOuterDiv = false;
    private string mOuterDivCSSClass = "UserPicture";
    private string mUserAvatarType = AvatarInfoProvider.AVATAR;
    private string mUserEmail = "";


    protected string imageUrl = "";
    public string width = "";
    public string height = "";

    #endregion


    #region "Public properties"

    /// <summary>
    /// Keep aspect ratio.
    /// </summary>
    public bool KeepAspectRatio
    {
        get
        {
            return mKeepAspectRatio;
        }
        set
        {
            mKeepAspectRatio = value;
        }
    }


    /// <summary>
    /// Max picture width.
    /// </summary>
    public int Width
    {
        get
        {
            return mWidth > 0 ? mWidth : SettingsKeyProvider.GetIntValue(CMSContext.CurrentSiteName + ".CMSAvatarWidth"); ;
        }
        set
        {
            mWidth = value;
        }
    }


    /// <summary>
    /// Max picture height.
    /// </summary>
    public int Height
    {
        get
        {
            return mHeight > 0 ? mHeight : SettingsKeyProvider.GetIntValue(CMSContext.CurrentSiteName + ".CMSAvatarHeight"); ;
        }
        set
        {
            mHeight = value;
        }
    }


    /// <summary>
    /// Enable/disable display picture
    /// </summary>
    public bool DisplayPicture
    {
        get
        {
            return mDisplayPicture;
        }
        set
        {
            mDisplayPicture = value;
        }
    }


    /// <summary>
    /// User ID.
    /// </summary>
    public int UserID
    {
        get
        {
            return mUserId;
        }
        set
        {
            mUserId = value;
        }
    }


    /// <summary>
    /// Gets or sets group id.
    /// </summary>
    public int GroupID
    {
        get
        {
            return mGroupId;
        }
        set
        {
            mGroupId = value;
        }
    }


    /// <summary>
    /// Gets or sets avatar id.
    /// </summary>
    public int AvatarID
    {
        get
        {
            return mAvatarID;
        }
        set
        {
            mAvatarID = value;
        }
    }


    /// <summary>
    /// Gets or sets user avatar type
    /// </summary>
    public string UserAvatarType
    {
        get
        {
            return mUserAvatarType;
        }
        set
        {
            mUserAvatarType = value;
        }
    }


    /// <summary>
    /// Gets or sets user e-mail
    /// </summary>
    public string UserEmail
    {
        get
        {
            return mUserEmail;
        }
        set
        {
            mUserEmail = value;
        }
    }


    /// <summary>
    /// Div tag is rendered around picture if true (default value = 'false').
    /// </summary>
    public bool RenderOuterDiv
    {
        get
        {
            return mRenderOuterDiv;
        }
        set
        {
            mRenderOuterDiv = value;
        }
    }


    /// <summary>
    /// CSS class of outer div (default value = 'UserPicture').
    /// </summary>
    public string OuterDivCSSClass
    {
        get
        {
            return mOuterDivCSSClass;
        }
        set
        {
            mOuterDivCSSClass = value;
        }
    }


    /// <summary>
    /// Use default avatar if any user/group avatar not found
    /// </summary>
    public bool UseDefaultAvatar
    {
        get;
        set;
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
    }


    /// <summary>
    /// Sets image  url, width and height.
    /// </summary>
    protected void SetImage()
    {
        Visible = false;

        // Only if display picture is allowed
        if (DisplayPicture)
        {
            string imageUrl = ResolveUrl("~/CMSModules/Avatars/CMSPages/GetAvatar.aspx?avatarguid=");

            // Is user id set? => Get user info
            if (mUserId > 0)
            {
                // Get user info
                UserInfo ui = UserInfoProvider.GetUserInfo(mUserId);
                if (ui != null)
                {
                    switch (UserAvatarType)
                    {
                        case AvatarInfoProvider.AVATAR:
                            AvatarID = ui.UserAvatarID;
                            if (AvatarID <= 0) // Backward compatibility
                            {
                                if (ui.UserPicture != "")
                                {
                                    // Get picture filename
                                    string filename = ui.UserPicture.Remove(ui.UserPicture.IndexOfCSafe('/'));
                                    string ext = Path.GetExtension(filename);
                                    imageUrl += filename.Substring(0, (filename.Length - ext.Length));
                                    imageUrl += "&extension=" + ext;
                                    Visible = true;
                                }
                                else if (UseDefaultAvatar)
                                {
                                    UserGenderEnum gender = (UserGenderEnum)ValidationHelper.GetInteger(ui.UserSettings.UserGender, 0);
                                    AvatarInfo ai = AvatarInfoProvider.GetDefaultAvatar(gender);

                                    if (ai != null)
                                    {
                                        AvatarID = ai.AvatarID;
                                    }
                                }
                            }
                            break;

                        case AvatarInfoProvider.GRAVATAR:
                            int sideSize = mWidth > 0 ? mWidth : SettingsKeyProvider.GetIntValue(CMSContext.CurrentSiteName + ".CMSAvatarMaxSideSize");
                            imageUrl = AvatarInfoProvider.CreateGravatarLink(ui.Email, ui.UserSettings.UserGender, ResolveUrl("~/CMSModules/Avatars/CMSPages/GetAvatar.aspx?maxsidesize=" + sideSize + "&avatarguid="), sideSize, CMSContext.CurrentSiteName);
                            Visible = true;
                            AvatarID = 0;
                            break;
                    }
                }

            }
            else
            {
                // If user is public try get his gravatar
                if (UserAvatarType == AvatarInfoProvider.GRAVATAR)
                {
                    int sideSize = mWidth > 0 ? mWidth : SettingsKeyProvider.GetIntValue(CMSContext.CurrentSiteName + ".CMSAvatarMaxSideSize");
                    imageUrl = AvatarInfoProvider.CreateGravatarLink(UserEmail, (int)UserGenderEnum.Unknown, ResolveUrl("~/CMSModules/Avatars/CMSPages/GetAvatar.aspx?maxsidesize=" + sideSize + "&avatarguid="), sideSize, CMSContext.CurrentSiteName);
                    Visible = true;
                    AvatarID = 0;

                }
            }

            // Is group id set? => Get group info
            if (mGroupId > 0)
            {
                // Get group info trough module commands
                GeneralizedInfo gi = ModuleCommands.CommunityGetGroupInfo(mGroupId);
                if (gi != null)
                {
                    AvatarID = ValidationHelper.GetInteger(gi.GetValue("GroupAvatarID"), 0);
                }

                if ((AvatarID <= 0) && UseDefaultAvatar)
                {
                    AvatarInfo ai = AvatarInfoProvider.GetDefaultAvatar(DefaultAvatarTypeEnum.Group);
                    if (ai != null)
                    {
                        AvatarID = ai.AvatarID;
                    }
                }
            }

            if (AvatarID > 0)
            {
                AvatarInfo ai = AvatarInfoProvider.GetAvatarInfoWithoutBinary(AvatarID);
                if (ai != null)
                {
                    imageUrl += ai.AvatarGUID.ToString();
                    Visible = true;
                }
            }


            // If item was found 
            if (Visible)
            {
                if (KeepAspectRatio)
                {
                    imageUrl += "&maxsidesize=" + (Width > Height ? Width : Height);
                }
                else
                {
                    imageUrl += "&width=" + Width + "&height=" + Height;
                }
                imageUrl = URLHelper.EncodeQueryString(imageUrl);
                ltlImage.Text = "<img alt=\"" + GetString("general.avatar") + "\" src=\"" + imageUrl + "\" />";

                // Render outer div with specific CSS class
                if (RenderOuterDiv)
                {
                    ltlImage.Text = "<div class=\"" + OuterDivCSSClass + "\">" + ltlImage.Text + "</div>";
                }
            }
        }
    }


    /// <summary>
    /// Render.
    /// </summary>
    protected override void Render(HtmlTextWriter writer)
    {
        if (DisplayPicture)
        {
            SetImage();
        }
        else
        {
            Visible = false;
        }

        base.Render(writer);
    }

}