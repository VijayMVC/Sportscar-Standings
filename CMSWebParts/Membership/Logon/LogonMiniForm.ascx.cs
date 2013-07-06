using System;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;

using CMS.CMSHelper;
using CMS.ExtendedControls;
using CMS.GlobalHelper;
using CMS.PortalControls;
using CMS.PortalEngine;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.URLRewritingEngine;
using CMS.WebAnalytics;

using TreeNode = CMS.DocumentEngine.TreeNode;

public partial class CMSWebParts_Membership_Logon_LogonMiniForm : CMSAbstractWebPart, ICallbackEventHandler
{
    #region "Local variables"

    private TextBox user = null;
    private TextBox pass = null;
    private LocalizedButton login = null;
    private LocalizedLabel lblUserName = null;
    private LocalizedLabel lblPassword = null;
    private ImageButton loginImg = null;
    private RequiredFieldValidator rfv = null;
    private Panel container = null;
    private string mDefaultTargetUrl = string.Empty;
    private string mUserNameText = String.Empty;
    private bool mShowUserNameLabel = false;
    private bool mShowPasswordLabel = false;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets the value that indicates if the username label should be displayed.
    /// </summary>
    public bool ShowUserNameLabel
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowUserNameLabel"), mShowUserNameLabel);
        }
        set
        {
            SetValue("ShowUserNameLabel", value);
            mShowUserNameLabel = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates if the password label should be displayed.
    /// </summary>
    public bool ShowPasswordLabel
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowPasswordLabel"), mShowPasswordLabel);
        }
        set
        {
            SetValue("ShowPasswordLabel", value);
            mShowPasswordLabel = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether image button is displayed instead of regular button.
    /// </summary>
    public bool ShowImageButton
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowImageButton"), false);
        }
        set
        {
            SetValue("ShowImageButton", value);
            login.Visible = !value;
            loginImg.Visible = value;
        }
    }


    /// <summary>
    /// Gets or sets an Image button URL.
    /// </summary>
    public string ImageUrl
    {
        get
        {
            return ResolveUrl(ValidationHelper.GetString(GetValue("ImageUrl"), loginImg.ImageUrl));
        }
        set
        {
            SetValue("ImageUrl", value);
            loginImg.ImageUrl = value;
        }
    }


    /// <summary>
    /// Gets or sets the logon failure text.
    /// </summary>
    public string FailureText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("FailureText"), string.Empty);
        }
        set
        {
            if (!string.IsNullOrEmpty(value.Trim()))
            {
                SetValue("FailureText", value);
                loginElem.FailureText = value;
            }
        }
    }


    /// <summary>
    /// Gets or sets the default target url (rediredction when the user is logged in).
    /// </summary>
    public string DefaultTargetUrl
    {
        get
        {
            return ValidationHelper.GetString(GetValue("DefaultTargetUrl"), mDefaultTargetUrl);
        }
        set
        {
            SetValue("DefaultTargetUrl", value);
            mDefaultTargetUrl = value;
        }
    }


    /// <summary>
    /// Gets or sets the username text.
    /// </summary>
    public string UserNameText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("UserNameText"), mUserNameText);
        }
        set
        {
            if (value.Trim() != string.Empty)
            {
                SetValue("UserNameText", value);
                mUserNameText = value;
            }
        }
    }


    /// <summary>
    /// Gets or sets whether show error as popup window.
    /// </summary>
    public bool ErrorAsPopup
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ErrorAsPopup"), false);
        }
        set
        {
            SetValue("ErrorAsPopup", value);
        }
    }


    /// <summary>
    /// Gets or sets whether make login persistent.
    /// </summary>
    public bool PersistentLogin
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("PersistentLogin"), false);
        }
        set
        {
            SetValue("PersistentLogin", value);
        }
    }

    #endregion


    #region "Overridden methods"

    /// <summary>
    /// Applies given stylesheet skin.
    /// </summary>
    public override void ApplyStyleSheetSkin(Page page)
    {
        SetSkinID(SkinID);
        base.ApplyStyleSheetSkin(page);
    }


    /// <summary>
    /// Content loaded event handler.
    /// </summary>
    public override void OnContentLoaded()
    {
        base.OnContentLoaded();
        SetupControl();
    }


    /// <summary>
    /// Reloads data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();
        SetupControl();
    }


    /// <summary>
    /// Pre render event handler.
    /// </summary>
    /// <param name="e">Event arguments</param>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Hide webpart for non-public users
        Visible &= CMSContext.CurrentUser.IsPublic();
    }

    #endregion


    #region "SetupControl and SetSkinID"

    /// <summary>
    /// Initializes the control properties.
    /// </summary>
    protected void SetupControl()
    {
        if (StopProcessing)
        {
            // Do nothing
        }
        else
        {
            // WAI validation
            lblUserName = (LocalizedLabel)loginElem.FindControl("lblUserName");
            if (lblUserName != null)
            {
                lblUserName.Text = GetString("general.username");
                if (!ShowUserNameLabel)
                {
                    lblUserName.Attributes.Add("style", "display: none;");
                }
            }
            lblPassword = (LocalizedLabel)loginElem.FindControl("lblPassword");
            if (lblPassword != null)
            {
                lblPassword.Text = GetString("general.password");
                if (!ShowPasswordLabel)
                {
                    lblPassword.Attributes.Add("style", "display: none;");
                }
            }

            // Set properties for validator
            rfv = (RequiredFieldValidator)loginElem.FindControl("rfvUserNameRequired");
            rfv.ErrorMessage = GetString("edituser.erroremptyusername");
            rfv.ToolTip = GetString("edituser.erroremptyusername");
            rfv.ValidationGroup = ClientID + "_MiniLogon";

            // Set failure text
            if (!string.IsNullOrEmpty(FailureText))
            {
                loginElem.FailureText = ResHelper.LocalizeString(FailureText);
            }
            else
            {
                loginElem.FailureText = GetString("Login_FailureText");
            }

            // Set visibility of buttons
            login = (LocalizedButton)loginElem.FindControl("btnLogon");
            if (login != null)
            {
                login.Visible = !ShowImageButton;
                login.ValidationGroup = ClientID + "_MiniLogon";
            }

            loginImg = (ImageButton)loginElem.FindControl("btnImageLogon");
            if (loginImg != null)
            {
                loginImg.Visible = ShowImageButton;
                loginImg.ImageUrl = ImageUrl;
                loginImg.ValidationGroup = ClientID + "_MiniLogon";
            }

            // Ensure display control as inline and is used right default button
            container = (Panel)loginElem.FindControl("pnlLogonMiniForm");
            if (container != null)
            {
                container.Attributes.Add("style", "display: inline;");
                if (ShowImageButton)
                {
                    if (loginImg != null)
                    {
                        container.DefaultButton = loginImg.ID;
                    }
                    else if (login != null)
                    {
                        container.DefaultButton = login.ID;
                    }
                }
            }

            CMSTextBox txtUserName = (CMSTextBox)loginElem.FindControl("UserName");
            if (txtUserName != null)
            {
                txtUserName.EnableAutoComplete = SecurityHelper.IsAutoCompleteEnabledForLogin(CMSContext.CurrentSiteName);
            }

            if (!string.IsNullOrEmpty(UserNameText))
            {
                // Initialize javascript for focus and blur UserName textbox
                user = (TextBox)loginElem.FindControl("UserName");
                user.Attributes.Add("onfocus", "MLUserFocus_" + ClientID + "('focus');");
                user.Attributes.Add("onblur", "MLUserFocus_" + ClientID + "('blur');");
                string focusScript = "function MLUserFocus_" + ClientID + "(type)" +
                                     "{" +
                                     "var userNameBox = document.getElementById('" + user.ClientID + "');" +
                                     "if(userNameBox.value == '" + UserNameText + "' && type == 'focus')" +
                                     "{userNameBox.value = '';}" +
                                     "else if (userNameBox.value == '' && type == 'blur')" +
                                     "{userNameBox.value = '" + UserNameText + "';}" +
                                     "}";

                ScriptHelper.RegisterClientScriptBlock(this, GetType(), "MLUserNameFocus_" + ClientID,
                                                       ScriptHelper.GetScript(focusScript));
            }
            loginElem.LoggedIn += loginElem_LoggedIn;
            loginElem.LoggingIn += loginElem_LoggingIn;
            loginElem.LoginError += loginElem_LoginError;

            if (!RequestHelper.IsPostBack())
            {
                // Set SkinID properties
                if (!StandAlone && (PageCycle < PageCycleEnum.Initialized) && (ValidationHelper.GetString(Page.StyleSheetTheme, string.Empty) == string.Empty))
                {
                    SetSkinID(SkinID);
                }
            }

            if (string.IsNullOrEmpty(loginElem.UserName))
            {
                loginElem.UserName = UserNameText;
            }

            // Register script to update logon error message
            Label failureLit = loginElem.FindControl("FailureText") as Label;
            if (failureLit != null)
            {
                StringBuilder sbScript = new StringBuilder();
                sbScript.Append(@"
function UpdateLabel_", ClientID, @"(content, context) {
    var lbl = document.getElementById(context);
    if(lbl)
    {
        lbl.innerHTML = content;
        lbl.className = ""InfoLabel"";      
    }
}");
                ScriptHelper.RegisterClientScriptBlock(this, GetType(), "InvalidLogonAttempts_" + ClientID, sbScript.ToString(), true);
            }
        }
    }


    /// <summary>
    /// Sets SkinId to all controls in logon form.
    /// </summary>
    private void SetSkinID(string skinId)
    {
        if (skinId != string.Empty)
        {
            loginElem.SkinID = skinId;

            user = (TextBox)loginElem.FindControl("UserName");
            if (user != null)
            {
                user.SkinID = skinId;
            }

            pass = (TextBox)loginElem.FindControl("Password");
            if (pass != null)
            {
                pass.SkinID = skinId;
            }

            login = (LocalizedButton)loginElem.FindControl("btnLogon");
            if (login != null)
            {
                login.SkinID = skinId;
            }

            loginImg = (ImageButton)loginElem.FindControl("btnImageLogon");
            if (loginImg != null)
            {
                loginImg.SkinID = skinId;
            }
        }
    }

    #endregion


    #region "Logging handlers"

    /// <summary>
    /// Logged in handler.
    /// </summary>
    private void loginElem_LoggedIn(object sender, EventArgs e)
    {
        // Set view mode to live site after login to prevent bar with "Close preview mode"
        CMSContext.ViewMode = ViewModeEnum.LiveSite;

        // Ensure response cookie
        CookieHelper.EnsureResponseCookie(FormsAuthentication.FormsCookieName);

        // Set cookie expiration
        if (loginElem.RememberMeSet)
        {
            CookieHelper.ChangeCookieExpiration(FormsAuthentication.FormsCookieName, DateTime.Now.AddYears(1), false);
        }
        else
        {
            // Extend the expiration of the authentication cookie if required
            if (!AuthenticationHelper.UseSessionCookies && (HttpContext.Current != null) && (HttpContext.Current.Session != null))
            {
                CookieHelper.ChangeCookieExpiration(FormsAuthentication.FormsCookieName, DateTime.Now.AddMinutes(Session.Timeout), false);
            }
        }

        // Current username
        string userName = loginElem.UserName;

        // Get user name (test site prefix too)
        UserInfo ui = UserInfoProvider.GetUserInfoForSitePrefix(userName, CMSContext.CurrentSite);

        // Check whether safe user name is required and if so get safe username
        if (RequestHelper.IsMixedAuthentication() && UserInfoProvider.UseSafeUserName)
        {
            // User stored with safe name
            userName = ValidationHelper.GetSafeUserName(loginElem.UserName, CMSContext.CurrentSiteName);

            // Find user by safe name
            ui = UserInfoProvider.GetUserInfoForSitePrefix(userName, CMSContext.CurrentSite);
            if (ui != null)
            {
                // Authenticate user by site or global safe username
                CMSContext.AuthenticateUser(ui.UserName, loginElem.RememberMeSet);
            }
        }

        // Log activity (warning: CMSContext contains info of previous user)
        if (ui != null)
        {
            // If user name is site prefixed, authenticate user manually 
            if (UserInfoProvider.IsSitePrefixedUser(ui.UserName))
            {
                CMSContext.AuthenticateUser(ui.UserName, loginElem.RememberMeSet);
            }

            // Log activity
            int contactID = ModuleCommands.OnlineMarketingGetUserLoginContactID(ui);
            Activity activityLogin = new ActivityUserLogin(contactID, ui, CMSContext.CurrentDocument, CMSContext.ActivityEnvironmentVariables);
            activityLogin.Log();
        }

        // Redirect user to the return url, or if is not defined redirect to the default target url
        string url = QueryHelper.GetString("ReturnURL", string.Empty);
        if (!string.IsNullOrEmpty(url))
        {
            if (url.StartsWithCSafe("~") || url.StartsWithCSafe("/") || QueryHelper.ValidateHash("hash"))
            {
                URLHelper.Redirect(ResolveUrl(ValidationHelper.GetString(Request.QueryString["ReturnURL"], "")));
            }
            else
            {
                URLHelper.Redirect(ResolveUrl("~/CMSMessages/Error.aspx?title=" + ResHelper.GetString("general.badhashtitle") + "&text=" + ResHelper.GetString("general.badhashtext")));
            }
        }
        else
        {
            if (DefaultTargetUrl != "")
            {
                URLHelper.Redirect(ResolveUrl(DefaultTargetUrl));
            }
            else
            {
                URLHelper.Redirect(URLRewriter.CurrentURL);
            }
        }
    }


    /// <summary>
    /// Logging in handler.
    /// </summary>
    private void loginElem_LoggingIn(object sender, LoginCancelEventArgs e)
    {
        // Ban IP addresses which are blocked for login
        if (!BannedIPInfoProvider.IsAllowed(CMSContext.CurrentSiteName, BanControlEnum.Login))
        {
            e.Cancel = true;

            LocalizedLiteral failureLit = loginElem.FindControl("FailureText") as LocalizedLiteral;
            if (failureLit != null)
            {
                failureLit.Visible = true;
                failureLit.Text = GetString("banip.ipisbannedlogin");
            }
        }

        loginElem.RememberMeSet = PersistentLogin;
    }


    /// <summary>
    /// Login error handler.
    /// </summary>
    protected void loginElem_LoginError(object sender, EventArgs e)
    {
        // Check if custom failure text is not set
        if (string.IsNullOrEmpty(FailureText))
        {
            Label failureLit = loginElem.FindControl("FailureText") as Label;
            if (failureLit != null)
            {
                // Display account lock information
                if (AuthenticationHelper.DisplayAccountLockInformation(CMSContext.CurrentSiteName))
                {
                    // Check if account locked due to reaching maximum invalid logon attempts
                    string link = "<a href=\"#\" onclick=\"" + Page.ClientScript.GetCallbackEventReference(this, "null", "UpdateLabel_" + ClientID, "'" + failureLit.ClientID + "'") + ";\">" + GetString("general.clickhere") + "</a>";
                    if (ValidationHelper.GetBoolean(RequestStockHelper.GetItem("UserAccountLockedInvalidLogonAttempts"), false))
                    {
                        loginElem.FailureText = GetString("invalidlogonattempts.unlockaccount.accountlocked");
                        if (!ErrorAsPopup)
                        {
                            loginElem.FailureText += string.Format(GetString("invalidlogonattempts.unlockaccount.accountlockedlink"), link);
                        }
                    }

                    if (ValidationHelper.GetBoolean(RequestStockHelper.GetItem("UserAccountLockedPasswordExpired"), false))
                    {
                        loginElem.FailureText = GetString("passwordexpiration.accountlocked");
                        if (!ErrorAsPopup)
                        {
                            loginElem.FailureText += string.Format(GetString("invalidlogonattempts.unlockaccount.accountlockedlink"), link);
                        }
                    }
                }
            }
        }

        //Display the failure message in a client-side alert box
        if (ErrorAsPopup)
        {
            ScriptHelper.RegisterStartupScript(this, GetType(), "LoginError", ScriptHelper.GetScript("alert(" + ScriptHelper.GetString(loginElem.FailureText) + ");"));

            Label error = (Label)loginElem.FindControl("FailureText");
            error.Visible = false;
        }
    }

    #endregion


    #region "ICallbackEventHandler Members"

    public string GetCallbackResult()
    {
        string result = "";
        bool outParam = true;
        UserInfo ui = UserInfoProvider.GetUserInfo(loginElem.UserName);
        if (ui != null)
        {
            string siteName = CMSContext.CurrentSiteName;

            // Prepare return URL
            string returnUrl = URLHelper.CurrentURL;
            if (!string.IsNullOrEmpty(loginElem.UserName))
            {
                returnUrl = URLHelper.AddParameterToUrl(returnUrl, "username", loginElem.UserName);
            }

            switch(UserAccountLockCode.ToEnum(ui.UserAccountLockReason))
            {
                case UserAccountLockEnum.MaximumInvalidLogonAttemptsReached:
                    result = AuthenticationHelper.SendUnlockAccountRequest(ui, siteName, "USERLOGON", SettingsKeyProvider.GetStringValue(siteName + ".CMSSendPasswordEmailsFrom"), CMSContext.CurrentResolver, returnUrl);
                    break;

                case UserAccountLockEnum.PasswordExpired:
                    result = AuthenticationHelper.SendPasswordRequest(ui, siteName, "USERLOGON", SettingsKeyProvider.GetStringValue(siteName + ".CMSSendPasswordEmailsFrom"), "Membership.PasswordExpired", null, AuthenticationHelper.GetResetPasswordUrl(siteName), out outParam, returnUrl);
                    break;
            }
        }

        return result;
    }

    public void RaiseCallbackEvent(string eventArgument)
    {
        return;
    }

    #endregion
}