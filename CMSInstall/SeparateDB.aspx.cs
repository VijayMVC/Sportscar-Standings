﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Security.Principal;
using System.Text;
using System.Web.UI.WebControls;

using CMS.DataEngine;
using CMS.DatabaseHelper;
using CMS.EventLog;
using CMS.ExtendedControls;
using CMS.GlobalHelper;
using CMS.PortalControls;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;

[CheckLicence(FeatureEnum.DBSeparation)]
public partial class CMSInstall_SeparateDB : SiteManagerPage
{
    #region "Variables"

    private LocalizedButton mNextButton;
    private LocalizedButton mPreviousButton;
    private DBSeparationHelper separation;
    private const string STORAGE_KEY = "SeparateDBProgressLog";

    #endregion


    #region "Properties"

    /// <summary>
    /// Help button on start step.
    /// </summary>
    protected HelpControl StartHelp
    {
        get
        {
            return (HelpControl)wzdInstaller.Controls[0].Controls[2].Controls[0].Controls[0].Controls[1].FindControl("hlpContext");
        }
    }


    /// <summary>
    /// Help button.
    /// </summary>
    protected HelpControl Help
    {
        get
        {
            return (HelpControl)wzdInstaller.Controls[0].Controls[2].Controls[0].Controls[2].Controls[1].FindControl("hlpContext");
        }
    }


    /// <summary>
    /// User password.
    /// </summary>
    private string UserPassword
    {
        get
        {
            return ValidationHelper.GetString(ViewState["UserPassword"], null);
        }
        set
        {
            ViewState["UserPassword"] = value;
        }
    }


    /// <summary>
    /// Return URL.
    /// </summary>
    private string ReturnUrl
    {
        get
        {
            string returnUrl = QueryHelper.GetString("returnurl", String.Empty);

            // Ensure that URL is valid
            if (string.IsNullOrEmpty(returnUrl) || returnUrl.StartsWithCSafe("~") || returnUrl.StartsWithCSafe("/") || QueryHelper.ValidateHash("hash"))
            {
                return returnUrl;
            }
            return null;
        }
    }

    #endregion


    #region "Step wizard buttons"

    /// <summary>
    /// Previous button.
    /// </summary>
    public LocalizedButton PreviousButton
    {
        get
        {
            if (mPreviousButton == null)
            {
                mPreviousButton = wzdInstaller.FindControl("StepNavigationTemplateContainerID").FindControl("stepNavigation").FindControl("StepPrevButton") as LocalizedButton;
            }
            return mPreviousButton;
        }
    }


    /// <summary>
    /// Next button.
    /// </summary>
    public LocalizedButton NextButton
    {
        get
        {
            if (mNextButton == null)
            {
                mNextButton = wzdInstaller.FindControl("StepNavigationTemplateContainerID").FindControl("stepNavigation").FindControl("StepNextButton") as LocalizedButton;
            }
            return mNextButton;
        }
    }


    /// <summary>
    /// Next button.
    /// </summary>
    public LocalizedButton FinishNextButton
    {
        get
        {
            if (wzdInstaller.FindControl("FinishNavigationTemplateContainerID").FindControl("finishNavigation") != null)
            {
                return wzdInstaller.FindControl("FinishNavigationTemplateContainerID").FindControl("finishNavigation").FindControl("StepNextButton") as LocalizedButton;
            }
            return null;
        }
    }

    #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        PortalHelper.EnsureScriptManager(Page);
        if (FinishNextButton != null)
        {
            FinishNextButton.Click += FinishNextButton_Click;
        }
        CurrentMaster.HeadElements.Text += @"<base target=""_self"" />";
        CurrentMaster.HeadElements.Visible = true;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (!RequestHelper.IsCallback())
        {
            PreviousButton.Attributes.Remove("disabled");
            NextButton.Attributes.Remove("disabled");
        }

