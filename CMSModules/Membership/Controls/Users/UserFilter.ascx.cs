using System;
using System.Collections;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.Controls;
using CMS.FormControls;
using CMS.GlobalHelper;
using CMS.IO;
using CMS.LicenseProvider;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.ExtendedControls;

public partial class CMSModules_Membership_Controls_Users_UserFilter : CMSAbstractBaseFilterControl, IUserFilter
{
    #region "Variables"

    private bool showGroups;
    private const string pathToGroupselector = "~/CMSModules/Groups/FormControls/MembershipGroupSelector.ascx";
    private const string pathToScoreSelector = "~/CMSModules/Scoring/FormControls/SelectScore.ascx";
    private string mAlphabetSeparator = "";
    private bool isAdvancedMode;
    private Hashtable alphabetHash = new Hashtable();
    private FormEngineUserControl selectInGroups;
    private FormEngineUserControl selectNotInGroups;
    private FormEngineUserControl scoreSelector;
    private bool? mDisplayGuestsByDefault;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets the site id for which the users should be filtered.
    /// </summary>
    public int SiteID
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the visibility of alphabet panel.
    /// </summary>
    public bool AlphabetVisible
    {
        get
        {
            return pnlAlphabet.Visible;
        }
        set
        {
            pnlAlphabet.Visible = value;
        }
    }


    /// <summary>
    /// Gets or sets the alphabet separator.
    /// </summary>
    public string AlphabetSeparator
    {
        get
        {
            return mAlphabetSeparator;
        }
        set
        {
            mAlphabetSeparator = value;
        }
    }


    /// <summary>
    /// Gets the where condition created using filtered parameters.
    /// </summary>
    public override string WhereCondition
    {
        get
        {
            return GenerateWhereCondition();
        }
        set
        {
            base.WhereCondition = value;
        }
    }



    /// <summary>
    /// Gets or sets if anonymous visitors should be displayed by default
    /// </summary>
    public bool DisplayGuestsByDefault
    {
        get
        {
            if (mDisplayGuestsByDefault == null)
            {
                return QueryHelper.GetBoolean("guest", false);
            }
            return (bool)mDisplayGuestsByDefault;
        }
        set
        {
            mDisplayGuestsByDefault = value;
        }
    }




    /// <summary>
    /// Indicates if guests should be displayed.
    /// </summary>
    public bool DisplayGuests
    {
        get
        {
            return (chkDisplayAnonymous.Checked && chkDisplayAnonymous.Visible && isAdvancedMode) || (chkDisplayAnonymous.Checked && DisplayGuestsByDefault) && ContactManagementPermission;
        }
    }


    /// <summary>
    /// Indicates if user has permission to display contact data.
    /// </summary>
    public bool ContactManagementPermission
    {
        get
        {
            return ResourceSiteInfoProvider.IsResourceOnSite("CMS.ContactManagement", CMSContext.CurrentSiteName)
                     && CurrentUser.IsAuthorizedPerUIElement("CMS.OnlineMarketing", "Contacts")
                     && CurrentUser.IsAuthorizedPerResource("CMS.ContactManagement", "ReadContacts")
                     && ModuleEntry.IsModuleRegistered("CMS.ContactManagement")
                     && ModuleEntry.IsModuleLoaded("CMS.ContactManagement");
        }
    }


    /// <summary>
    /// Indicates if 'user enabled' filter should be hidden.
    /// </summary>
    public bool DisplayUserEnabled
    {
        get
        {
            return plcUserEnabled.Visible;
        }
        set
        {
            plcUserEnabled.Visible = value;
        }
    }


    /// <summary>
    /// Selected score.
    /// </summary>
    public int SelectedScore
    {
        get
        {
            if ((scoreSelector != null) && scoreSelector.Enabled)
            {
                return ValidationHelper.GetInteger(scoreSelector.Value, 0);
            }
            return 0;
        }
    }


    /// <summary>
    /// Selected site.
    /// </summary>
    public int SelectedSite
    {
        get
        {
            return (siteSelector.Visible ? siteSelector.SiteID : 0);
        }
    }


