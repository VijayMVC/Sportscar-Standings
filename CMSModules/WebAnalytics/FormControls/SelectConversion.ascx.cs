using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

using CMS.CMSHelper;
using CMS.FormControls;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.UIControls;
using CMS.WebAnalytics;
using CMS.LicenseProvider;

public partial class CMSModules_WebAnalytics_FormControls_SelectConversion : FormEngineUserControl
{
    #region "Variables"

    private bool wasInit = false;
    private string mWhereCondition = String.Empty;
    private string mABTestName = String.Empty;
    private string mMVTestName = String.Empty;

    #endregion


    #region "Properties"

    /// <summary>
    /// All record value for UniSelector
    /// </summary>
    public string AllRecordValue
    {
        get
        {
            return usConversions.AllRecordValue;
        }
        set
        {
            usConversions.AllRecordValue = value;
        }
    }

    /// <summary>
    /// Selection mode of control (dropdown,multiselect...).
    /// </summary>
    public SelectionModeEnum SelectionMode
    {
        get
        {
            return usConversions.SelectionMode;
        }
        set
        {
            usConversions.SelectionMode = value;
        }
    }


    /// <summary>
    /// If true change of dropdown raises PostBack.
    /// </summary>
    public bool PostbackOnDropDownChange
    {
        get
        {
            return ddlConversions.AutoPostBack;
        }
        set
        {
            ddlConversions.AutoPostBack = value;
        }
    }


    /// <summary>
    /// Enables or disables UniSelector
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return usConversions.Enabled;
        }
        set
        {
            usConversions.Enabled = value;
        }
    }


    /// <summary>
    /// Value representing the control.
    /// </summary>
    public override object Value
    {
        get
        {
            return usConversions.Value;
        }
        set
        {
            usConversions.Value = value;
        }
    }


    /// <summary>
    /// Where condition for selector.
    /// </summary>
    public string WhereCondition
    {
        get
        {
            return mWhereCondition;
        }
        set
        {
            mWhereCondition = value;
        }
    }


    /// <summary>
    /// If true (all) is added to conversion selector.
    /// </summary>
    public bool AllowAll
    {
        get
        {
            return usConversions.AllowAll;
        }
        set
        {
            usConversions.AllowAll = value;
        }
    }


    /// <summary>
    /// For drop down mode - we select only conversions for one test.
    /// </summary>
    public string ABTestName
    {
        get
        {
            return mABTestName;
        }
        set
        {
            mABTestName = value;
        }
    }


    /// <summary>
    /// For drop down mode - we select only conversions for one test.
    /// </summary>
    public string MVTestName
    {
        get
        {
            return mMVTestName;
        }
        set
        {
            mMVTestName = value;
        }
    }


    /// <summary>
    /// Uniselector controls
    /// </summary>
    public UniSelector UniSelector
    {
        get
        {
            return usConversions;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        usConversions.IsLiveSite = IsLiveSite;
        usConversions.TextBoxSelect.MaxLength = 200;

        // Check user authorization
        if (CMSContext.CurrentUser.IsAuthorizedPerResource("CMS.WebAnalytics", "ManageConversions") && (SelectionMode == SelectionModeEnum.SingleTextBox))
        {
            string url = "~/CMSModules/WebAnalytics/Pages/Tools/Conversion/edit.aspx?conversionName=##ITEMID##&modaldialog=true";
            usConversions.EditItemPageUrl = url;

            url = "~/CMSModules/WebAnalytics/Pages/Tools/Conversion/edit.aspx?modaldialog=true";
            usConversions.NewItemPageUrl = url;

            usConversions.EditDialogWindowWidth = 610;
            usConversions.EditDialogWindowHeight = 400;
        }

        // PostBack action
        if (PostbackOnDropDownChange)
        {
            usConversions.DropDownSingleSelect.AutoPostBack = true;
            ScriptManager scr = ScriptManager.GetCurrent(Page);
            scr.RegisterPostBackControl(usConversions);
        }

        // Create where condition
        usConversions.WhereCondition = SqlHelperClass.AddWhereCondition(usConversions.WhereCondition, "ConversionSiteID = " + CMSContext.CurrentSiteID);
    }


    protected override void OnPreRender(EventArgs e)
    {
        if (!URLHelper.IsPostback())
        {
            // Reload data
            ReloadData(false);
        }

        base.OnPreRender(e);
    }


    /// <summary>
    /// Reloads data
    /// </summary>
    /// <param name="forceReload">Indicates whether apply force load</param>
    public void ReloadData(bool forceReload)
    {
        if (forceReload || !wasInit)
        {
            // Reload data
            CreateWhereCondition();
            usConversions.Reload(forceReload);
            wasInit = true;
        }
    }


    /// <summary>
    /// Creates where condition
    /// </summary>
    private void CreateWhereCondition()
    {
        usConversions.WhereCondition = SqlHelperClass.AddWhereCondition(usConversions.WhereCondition, WhereCondition);

        // Where condition for A/B testing
        if (ABTestName != String.Empty)
        {
            usConversions.WhereCondition = SqlHelperClass.AddWhereCondition(usConversions.WhereCondition, "ConversionName IN (SELECT StatisticsObjectName FROM Analytics_Statistics WHERE StatisticsSiteID = " + CMSContext.CurrentSiteID + " AND StatisticsCode LIKE (N'abconversion;" + SqlHelperClass.GetSafeQueryString(ABTestName, false) + ";%'))");
        }

        // Where condition for MVT testing
        if (MVTestName != String.Empty)
        {
            usConversions.WhereCondition = SqlHelperClass.AddWhereCondition(usConversions.WhereCondition, "ConversionName IN (SELECT StatisticsObjectName FROM Analytics_Statistics WHERE StatisticsSiteID = " + CMSContext.CurrentSiteID + " AND StatisticsCode LIKE (N'mvtconversion;" + SqlHelperClass.GetSafeQueryString(MVTestName, false) + ";%'))");
        }
    }


    /// <summary>
    /// Test if conversion is valid and create new conversion if not exists
    /// </summary>    
    public override bool IsValid()
    {
        // Get validated string
        String value = ValidationHelper.GetString(usConversions.Value, String.Empty).Trim();
        if (value != String.Empty)
        {
            String domain = URLHelper.GetCurrentDomain();
            if (DataHelper.GetNotEmpty(domain, "") != "")
            {
                string parsedDomain = LicenseKeyInfoProvider.ParseDomainName(domain);
                if (!LicenseKeyInfoProvider.IsFeatureAvailable(parsedDomain, FeatureEnum.CampaignAndConversions))
                {
                    ValidationError = GetString("conversionselector.nolicence");
                    return false;
                }
            }

            // Validate for code name
            if (!ValidationHelper.IsCodeName(value))
            {
                ValidationError = GetString("conversion.validcodename");
                return false;
            }

            // Test if selected name exists
            ConversionInfo ci = ConversionInfoProvider.GetConversionInfo(value, CMSContext.CurrentSiteName);

            // If not exist create new one
            if (ci == null)
            {
                if (!CMSContext.CurrentUser.IsAuthorizedPerResource("CMS.WebAnalytics", "ManageConversions"))
                {
                    ValidationError = GetString("conversion.notallowedcreate");
                    return false;
                }

                // Create new object
                ci = new ConversionInfo();
                ci.ConversionName = value;
                ci.ConversionDisplayName = value;
                ci.ConversionSiteID = CMSContext.CurrentSiteID;

                // Save to database
                ConversionInfoProvider.SetConversionInfo(ci);
            }
        }

        return true;
    }

    #endregion
}