        StartHelp.Tooltip = ResHelper.GetFileString("install.tooltip");
        StartHelp.TopicName = "DBInstall_Step1";
        StartHelp.IconUrl = GetImageUrl("Others/LogonForm/HelpButton.png"); Response.Cache.SetNoStore();
        Help.Tooltip = ResHelper.GetFileString("install.tooltip");
        Help.IconUrl = GetImageUrl("Others/LogonForm/HelpButton.png");
        progress.NextButtonClientID = NextButton.ClientID;
        progress.PreviousButtonClientID = PreviousButton.ClientID;
        asyncControl.OnFinished += asyncControl_OnFinished;
        asyncControl.OnError += asyncControl_OnError;
        CurrentMaster.Title.TitleText = GetString("separationDB.Title");
        CurrentMaster.Title.TitleImage = GetImageUrl("Others/Install/separatedb.png");
        CurrentMaster.FooterContainer.CssClass = "Hidden";
        userServer.IsDBSeparation = true;
        asyncControl.LogPanel.CssClass = "Hidden";
        if (databaseDialog.UseExistingChecked && (wzdInstaller.ActiveStepIndex == 3))
        {
            separationFinished.Database = databaseDialog.ExistingDatabaseName;
            separationFinished.ConnectionString = databaseDialog.ConnectionString;
            separationFinished.DisplayCollationDialog();
        }
        databaseDialog.ServerName = userServer.ServerName;
        SetProgressStep();
        RegisterJS();
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        InitializeHeader(wzdInstaller.ActiveStepIndex);
        CurrentMaster.PanelBody.CssClass += " InstallBody";
        CurrentMaster.PanelContent.CssClass += " InstallerWaterMark";