    /// <summary>
    /// Indicates if checkbox for hiding/displaying hidden users should be visible.
    /// </summary>
    public bool EnableDisplayingHiddenUsers
    {
        get
        {
            return plcHidden.Visible;
        }
        set
        {
            plcHidden.Visible = value;
        }
    }


    /// <summary>
    /// Indicates if checkbox for displaying guests should be visible.
    /// </summary>
    public bool EnableDisplayingGuests
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates if filter is working with CMS_Session table instead of CMS_User.
    /// </summary>
    public bool SessionInsteadOfUser
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets filter mode for various type of users list.
    /// </summary>
    public string CurrentMode
    {
        get;
        set;
    }


    #endregion


    #region "Events"

    /// <summary>
    /// Resets the associated UniGrid control.
    /// </summary>
    protected void btnReset_Click(object sender, EventArgs e)
    {
        UniGrid grid = FilteredControl as UniGrid;
        if (grid != null)
        {
            grid.Reset();
        }
    }


    /// <summary>
    /// Applies filter on associated UniGrid control.
    /// </summary>
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        UniGrid grid = FilteredControl as UniGrid;
        if (grid != null)
        {
            grid.ApplyFilter(sender, e);
        }
    }

    #endregion


    #region "Page methods"


    protected override void OnInit(EventArgs e)
    {
        SiteID = QueryHelper.GetInteger("siteid", 0);


        if (File.Exists(HttpContext.Current.Request.MapPath(ResolveUrl(pathToGroupselector))))
        {
            Control ctrl = this.LoadUserControl(pathToGroupselector);
            if (ctrl != null)
            {
                selectInGroups = ctrl as FormEngineUserControl;
                ctrl.ID = "selGroups";
                ctrl = this.LoadUserControl(pathToGroupselector);
                selectNotInGroups = ctrl as FormEngineUserControl;
                ctrl.ID = "selNoGroups";

                plcGroups.Visible = true;
                plcSelectInGroups.Controls.Add(selectInGroups);
                plcSelectNotInGroups.Controls.Add(selectNotInGroups);

                selectNotInGroups.SetValue("UseFriendlyMode", true);
                selectInGroups.IsLiveSite = false;
                selectInGroups.SetValue("UseFriendlyMode", true);
                selectNotInGroups.IsLiveSite = false;
            }
        }

        if (LicenseHelper.CheckFeature(URLHelper.GetCurrentDomain(), FeatureEnum.LeadScoring)
               && ResourceSiteInfoProvider.IsResourceOnSite("CMS.Scoring", CMSContext.CurrentSiteName)
               && SettingsKeyProvider.GetBoolValue(CMSContext.CurrentSiteName + ".CMSEnableOnlineMarketing"))
        {
            Control ctrl = this.LoadUserControl(pathToScoreSelector);
            if (ctrl != null)
            {
                ctrl.ID = "selectScore";
                scoreSelector = ctrl as FormEngineUserControl;
                if (scoreSelector != null)
                {
                    plcUpdateContent.Controls.Add(scoreSelector);
                    scoreSelector.SetValue("AllowAll", false);
                    scoreSelector.SetValue("AllowEmpty", true);
                }
            }
        }
        else
        {
            plcScore.Visible = false;
            lblScore.AssociatedControlID = null;
        }

        CMSUsersPage usersPage = (CMSUsersPage)Page;
        if (usersPage != null)
        {
            CurrentMode = usersPage.FilterMode;
            EnableDisplayingGuests = usersPage.EnableDisplayingGuests;

            if (CurrentMode == "online" && usersPage.DisplayContacts)
            {
                if (usersPage.DisplayScore && (scoreSelector != null))
                {
                    plcScore.Visible = true;
                }
            }
        }

        // Initialize advanced filter dropdownlists
        if (!RequestHelper.IsPostBack())
        {
            InitAllAnyDropDown(drpTypeSelectInRoles);
            InitAllAnyDropDown(drpTypeSelectNotInRoles);
            InitAllAnyDropDown(drpTypeSelectInGroups);
            InitAllAnyDropDown(drpTypeSelectNotInGroups);

            // Init lock account reason DDL
            drpLockReason.Items.Add(new ListItem(GetString("General.selectall"), ""));
            DataHelper.FillListControlWithEnum(typeof(UserAccountLockEnum), drpLockReason, "userlist.account.", null);
        }


        base.OnInit(e);

        plcDisplayAnonymous.Visible = ContactManagementPermission && SessionManager.StoreOnlineUsersInDatabase && EnableDisplayingGuests;
        if (!RequestHelper.IsPostBack())
        {
            chkDisplayAnonymous.Checked = DisplayGuestsByDefault;
        }

        siteSelector.DropDownSingleSelect.AutoPostBack = true;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        InitializeForm();

        drpLockReason.Enabled = chkEnabled.Checked;

        // Show alphabet filter if enabled
        if (pnlAlphabet.Visible)
        {
            pnlAlphabet.Controls.Add(CreateAlphabetTable());
        }

        if (scoreSelector != null)
        {
            int siteId = QueryHelper.GetInteger("siteid", 0);
            scoreSelector.Enabled = (siteId > 0) || ((siteId == 0) && (siteSelector.SiteID > 0));

            if (siteId == 0)
            {
                scoreSelector.SetValue("SiteID", siteSelector.SiteID);
            }
        }

        // Show correct filter panel
        EnsureFilterMode();
        pnlAdvancedFilter.Visible = isAdvancedMode;
        pnlSimpleFilter.Visible = !isAdvancedMode;

        // Set reset link button
        UniGrid grid = FilteredControl as UniGrid;
        if (grid != null && grid.RememberState)
        {
            if (isAdvancedMode)
            {
                btnAdvancedReset.Text = GetString("general.reset");
                btnAdvancedReset.Click += btnReset_Click;
            }
            else
            {
                btnReset.Text = GetString("general.reset");
                btnReset.Click += btnReset_Click;
            }
        }
        else
        {
            if (isAdvancedMode)
            {
                btnAdvancedReset.Visible = false;
            }
            else
            {
                btnReset.Visible = false;
            }
        }

        // Show group filter only if enabled
        if (SiteID > 0)
        {
            SiteInfo si = SiteInfoProvider.GetSiteInfo(SiteID);
            if ((si != null) && isAdvancedMode)
            {
                showGroups = ModuleCommands.CommunitySiteHasGroup(si.SiteID);
            }
        }

        // Setup role selector
        selectNotInRole.SiteID = SiteID;
        selectRoleElem.SiteID = SiteID;
        selectRoleElem.CurrentSelector.ResourcePrefix = "addroles";
        selectNotInRole.CurrentSelector.ResourcePrefix = "addroles";
        selectRoleElem.UseFriendlyMode = true;
        selectNotInRole.UseFriendlyMode = true;

        // Setup groups selectors
        plcGroups.Visible = showGroups;
        if (selectInGroups != null)
        {
            selectInGroups.StopProcessing = !showGroups;
            selectInGroups.FormControlParameter = SiteID;
        }

        if (selectNotInGroups != null)
        {
            selectNotInGroups.StopProcessing = !showGroups;
            selectNotInGroups.FormControlParameter = SiteID;
        }

        if (SessionInsteadOfUser && DisplayGuestsByDefault)
        {
            plcNickName.Visible = false;
            plcUserName.Visible = false;
        }

        if (grid != null)
        {
            string argument = ValidationHelper.GetString(Request.Params["__EVENTARGUMENT"], String.Empty);
            if (argument == "Alphabet")
            {
                grid.ApplyFilter(null, null);
            }
        }

        if (QueryHelper.GetBoolean("isonlinemarketing", false))
        {
            // Set disabled modules info (only on On-line marketing tab)
            ucDisabledModule.SettingsKeys = "CMSSessionUseDBRepository;CMSEnableOnlineMarketing";
            ucDisabledModule.InfoTexts.Add(GetString("administration.users.usedbrepository.disabled") + "<br/>");
            ucDisabledModule.InfoTexts.Add(GetString("om.onlinemarketing.disabled"));
            ucDisabledModule.Visible = true;
        }
   }

    #endregion


    #region "UI methods"


    /// <summary>
    /// Stores filter state to the specified object.
    /// </summary>
    /// <param name="state">The object that holds the filter state.</param>
    public override void StoreFilterState(FilterState state)
    {
        base.StoreFilterState(state);
        state.AddValue("AdvancedMode", isAdvancedMode);
        state.AddValue("Alphabet", hdnAlpha.Value);
    }


    /// <summary>
    /// Restores filter state from the specified object.
    /// </summary>
    /// <param name="state">The object that holds the filter state.</param>
    public override void RestoreFilterState(FilterState state)
    {
        base.RestoreFilterState(state);
        isAdvancedMode = state.GetBoolean("AdvancedMode");
        ViewState["IsAdvancedMode"] = isAdvancedMode;
        hdnAlpha.Value = state.GetString("Alphabet");
    }


    /// <summary>
    /// Resets filter to the default state.
    /// </summary>
    public override void ResetFilter()
    {
        txtSearch.Text = String.Empty;
        ViewState["IsAdvancedMode"] = isAdvancedMode;

        fltEmail.ResetFilter();
        fltUserName.ResetFilter();
        fltNickName.ResetFilter();
        fltFullName.ResetFilter();

        drpTypeSelectInGroups.SelectedIndex = 0;
        drpTypeSelectInRoles.SelectedIndex = 0;
        drpTypeSelectNotInGroups.SelectedIndex = 0;
        drpTypeSelectNotInRoles.SelectedIndex = 0;

        if (siteSelector.DropDownSingleSelect.Items.Count > 0)
        {
            siteSelector.DropDownSingleSelect.SelectedIndex = 0;
        }
        if (scoreSelector != null)
        {
            scoreSelector.Value = 0;
            scoreSelector.Enabled = (SiteID != 0);
        }

        selectRoleElem.Value = "";
        selectNotInRole.Value = "";

        if (selectInGroups != null)
        {
            selectNotInGroups.Value = "";
        }

        if (selectNotInGroups != null)
        {
            selectInGroups.Value = "";
        }

        chkDisplayAnonymous.Checked = DisplayGuestsByDefault;

        // Reset the panel with letters to its default state.
        hdnAlpha.Value = String.Empty;
        foreach (HyperLink link in alphabetHash.Values)
        {
            link.CssClass = String.Empty;
        }
        HyperLink defaultLink = alphabetHash["ALL"] as HyperLink;
        if (defaultLink != null)
        {
            defaultLink.CssClass = "ActiveLink";
        }
    }

    /// <summary>
    /// Initializes items in "all/any" dropdown list
    /// </summary>
    /// <param name="drp">Dropdown list to initialize</param>
    private void InitAllAnyDropDown(DropDownList drp)
    {
        if (drp.Items.Count <= 0)
        {
            drp.Items.Add(new ListItem(GetString("General.selectall"), "ALL"));
            drp.Items.Add(new ListItem(GetString("General.Any"), "ANY"));
        }
    }


    /// <summary>
    /// Initializes the layout of the form.
    /// </summary>
    private void InitializeForm()
    {
        // General UI
        btnSimpleSearch.Text = GetString("General.Search");
        btnAdvancedSearch.Text = GetString("General.Show");
        lnkShowAdvancedFilter.Text = GetString("user.filter.showadvanced");
        imgShowAdvancedFilter.ImageUrl = GetImageUrl("Design/Controls/UniGrid/Actions/SortDown.png");
        lnkShowSimpleFilter.Text = GetString("user.filter.showsimple");
        imgShowSimpleFilter.ImageUrl = GetImageUrl("Design/Controls/UniGrid/Actions/SortUp.png");
        pnlSimpleFilter.Visible = !isAdvancedMode;
        pnlAdvancedFilter.Visible = isAdvancedMode;

        // Labels
        lblFullName.Text = GetString("general.FullName") + ResHelper.Colon;
        lblNickName.Text = GetString("userlist.NickName") + ResHelper.Colon;
        lblEmail.Text = GetString("userlist.Email") + ResHelper.Colon;
        lblInRoles.Text = GetString("userlist.InRoles") + ResHelper.Colon;
        lblNotInRoles.Text = GetString("userlist.NotInRoles") + ResHelper.Colon;
        lblInGroups.Text = GetString("userlist.InGroups") + ResHelper.Colon;
        lblNotInGroups.Text = GetString("userlist.NotInGroups") + ResHelper.Colon;
        lblEnabled.Text = GetString("userlist.Enabled") + ResHelper.Colon;
        lblLockReason.Text = GetString("userlist.LockReason") + ResHelper.Colon;

        // Checkbox javascript
        string script = "var drpEnabled = document.getElementById('" + drpLockReason.ClientID + "'); if(drpEnabled) {drpEnabled.disabled = !drpEnabled.disabled; if(drpEnabled.disabled){drpEnabled.selectedIndex = 0;}}";
        chkEnabled.Attributes.Add("onclick", script);

        int siteId = QueryHelper.GetInteger("siteid", 0);
        plcSite.Visible = siteId == 0;

        // Rename columns for online users saved in database
        if (SessionInsteadOfUser)
        {
            fltUserName.Column = "SessionUserName";
            fltNickName.Column = "SessionNickName";
            fltFullName.Column = "SessionFullName";
            fltEmail.Column = "SessionEmail";
        }
    }


    /// <summary>
    /// Creates table element with alphabet.
    /// </summary>
    private Table CreateAlphabetTable()
    {
        // Register javascript for alphabet filtering
        string postbackScript = Page.ClientScript.GetPostBackEventReference(this, "Alphabet");
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "alphabetSelect", ScriptHelper.GetScript(
            @"function SetAlphabetLetter(letter) {
                var hiddenElem = document.getElementById('" + hdnAlpha.ClientID + @"');
                if (hiddenElem != null) {
                    hiddenElem.value = letter;
                    " + postbackScript + @";
                }
                return false;
              }"));

        Table table = new Table();
        table.EnableViewState = false;
        table.Attributes.Add("style", "width: 100%");
        table.Rows.Add(new TableRow());

        TableCell cell = new TableCell();
        HyperLink link = new HyperLink();

        // Create "ALL" link
        link.NavigateUrl = "javascript:SetAlphabetLetter('ALL');";
        link.Text = GetString("general.all");

        cell.Controls.Add(link);
        table.Rows[0].Cells.Add(cell);
        alphabetHash["ALL"] = link;

        // Create alphabet links
        for (char c = 'A'; c <= 'Z'; c++)
        {
            // Add link
            cell = new TableCell();
            link = new HyperLink();
            table.Rows[0].Cells.Add(cell);

            link.NavigateUrl = "javascript:SetAlphabetLetter('" + c + "');";

            link.Text = c.ToString();
            cell.Controls.Add(link);
            table.Rows[0].Cells.Add(cell);

            // Add to hashtable
            alphabetHash[c.ToString()] = link;
        }

        // If something is already selected, highlight the letter, otherwise all
        string key = String.IsNullOrEmpty(hdnAlpha.Value) ? "ALL" : hdnAlpha.Value;
        link = alphabetHash[key] as HyperLink;
        if (link != null)
        {
            link.CssClass = "ActiveLink";
        }

        return table;
    }


    /// <summary>
    /// Ensures correct filter mode flag if filter mode was just changed.
    /// </summary>
    private void EnsureFilterMode()
    {
        if (URLHelper.IsPostback())
        {
            // Get current event target
            string uniqieId = ValidationHelper.GetString(Request.Params["__EVENTTARGET"], String.Empty);
            uniqieId = uniqieId.Replace("$", "_");

            // If postback was fired by mode switch, update isAdvancedMode variable
            if (uniqieId == lnkShowAdvancedFilter.ClientID)
            {
                isAdvancedMode = true;
            }
            else if (uniqieId == lnkShowSimpleFilter.ClientID)
            {
                chkDisplayAnonymous.Checked = DisplayGuestsByDefault;
                isAdvancedMode = false;
            }
            else
            {
                isAdvancedMode = ValidationHelper.GetBoolean(ViewState["IsAdvancedMode"], false);
            }
        }
    }


    /// <summary>
    /// Sets the advanced mode.
    /// </summary>
    protected void lnkShowAdvancedFilter_Click(object sender, EventArgs e)
    {
        isAdvancedMode = true;
        ViewState["IsAdvancedMode"] = isAdvancedMode;
        pnlSimpleFilter.Visible = !isAdvancedMode;
        pnlAdvancedFilter.Visible = isAdvancedMode;
    }


    /// <summary>
    /// Sets the simple mode.
    /// </summary>
    protected void lnkShowSimpleFilter_Click(object sender, EventArgs e)
    {
        isAdvancedMode = false;
        ViewState["IsAdvancedMode"] = isAdvancedMode;
        pnlSimpleFilter.Visible = !isAdvancedMode;
        pnlAdvancedFilter.Visible = isAdvancedMode;
    }

    #endregion


    #region "Search methods - where condition"

    /// <summary>
    /// Generates complete filter where condition.
    /// </summary>    
    private string GenerateWhereCondition()
    {
        // Get mode from view state
        EnsureFilterMode();

        string whereCond = null;

        // Create first where condition depending on mode
        if (isAdvancedMode)
        {
            whereCond = AdvancedSearch();
        }
        else
        {
            whereCond = SimpleSearch();
        }

        int siteId = SiteID;
        if (SelectedSite > 0)
        {
            siteId = SelectedSite;
        }

        // Append site condition if siteid given.
        if (siteId > 0)
        {
            if (SessionInsteadOfUser)
            {
                whereCond += (String.IsNullOrEmpty(whereCond) ? "" : " AND ") + "(SessionSiteID=" + siteId + ")";
            }
            else
            {
                whereCond += (String.IsNullOrEmpty(whereCond) ? "" : " AND ") + "(UserID IN (SELECT UserID FROM CMS_UserSite WHERE SiteID=" + siteId + "))";
            }
        }

        if (SessionInsteadOfUser)
        {
            if (!DisplayGuests)
            {
                whereCond += (String.IsNullOrEmpty(whereCond) ? "" : " AND ") + "(SessionUserID > 0)";
            }

            if (chkDisplayHidden.Visible && !chkDisplayHidden.Checked)
            {
                whereCond = SqlHelperClass.AddWhereCondition(whereCond, "(SessionUserIsHidden=0 OR SessionUserID IS NULL)");
            }
        }
        else
        {
            whereCond += (String.IsNullOrEmpty(whereCond) ? "" : " AND ") + "(UserID > 0)";

            if (chkDisplayHidden.Visible && !chkDisplayHidden.Checked)
            {
                whereCond = SqlHelperClass.AddWhereCondition(whereCond, "(UserID IN (SELECT UserID FROM CMS_User WHERE UserIsHidden=0 OR UserIsHidden IS NULL))");
            }
        }

        return whereCond;
    }


    /// <summary>
    /// Generates where condition for advanced filter.
    /// </summary>
    public string AdvancedSearch()
    {
        // Get condition parts
        string roleWhere = GetMultipleSelectorCondition(drpTypeSelectInRoles.SelectedValue, "roles", selectRoleElem.Value.ToString().Trim(), false);
        string roleWhereNot = GetMultipleSelectorCondition(drpTypeSelectNotInRoles.SelectedValue, "roles", selectNotInRole.Value.ToString().Trim(), true);

        string groupWhere = (showGroups) ? GetMultipleSelectorCondition(drpTypeSelectInGroups.SelectedValue, "groups", selectInGroups.Value.ToString().Trim(), false) : "";
        string groupWhereNot = (showGroups) ? GetMultipleSelectorCondition(drpTypeSelectNotInGroups.SelectedValue, "groups", selectNotInGroups.Value.ToString().Trim(), true) : "";

        // Join where conditions
        if ((roleWhere != "") && (roleWhereNot != ""))
        {
            roleWhere = roleWhere + " AND " + roleWhereNot;
        }
        else
        {
            roleWhere = roleWhere + roleWhereNot;
        }


        string whereCond = "";
        whereCond = SqlHelperClass.AddWhereCondition(whereCond, fltUserName.GetCondition());
        whereCond = SqlHelperClass.AddWhereCondition(whereCond, fltFullName.GetCondition());
        whereCond = SqlHelperClass.AddWhereCondition(whereCond, fltEmail.GetCondition());
        whereCond = SqlHelperClass.AddWhereCondition(whereCond, fltNickName.GetCondition());
        whereCond = SqlHelperClass.AddWhereCondition(whereCond, roleWhere);
        whereCond = SqlHelperClass.AddWhereCondition(whereCond, roleWhereNot);
        whereCond = SqlHelperClass.AddWhereCondition(whereCond, groupWhere);
        whereCond = SqlHelperClass.AddWhereCondition(whereCond, groupWhereNot);

        if (DisplayUserEnabled)
        {
            string enabledWhere = (chkEnabled.Checked ? "UserEnabled = 0" : String.Empty);
            string lockWhere = SqlHelperClass.GetSafeQueryString((!string.IsNullOrEmpty(drpLockReason.SelectedValue) ? "UserAccountLockReason = " + ((drpLockReason.SelectedValue == "0") ? drpLockReason.SelectedValue + " OR UserAccountLockReason IS NULL" : drpLockReason.SelectedValue) : ""));
            whereCond = SqlHelperClass.AddWhereCondition(whereCond, enabledWhere);
            whereCond = SqlHelperClass.AddWhereCondition(whereCond, lockWhere);
        }

        // Starting letter for username
        string firstLetter = hdnAlpha.Value;
        if (firstLetter != "" && firstLetter != "ALL")
        {
            if (SessionInsteadOfUser)
            {
                whereCond += (String.IsNullOrEmpty(whereCond) ? "" : " AND ") + "(SessionFullName LIKE N'" + SqlHelperClass.GetSafeQueryString(firstLetter, false) + "%')";
            }
            else
            {
                whereCond += (String.IsNullOrEmpty(whereCond) ? "" : " AND ") + "(UserName LIKE N'" + SqlHelperClass.GetSafeQueryString(firstLetter, false) + "%')";
            }
        }

        return whereCond;
    }


    /// <summary>
    /// Returns where condition for specialized role and group conditions.
    /// </summary>
    /// <param name="op">Condition to use (ANY/ALL)</param>
    /// <param name="type">Type of condition to create (roles,groups)</param>
    /// <param name="valuesStr">Values separated with semicolon</param>    
    /// <param name="negate">If true add negation to where condition (NOT)</param>    
    private string GetMultipleSelectorCondition(string op, string type, string valuesStr, bool negate)
    {
        string retval = string.Empty;
        if (!String.IsNullOrEmpty(valuesStr))
        {
            string having;
            string[] items = valuesStr.Split(';');
            string not = negate ? "NOT" : String.Empty;

            switch (type.ToLowerCSafe())
            {
                case "roles":
                    // Create where condition for roles
                    // Global roles start with prefix '.'                    
                    StringBuilder sbSite = new StringBuilder();
                    StringBuilder sbGlobal = new StringBuilder();

                    // First split both groups (of roles) to different string builders 
                    foreach (String item in items)
                    {
                        if (item.StartsWithCSafe("."))
                        {
                            sbGlobal.Append(",'", item.TrimStart('.').Replace("'", "''"), "'");
                        }
                        else
                        {
                            sbSite.Append(",'", item.Replace("'", "''"), "'");
                        }
                    }

                    // Convert builders to string
                    String siteItem = sbSite.ToString().Trim(',');
                    String globalItem = sbGlobal.ToString().Trim(',');

                    // Create where condition for site roles. Empty string if no site role selected
                    String siteWhere = (siteItem != String.Empty) ? "RoleName IN (" + siteItem + " )" : String.Empty;

                    // Create global roles where condition. Only if user selects any global roles
                    String globalWhere = (globalItem != String.Empty) ? " (RoleName IN (" + globalItem + ") AND SiteID IS NULL ) " : String.Empty;

                    // If user selected both site and global roles add 'OR' between these two conditions
                    if ((globalItem != String.Empty) && (siteWhere != String.Empty))
                    {
                        siteWhere += " OR ";
                    }

                    having = (op.ToLowerCSafe() == "all") ? "HAVING COUNT(RoleID) = " + items.Length : String.Empty;

                    // Select users assigned to roles by given names (no matter what site) or user assigned to global role (only global roles accepted)                                       
                    retval = SessionInsteadOfUser ? "SessionUserID " : "UserID ";
                    retval += not + @" IN (SELECT UserID FROM CMS_UserRole
                                                WHERE RoleID IN (SELECT RoleID FROM CMS_Role WHERE " + siteWhere + globalWhere + @")  AND
                                                    (ValidTo IS NULL OR ValidTo > @Now)
                                                GROUP BY UserID " + having + ")";
                    break;

                case "groups":
                    having = (op.ToLowerCSafe() == "all") ? "HAVING COUNT (MemberGroupID) =" + items.Length : String.Empty;
                    string itemsWhere = SqlHelperClass.GetWhereCondition("GroupName", items);
                    retval = SessionInsteadOfUser ? "SessionUserID " : "UserID ";
                    retval += not + @" IN (SELECT MemberUserID FROM Community_GroupMember 
                                         WHERE MemberGroupID IN (SELECT GroupID FROM Community_Group WHERE " + itemsWhere + @")
                                         GROUP BY MemberUserID " + having + ")";
                    break;
            }
        }
        return retval;
    }


    /// <summary>
    /// Generates where condition for simple filter.
    /// </summary>    
    public string SimpleSearch()
    {
        string where = String.Empty;
        string searchExpression = null;
        string queryOperator = "LIKE";

        if (txtSearch.Text != String.Empty)
        {
            // Create skeleton of where condition (ensure also site and starting letter)
            if (SessionInsteadOfUser)
            {
                where = "((SessionUserName {0} N'{1}') OR (SessionEmail {0} N'{1}') OR (SessionFullName {0} N'{1}') OR (SessionNickName {0} N'{1}'))";
            }
            else
            {
                where = "((UserName {0} N'{1}') OR (Email {0} N'{1}') OR (FullName {0} N'{1}') OR (UserNickName {0} N'{1}'))";
            }

            // Avoid SQL Injection
            searchExpression = txtSearch.Text.Trim().Replace("'", "''");

            // Choose the operator (if surrounded with quotes use '=' operator instead of LIKE)            
            if (searchExpression.StartsWithCSafe("\"") && searchExpression.EndsWithCSafe("\""))
            {
                queryOperator = "=";

                // Remove quotes
                searchExpression = searchExpression.Substring(1, searchExpression.Length - 2);
            }
            else
            {
                searchExpression = "%" + searchExpression + "%";
            }
        }

        // Starting letter for username
        string firstLetter = hdnAlpha.Value;
        if (firstLetter != "" && firstLetter != "ALL")
        {
            if (!String.IsNullOrEmpty(where))
            {
                where += "AND ";
            }
            if (SessionInsteadOfUser)
            {
                where += "(SessionFullName LIKE N'" + SqlHelperClass.GetSafeQueryString(firstLetter, false) + "%')";
            }
            else
            {
                where += "(UserName LIKE N'" + SqlHelperClass.GetSafeQueryString(firstLetter, false) + "%')";
            }
        }

        // Get final where condition
        return String.Format(where, queryOperator, searchExpression);
    }


    #endregion


}