        if (!String.IsNullOrEmpty(ReturnUrl))
        {
            CurrentMaster.HeaderContainer.CssClass += " SeparationHeader";

            NextButton.Text = GetString("separationDB.return");
            NextButton.OnClientClick = "window.location = \"" + URLHelper.GetAbsoluteUrl(ReturnUrl) + "\"; return false;";
            PreviousButton.Visible = false;

            // Ensure the refresh script
            const string defaultCondition = "((top.frames['cmsdesktop'] != null) || (top.frames['propheader'] != null))";
            const string topWindowReplace = "top.window.location.replace(top.window.location.href.replace(/\\/\\(\\w\\([^\\)]+\\)\\)/g, ''));";
            ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "TopWindow", ScriptHelper.GetScript(" if " + defaultCondition + " { try {" + topWindowReplace + "} catch(err){} }"));
        }
    }

    #endregion


    #region "Wizard events"

    /// <summary>
    /// Next step button click.
    /// </summary>
    protected void wzdInstaller_NextButtonClick(object sender, WizardNavigationEventArgs e)
    {
        bool validationResult = true;
        switch (wzdInstaller.ActiveStepIndex)
        {
            case 0:
                validationResult = CheckSqlSettings();
                break;

            case 1:
                validationResult = databaseDialog.ValidateForSeparation();
                break;
        }

        if (!validationResult)
        {
            e.Cancel = true;
        }
        else
        {
            databaseDialog.TasksManuallyStopped = false;
            RunAction();
        }
    }


    /// <summary>
    /// Finish button click.
    /// </summary>
    protected void FinishNextButton_Click(object sender, EventArgs e)
    {
        if (separationFinished.ValidateForSeparationFinish())
        {
            string error = String.Empty;
            if (!AzureHelper.IsRunningOnAzure)
            {
                var separationHelper = new DBSeparationHelper();

                separationHelper.InstallationConnStringSeparate = DBSeparationHelper.ConnStringSeparate;
                separationHelper.InstallScriptsFolder = CMSDatabaseHelper.GetSQLInstallPathToObjects();
                separationHelper.ScriptsFolder = Server.MapPath("~/App_Data/DBSeparation");
                error = separationHelper.DeleteSourceTables(false, true);
            }

            if (!String.IsNullOrEmpty(error))
            {
                separationFinished.ErrorLabel.Visible = true;
                separationFinished.ErrorLabel.Text = error;
                new EventLogProvider().LogEvent(EventLogProvider.EVENT_TYPE_ERROR, DateTime.Now, "Contact management database separation", "DELETEOLDDATA", URLHelper.CurrentURL, error);
            }
            else
            {
                EnableTasks();
                TakeSitesOnline();
                WebSyncHelperClass.CreateTask(WebFarmTaskTypeEnum.RestartApplication, "RestartApplication", "", null);
                ScriptHelper.RegisterStartupScript(this, typeof(string), "Close dialog", ScriptHelper.GetScript("RefreshParent(); CloseDialog();"));
            }
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Initialize wizard header.
    /// </summary>
    /// <param name="index">Step index</param>
    private void InitializeHeader(int index)
    {
        Help.Visible = true;
        StartHelp.Visible = true;

        string imgName = null;

        lblHeader.Text = ResHelper.GetFileString("Install.Step") + " - ";

        switch (index)
        {
            // SQL server and authentication mode
            case 0:
                imgName = "header_separation_0.png";
                StartHelp.TopicName = Help.TopicName = "DBSeparation_StepConnection";
                lblHeader.Text += ResHelper.GetString("separationDB.Step0");
                break;
            // Database
            case 1:
                imgName = "header_separation_1.png";
                StartHelp.TopicName = Help.TopicName = "DBSeparation_StepDB";
                lblHeader.Text += ResHelper.GetFileString("separationDB.Step1");
                break;
            // Separation
            case 2:
                imgName = "header_separation_2.png";
                StartHelp.Visible = Help.Visible = false;
                lblHeader.Text += ResHelper.GetFileString("separationDB.Step2");
                break;
            // Finish step
            case 3:
                imgName = "header_separation_3.png";
                StartHelp.TopicName = Help.TopicName = "DBSeparation_StepFinish";
                lblHeader.Text += ResHelper.GetFileString("Install.Step7");
                break;
        }

        lblHeader.Text = string.Format(lblHeader.Text, wzdInstaller.ActiveStepIndex + 1);
        imgHeader.ImageUrl = GetImageUrl("Others/Install/" + imgName);
        imgHeader.AlternateText = "Header";
    }


    /// <summary>
    /// Checks if SQL settings on first page are correct.
    /// </summary>
    private bool CheckSqlSettings()
    {
        UserPassword = userServer.DBPassword;

        if (!userServer.ValidateForSeparation())
        {
            return false;
        }

        databaseDialog.AuthenticationType = userServer.WindowsAuthenticationChecked ? SQLServerAuthenticationModeEnum.WindowsAuthentication : SQLServerAuthenticationModeEnum.SQLServerAuthentication;
        databaseDialog.Password = UserPassword;
        databaseDialog.Username = userServer.DBUsername;
        databaseDialog.ServerName = userServer.ServerName;

        return true;
    }


    /// <summary>
    /// Runs appropriate action for specific wizard step.
    /// </summary>
    private void RunAction()
    {
        switch (wzdInstaller.ActiveStepIndex)
        {
            case 1:
                PreviousButton.Attributes.Add("disabled", "true");
                NextButton.Attributes.Add("disabled", "true");
                PersistentStorageHelper.RemoveValue(STORAGE_KEY);
                progress.StartLogging();
                asyncControl.RunAsync(StartSeparation, WindowsIdentity.GetCurrent());
                break;
        }
    }


    /// <summary>
    /// Launches separation process.
    /// </summary>
    private void StartSeparation(object parameter)
    {
        try
        {
            TakeSitesOffline();
            if (CreateDB())
            {
                SeparateDB();
            }
        }
        catch (Exception e)
        {
            progress.LogMessage(e.Message, CMSDatabaseHelper.MessageType.Error, false);
            asyncControl.RaiseError(this, null);
            throw;
        }
        finally
        {
            SystemHelper.DBSeparationInProgress = false;
        }
    }


    /// <summary>
    /// Creates new database.
    /// </summary>
    private bool CreateDB()
    {
        bool continueSeparation = true;
        if (databaseDialog.CreateNewChecked && !AzureHelper.IsRunningOnAzure)
        {
            progress.LogMessage(ResHelper.GetString("separationDB.creatingDB"), CMSDatabaseHelper.MessageType.Info, false);

            var connectionString = new SqlConnectionStringBuilder(databaseDialog.ConnectionString);
            connectionString.InitialCatalog = "";

            try
            {
                using (new CMSConnectionScope(connectionString.ConnectionString, false))
                {
                    CMSDatabaseHelper.CreateDatabase(databaseDialog.NewDatabaseName, connectionString.ConnectionString, null);
                }
            }
            catch (Exception ex)
            {
                string message = ResHelper.GetFileString("Intaller.LogErrorCreateDB") + " " + ex.Message;
                progress.LogMessage(message, CMSDatabaseHelper.MessageType.Error, false);
                continueSeparation = false;
            }
        }
        return continueSeparation;
    }


    /// <summary>
    /// Separates database.
    /// </summary>
    private void SeparateDB()
    {
        separation = new DBSeparationHelper();

        separation.InstallationConnStringSeparate = databaseDialog.ConnectionString;
        separation.InstallScriptsFolder = CMSDatabaseHelper.GetSQLInstallPathToObjects();
        separation.ScriptsFolder = Server.MapPath("~/App_Data/DBSeparation");
        separation.LogMessage = progress.LogMessage;

        separation.SeparateDatabase();

        progress.LogMessage(ResHelper.GetString("SeparationDB.OK"), CMSDatabaseHelper.MessageType.Finished, false);
    }


    /// <summary>
    /// Set connection string into web.config.
    /// </summary>
    private void SetConnectionString()
    {
        string connectionStringName = DBSeparationHelper.ConnStringSeparateName;
        if (!AzureHelper.IsRunningOnAzure)
        {
            if (!SettingsHelper.SetConnectionString(connectionStringName, databaseDialog.ConnectionString))
            {
                string connStringDisplay = ConnectionHelper.GetConnectionString(databaseDialog.AuthenticationType, databaseDialog.ServerName, databaseDialog.Database, databaseDialog.Username, databaseDialog.Password, 240, true);
                string resultStringDisplay = " <br/><br/><strong>&lt;add name=\"" + connectionStringName + "\" connectionString=\"" + connStringDisplay + "\"/&gt;</strong><br/><br/>";

                separationFinished.ErrorLabel.Visible = true;
                separationFinished.ErrorLabel.Text = ResHelper.GetFileString("Install.ConnectionStringError") + resultStringDisplay;
            }
        }
        else
        {
            string connStringValue = ConnectionHelper.GetConnectionString(databaseDialog.AuthenticationType, databaseDialog.ServerName, databaseDialog.Database, databaseDialog.Username, databaseDialog.Password, "English", 240, true, true);
            string connString = "&lt;add name=\"" + connectionStringName + "\" connectionString=\"" + connStringValue + "\"/&gt;";
            string appSetting = "&lt;Setting name=\"" + connectionStringName + "\" value=\"" + connStringValue + "\"/&gt;";

            separationFinished.AzureErrorLabel.Visible = true;
            separationFinished.AzureErrorLabel.Text = String.Format(ResHelper.GetFileString("Install.ConnectionStringAzure"), connString, appSetting);
            separationFinished.AzureErrorLabel.Text += GetManualCopyText();
        }

        if (PersistentStorageHelper.GetValueFromFile("SeparateDBSites") == null)
        {
            if (!separationFinished.ErrorLabel.Visible)
            {
                separationFinished.ErrorLabel.Text += "<br />";
            }
            else
            {
                separationFinished.ErrorLabel.Visible = true;
            }
            separationFinished.ErrorLabel.Text += ResHelper.GetFileString("separationdb.startsites");
        }
    }


    /// <summary>
    /// Returns text for azure containing tables which need to be manually copied.
    /// </summary>
    private string GetManualCopyText()
    {
        SeparatedTables tables = new SeparatedTables(Server.MapPath("~/App_Data/DBSeparation"), null);
        return ResHelper.GetString("separationDB.manualcopy") + tables.GetTableNames("<br />");
    }


    /// <summary>
    ///  Skips dialog directly to progress step.
    /// </summary>
    private void SetProgressStep()
    {
        if (!String.IsNullOrEmpty(ReturnUrl) && SystemHelper.DBSeparationInProgress)
        {
            wzdInstaller.ActiveStepIndex = 2;
            NextButton.Attributes.Add("disabled", "true");
            PreviousButton.Attributes.Add("disabled", "true");
            progress.StartLogging();
        }
    }


    /// <summary>
    /// Enables tasks after separation is over.
    /// </summary>
    private void EnableTasks()
    {
        if (ValidationHelper.GetBoolean(PersistentStorageHelper.GetValue("CMSSchedulerTasksEnabled"), false))
        {
            SettingsKeyProvider.SetValue("CMSSchedulerTasksEnabled", true);

            // Restart win service
            WinServiceItem def = WinServiceHelper.GetServiceDefinition(WinServiceHelper.HM_SERVICE_BASENAME);
            if (def != null)
            {
                WinServiceHelper.RestartService(def.GetServiceName());
            }
        }
        PersistentStorageHelper.RemoveValue("CMSSchedulerTasksEnabled");
    }


    /// <summary>
    /// Takes all sites offline.
    /// </summary>
    private void TakeSitesOffline()
    {
        var sites = SiteInfoProvider.GetSites("(SiteIsOffline IS NULL) OR (SiteIsOffline = 0)", null);
        var siteIDs = new List<int>(sites.Items.Count);
        foreach (var site in sites)
        {
            site.SiteIsOffline = true;
            SiteInfoProvider.SetSiteInfo(site);
            siteIDs.Add(site.SiteID);
        }
        PersistentStorageHelper.SetValue("SeparateDBSites", siteIDs);
    }


    /// <summary>
    /// Takes sites online.
    /// </summary>
    private void TakeSitesOnline()
    {
        var siteIDs = (List<int>)PersistentStorageHelper.GetValue("SeparateDBSites");
        if ((siteIDs != null) && (siteIDs.Count > 0))
        {
            foreach (var siteID in siteIDs)
            {
                SiteInfo site = SiteInfoProvider.GetSiteInfo(siteID);
                if (site != null)
                {
                    site.SiteIsOffline = false;
                    SiteInfoProvider.SetSiteInfo(site);
                }
            }
        }
        PersistentStorageHelper.RemoveValue("SeparateDBSites");
    }


    /// <summary>
    /// Registers JavaScript.
    /// </summary>
    private void RegisterJS()
    {
        ScriptHelper.RegisterWOpenerScript(Page);

        // Register refresh script to refresh wopener
        var script = new StringBuilder();
        script.Append(@"
function RefreshParent() {
  if ((wopener != null) && (wopener.RefreshPage != null)) {
      wopener.RefreshPage();
    }
}");
        // Register script
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "WOpenerRefresh", ScriptHelper.GetScript(script.ToString()));
    }

    #endregion


    #region "Event handlers"

    /// <summary>
    /// Thread with separation finished with exception.
    /// </summary>
    void asyncControl_OnError(object sender, EventArgs e)
    {
        if (asyncControl.Worker.LastException != null)
        {
            progress.LogMessage(asyncControl.Worker.LastException.Message, CMSDatabaseHelper.MessageType.Error, false);
        }
        PreviousButton.Attributes.Remove("disabled");
    }


    /// <summary>
    /// Thread with separation finished successfully.
    /// </summary>
    void asyncControl_OnFinished(object sender, EventArgs e)
    {
        NextButton.Attributes.Remove("disabled");
        wzdInstaller.ActiveStepIndex++;
        SetConnectionString();
    }

    #endregion
}