using System;
using System.Data;
using System.Collections;
using System.Security.Principal;
using System.Threading;

using CMS.CMSHelper;
using CMS.DataEngine;
using CMS.EventLog;
using CMS.GlobalHelper;
using CMS.LicenseProvider;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.Synchronization;
using CMS.DocumentEngine;
using CMS.UIControls;
using CMS.ExtendedControls;

[RegisterTitle("content.ui.propertiessecurity")]
public partial class CMSModules_Content_CMSDesk_Properties_Security : CMSPropertiesPage
{
    #region "Variables"

    protected AclProvider mAclProvider = null;
    private EventLogProvider mEventLog = null;
    protected SiteInfo currentSite = null;
    protected CurrentUserInfo currentUser = null;

    protected bool inheritsPermissions = false;
    protected string ipAddress = null;
    protected string eventUrl = null;

    protected static Hashtable mLogs = new Hashtable();
    protected static Hashtable mErrors = new Hashtable();
    protected static Hashtable mInfos = new Hashtable();

    #endregion


    #region "Properties"

    /// <summary>
    /// Access control list provider.
    /// </summary>
    public AclProvider AclProvider
    {
        get
        {
            return mAclProvider ?? (mAclProvider = new AclProvider(DocumentManager.Tree));
        }
        set
        {
            mAclProvider = value;
        }
    }


    /// <summary>
    /// Document name.
    /// </summary>
    protected string DocumentName
    {
        get
        {
            if (Node != null)
            {
                return Node.GetDocumentName();
            }
            return string.Empty;
        }
    }


    /// <summary>
    /// Event log provider.
    /// </summary>
    public EventLogProvider EventLog
    {
        get
        {
            return mEventLog ?? (mEventLog = new EventLogProvider());
        }
    }


    /// <summary>
    /// Current log context.
    /// </summary>
    public LogContext CurrentLog
    {
        get
        {
            return EnsureLog();
        }
    }


    /// <summary>
    /// Current Error.
    /// </summary>
    public string CurrentError
    {
        get
        {
            return ValidationHelper.GetString(mErrors["SyncError_" + ctlAsync.ProcessGUID], string.Empty);
        }
        set
        {
            mErrors["SyncError_" + ctlAsync.ProcessGUID] = value;
        }
    }


    /// <summary>
    /// Current Info.
    /// </summary>
    public string CurrentInfo
    {
        get
        {
            return ValidationHelper.GetString(mInfos["SyncInfo_" + ctlAsync.ProcessGUID], string.Empty);
        }
        set
        {
            mInfos["SyncInfo_" + ctlAsync.ProcessGUID] = value;
        }
    }

    #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        DocumentManager.OnCheckPermissions += DocumentManager_OnCheckPermissions;

        securityElem.Node = Node;
        securityElem.StopProcessing = pnlUIPermissionsPart.IsHidden;
        securityElem.GroupID = Node.GetIntegerValue("NodeGroupID");

        base.OnInit(e);

        pnlAccessPart.Visible = !(pnlUIAuth.IsHidden && pnlUISsl.IsHidden);

        if (!CMSContext.CurrentUser.IsAuthorizedPerUIElement("CMS.Content", "Properties.Security"))
        {
            RedirectToCMSDeskUIElementAccessDenied("CMS.Content", "Properties.Security");
        }

        // Redirect to information page when no UI elements displayed
        if (pnlUIAuth.IsHidden && pnlUISsl.IsHidden && pnlUIPermissionsPart.IsHidden)
        {
            RedirectToUINotAvailable();
        }
    }


    protected void DocumentManager_OnCheckPermissions(object sender, SimpleDocumentManagerEventArgs e)
    {
        e.CheckDefault = false;
        e.ErrorMessage = String.Format(GetString("cmsdesk.notauthorizedtoeditdocumentpermissions"), e.Node.NodeAliasPath);
        e.IsValid = CanModifyPermission(false, e.Node, currentUser);
    }


    protected void Page_Load(Object sender, EventArgs e)
    {
        currentSite = CMSContext.CurrentSite;
        currentUser = CMSContext.CurrentUser;

        ipAddress = HTTPHelper.UserHostAddress;
        eventUrl = HTTPHelper.GetAbsoluteUri();

        if (!RequestHelper.IsCallback())
        {
            btnCancel.Attributes.Add("onclick", ctlAsync.GetCancelScript(true) + "return false;");
            btnCancel.Text = GetString("General.Cancel");
            pnlLog.Visible = false;
            pnlPageContent.Visible = true;

            // Gets the node
            if (Node != null)
            {
                UIContext.PropertyTab = PropertyTabEnum.Security;

                // Check license
                if (DataHelper.GetNotEmpty(URLHelper.GetCurrentDomain(), string.Empty) != string.Empty)
                {
                    if (!LicenseKeyInfoProvider.IsFeatureAvailable(URLHelper.GetCurrentDomain(), FeatureEnum.DocumentLevelPermissions))
                    {
                        if (UIHelper.IsUnavailableUIHidden())
                        {
                            plcContainer.Visible = false;
                        }
                        else
                        {
                            pnlPermissions.Visible = false;
                            lblLicenseInfo.Visible = true;
                            lblLicenseInfo.Text = GetString("Security.NotAvailableInThisEdition");
                        }
                    }
                }

                // Initialize controls
                SetupControls();

                // Register scripts
                ScriptHelper.RegisterDialogScript(this);

                // Check if document inherits permissions and display info
                inheritsPermissions = AclProvider.DoesNodeInheritPermissions(Node.NodeID);
                lblInheritanceInfo.Text = inheritsPermissions ? GetString("Security.InheritsInfo.Inherits") : GetString("Security.InheritsInfo.DoesNotInherit");

                if (!RequestHelper.IsPostBack())
                {
                    // Set secured radio buttons
                    switch (Node.IsSecuredNode)
                    {
                        case 0:
                            radNo.Checked = true;
                            break;

                        case 1:
                            radYes.Checked = true;
                            break;

                        default:
                            if (Node.NodeParentID == 0)
                            {
                                radNo.Checked = true;
                            }
                            else
                            {
                                radParent.Checked = true;
                            }
                            break;
                    }

                    // Set secured radio buttons
                    switch (Node.RequiresSSL)
                    {
                        case 0:
                            radNoSSL.Checked = true;
                            break;

                        case 1:
                            radYesSSL.Checked = true;
                            break;

                        case 2:
                            radNeverSSL.Checked = true;
                            break;

                        default:
                            if (Node.NodeParentID == 0)
                            {
                                radNoSSL.Checked = true;
                            }
                            else
                            {
                                radParentSSL.Checked = true;
                            }
                            break;
                    }
                }

                // Hide link to the inheritance settings if this is the root node
                if (Node.NodeParentID == 0)
                {
                    plcAuthParent.Visible = false;
                    plcSSLParent.Visible = false;
                    lnkInheritance.Visible = false;
                }
            }
            else
            {
                pnlPageContent.Visible = false;
            }
        }

        ctlAsync.OnFinished += ctlAsync_OnFinished;
        ctlAsync.OnError += ctlAsync_OnError;
        ctlAsync.OnRequestLog += ctlAsync_OnRequestLog;
        ctlAsync.OnCancel += ctlAsync_OnCancel;

        pnlPageContent.Enabled = !DocumentManager.ProcessingAction;
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        CheckModifyPermission(false);

        if (pnlAccessPart.Visible)
        {
            pnlAccessPart.Visible = !(pnlUIAuth.IsHidden && pnlUISsl.IsHidden);
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Checks if current use can modify  the permission.
    /// </summary>
    /// <param name="redirect">If true and can't modify the user is redirected to denied page</param>
    private void CheckModifyPermission(bool redirect)
    {
        CanModifyPermission(redirect, Node, currentUser);
    }


    /// <summary>
    /// Checks if current use can modify  the permission.
    /// </summary>
    /// <param name="redirect">If true and can't modify the user is redirected to denied page</param>
    /// <param name="currentNode">Current node</param>
    /// <param name="user">Current user</param>    
    private bool CanModifyPermission(bool redirect, TreeNode currentNode, CurrentUserInfo user)
    {
        bool hasPermission = false;
        if (currentNode != null)
        {
            hasPermission = (user.IsAuthorizedPerDocument(currentNode, NodePermissionsEnum.ModifyPermissions) == AuthorizationResultEnum.Allowed);

            // If hasn't permission and redirect enabled
            if (!hasPermission)
            {
                if (redirect)
                {
                    RedirectToAccessDenied(String.Format(GetString("cmsdesk.notauthorizedtoeditdocumentpermissions"), currentNode.NodeAliasPath));
                }
                else
                {
                    pnlAccessPart.Enabled = false;
                    btnRadOk.Enabled = false;
                    pnlInheritance.Enabled = false;
                    lnkInheritance.Visible = false;
                }
            }
        }
        return hasPermission;
    }


    /// <summary>
    /// Initializes components.
    /// </summary>
    private void SetupControls()
    {
        // Set labels
        pnlPermissionsPart.GroupingText = GetString("Security.Permissions");
        pnlInheritance.GroupingText = GetString("Security.Permissions");
        pnlAccessPart.GroupingText = GetString("Security.Access");
    }


    /// <summary>
    /// Switches back to default layout (after some action).
    /// </summary>
    private void SwitchBackToPermissionsMode()
    {
        plcContainer.Visible = true;
        pnlAccessPart.Visible = true;
        pnlInheritance.Visible = false;
    }

    #endregion


    #region "Events"

    /// <summary>
    /// Displays inheritance settings.
    /// </summary>
    protected void lnkInheritance_Click(Object sender, EventArgs e)
    {
        // Check permission
        CheckModifyPermission(true);

        plcContainer.Visible = false;
        pnlAccessPart.Visible = false;
        pnlInheritance.Visible = true;

        // Test if current document inherits permissions
        if (inheritsPermissions)
        {
            plcBreakClear.Visible = true;
            plcBreakCopy.Visible = true;
            plcRestore.Visible = false;
        }
        else
        {
            plcBreakClear.Visible = false;
            plcBreakCopy.Visible = false;
            plcRestore.Visible = true;
        }
    }


    protected void btnCancelAction_Click(Object sender, EventArgs e)
    {
        SwitchBackToPermissionsMode();
    }


    protected void lnkBreakWithCopy_Click(Object sender, EventArgs e)
    {
        // Check permission
        CheckModifyPermission(true);

        // Break permission inheritance and copy parent permissions
        AclProvider.BreakInherintance(Node, true);

        // Log staging task
        TaskParameters taskParam = new TaskParameters();
        taskParam.SetParameter("copyPermissions", true);
        DocumentSynchronizationHelper.LogDocumentChange(Node, TaskTypeEnum.BreakACLInheritance, Node.TreeProvider, SynchronizationInfoProvider.ENABLED_SERVERS, taskParam, Node.TreeProvider.AllowAsyncActions);

        // Insert information about this event to event log.
        if (DocumentManager.Tree.LogEvents)
        {
            EventLog.LogEvent(EventLogProvider.EVENT_TYPE_INFORMATION, DateTime.Now, "Content", "DOCPERMISSIONSMODIFIED", DocumentManager.Tree.UserInfo.UserID, DocumentManager.Tree.UserInfo.UserName, Node.NodeID, DocumentName, ipAddress, ResHelper.GetAPIString("security.documentpermissionsbreakcopy", "Inheritance of the parent document permissions have been broken. Parent document permissions have been copied."), Node.NodeSiteID, eventUrl);
        }

        lblInheritanceInfo.Text = GetString("Security.InheritsInfo.DoesNotInherit");
        SwitchBackToPermissionsMode();

        // Clear and reload
        securityElem.InvalidateAcls();
        securityElem.LoadOperators(true);
    }


    protected void lnkBreakWithClear_Click(Object sender, EventArgs e)
    {
        // Check permission
        CheckModifyPermission(true);

        // Break permission inheritance and clear permissions
        AclProvider.BreakInherintance(Node, false);

        // Log staging task and flush cache
        DocumentSynchronizationHelper.LogDocumentChange(Node, TaskTypeEnum.BreakACLInheritance, Node.TreeProvider, SynchronizationInfoProvider.ENABLED_SERVERS, null, Node.TreeProvider.AllowAsyncActions);
        CacheHelper.TouchKeys(TreeProvider.GetDependencyCacheKeys(Node, Node.NodeSiteName));

        // Insert information about this event to event log.
        if (DocumentManager.Tree.LogEvents)
        {
            EventLog.LogEvent(EventLogProvider.EVENT_TYPE_INFORMATION, DateTime.Now, "Content", "DOCPERMISSIONSMODIFIED", currentUser.UserID, currentUser.UserName, Node.NodeID, DocumentName, ipAddress, ResHelper.GetAPIString("security.documentpermissionsbreakclear", "Inheritance of the parent document permissions have been broken."), Node.NodeSiteID, eventUrl);
        }

        lblInheritanceInfo.Text = GetString("Security.InheritsInfo.DoesNotInherit");
        SwitchBackToPermissionsMode();

        // Clear and reload
        securityElem.InvalidateAcls();
        securityElem.LoadOperators(true);
    }


    protected void lnkRestoreInheritance_Click(Object sender, EventArgs e)
    {
        ResetNodePermission(currentSite.SiteName, Node.NodeAliasPath, false, currentUser, null);

        lblInheritanceInfo.Text = GetString("Security.InheritsInfo.Inherits");
        SwitchBackToPermissionsMode();

        // Clear and reload
        securityElem.InvalidateAcls();
        securityElem.LoadOperators(true);
    }


    protected void lnkRestoreInheritanceRecursively_Click(object sender, EventArgs e)
    {
        // Setup design
        pnlLog.Visible = true;
        pnlPageContent.Visible = false;
        titleElem.TitleText = GetString("cmsdesk.restoringpermissioninheritance");
        titleElem.TitleImage = GetImageUrl("/CMSModules/CMS_Content/Properties/restoreinheritance.png");

        CurrentLog.Close();
        EnsureLog();
        CurrentError = string.Empty;
        CurrentInfo = string.Empty;

        // Recursively
        ctlAsync.Parameter = CMSContext.CurrentUser;
        ctlAsync.RunAsync(ResetNodePermission, WindowsIdentity.GetCurrent());

        lblInheritanceInfo.Text = GetString("Security.InheritsInfo.Inherits");
        SwitchBackToPermissionsMode();

        // Clear and reload
        securityElem.InvalidateAcls();
        securityElem.LoadOperators(true);
    }


    /// <summary>
    /// Async reset node action.
    /// </summary>
    /// <param name="parameter">Accepts CurrentUserInfo</param>
    protected void ResetNodePermission(object parameter)
    {
        CurrentUserInfo user = parameter as CurrentUserInfo;
        if (user != null)
        {
            // Add information to log
            AddLog(GetString("cmsdesk.restoringpermissioninheritance"));
            TreeProvider tr = new TreeProvider(user);
            ResetNodePermission(currentSite.SiteName, Node.NodeAliasPath, true, user, tr);
        }
    }


    /// <summary>
    /// Resets permission inheritance of node and its children.
    /// </summary>
    /// <param name="siteName">Name of site</param>
    /// <param name="nodeAliasPath">Alias path</param>
    /// <param name="recursive">Indicates whether to recursively reset all nodes below the current node</param>
    /// <param name="user">Current user</param>
    /// <param name="tr">Tree provider</param>
    /// <returns>Whether TRUE if no permission conflict has occurred</returns>
    private bool ResetNodePermission(string siteName, string nodeAliasPath, bool recursive, CurrentUserInfo user, TreeProvider tr)
    {
        // Check permissions
        bool permissionsResult = false;
        try
        {
            if (tr == null)
            {
                tr = new TreeProvider(user);
            }
            // Get node by alias path
            TreeNode treeNode = tr.SelectSingleNode(siteName, nodeAliasPath, null, true, null, false);
            permissionsResult = CanModifyPermission(!recursive, treeNode, user);

            if (treeNode != null)
            {
                // If user has permissions
                if (permissionsResult)
                {
                    // Break inheritance of a node
                    if (!AclProvider.DoesNodeInheritPermissions(treeNode.NodeID))
                    {
                        // Restore inheritance of a node
                        AclProvider.RestoreInheritance(treeNode);

                        // Log current encoded alias path
                        AddLog(HTMLHelper.HTMLEncode(nodeAliasPath));

                        // Log staging task and flush cache
                        DocumentSynchronizationHelper.LogDocumentChange(treeNode, TaskTypeEnum.RestoreACLInheritance, treeNode.TreeProvider, SynchronizationInfoProvider.ENABLED_SERVERS, null, treeNode.TreeProvider.AllowAsyncActions);
                        CacheHelper.TouchKeys(TreeProvider.GetDependencyCacheKeys(Node, Node.NodeSiteName));

                        // Insert information about this event to event log.
                        if (DocumentManager.Tree.LogEvents)
                        {
                            if (recursive)
                            {
                                LogContext.LogEvent(EventLogProvider.EVENT_TYPE_INFORMATION, DateTime.Now, "Content", "DOCPERMISSIONSMODIFIED", user.UserID, user.UserName, treeNode.NodeID, treeNode.GetDocumentName(), ipAddress, string.Format(ResHelper.GetAPIString("security.documentpermissionsrestoredfordoc", "Permissions of document '{0}' have been restored to the parent document permissions."), nodeAliasPath), Node.NodeSiteID, null, null, null, null);
                            }
                            else
                            {
                                EventLog.LogEvent(EventLogProvider.EVENT_TYPE_INFORMATION, DateTime.Now, "Content", "DOCPERMISSIONSMODIFIED", user.UserID, user.UserName, treeNode.NodeID, treeNode.GetDocumentName(), ipAddress, ResHelper.GetAPIString("security.documentpermissionsrestored", "Permissions have been restored to the parent document permissions."), Node.NodeSiteID, eventUrl);
                            }
                        }
                    }
                    else
                    {
                        AddLog(string.Format(ResHelper.GetString("cmsdesk.skippingrestoring"), HTMLHelper.HTMLEncode(nodeAliasPath)));
                    }
                }

                // Recursively reset node inheritance
                if (recursive)
                {
                    // Get child nodes of current node
                    DataSet ds = DocumentManager.Tree.SelectNodes(siteName, treeNode.NodeAliasPath.TrimEnd('/') + "/%", TreeProvider.ALL_CULTURES, true, null, null, null, 1, false, -1, TreeProvider.SELECTNODES_REQUIRED_COLUMNS + ",NodeAliasPath");
                    if (!DataHelper.DataSourceIsEmpty(ds))
                    {
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            string childNodeAliasPath = ValidationHelper.GetString(dr["NodeAliasPath"], string.Empty);

                            if (!string.IsNullOrEmpty(childNodeAliasPath))
                            {
                                bool tempPermissionsResult = ResetNodePermission(siteName, childNodeAliasPath, true, user, tr);
                                permissionsResult = tempPermissionsResult && permissionsResult;
                            }
                        }
                    }
                }
            }
        }
        catch (ThreadAbortException ex)
        {
            string state = ValidationHelper.GetString(ex.ExceptionState, string.Empty);
            if (state == CMSThread.ABORT_REASON_STOP)
            {
                // When canceled
                CurrentInfo = ResHelper.GetString("cmsdesk.restoringcanceled");
                AddLog(CurrentInfo);
            }
            else
            {
                // Log error
                CurrentError = ResHelper.GetString("cmsdesk.restoringfailed") + ": " + ex.Message;
                AddLog(CurrentError);
            }
        }
        catch (Exception ex)
        {
            // Log error
            CurrentError = ResHelper.GetString("cmsdesk.restoringfailed") + ": " + ex.Message;
            AddLog(CurrentError);
        }
        return permissionsResult;
    }


    /// <summary>
    /// On click OK save secured settings.
    /// </summary>
    protected void btnRadOk_Click(object sender, EventArgs e)
    {
        // Check permission
        CheckModifyPermission(true);

        if (Node != null)
        {
            string message = null;
            bool clearCache = false;
            // Authentication
            if (!pnlUIAuth.IsHidden)
            {
                int isSecuredNode = Node.IsSecuredNode;

                if (radYes.Checked)
                {
                    isSecuredNode = 1;
                }
                else if (radNo.Checked)
                {
                    isSecuredNode = 0;
                }
                else if (radParent.Checked)
                {
                    isSecuredNode = -1;
                }

                // Set secured areas settings
                if (isSecuredNode != Node.IsSecuredNode)
                {
                    Node.IsSecuredNode = isSecuredNode;
                    clearCache = true;
                    message += ResHelper.GetAPIString("security.documentaccessauthchanged", "Document authentication settings have been modified.");
                }
            }

            // SSL
            if (!pnlUISsl.IsHidden)
            {
                int requiresSSL = Node.RequiresSSL;

                if (radYesSSL.Checked)
                {
                    requiresSSL = 1;
                }
                else if (radNoSSL.Checked)
                {
                    requiresSSL = 0;
                }
                else if (radParentSSL.Checked)
                {
                    requiresSSL = -1;
                }
                else if (radNeverSSL.Checked)
                {
                    requiresSSL = 2;
                }

                // Set SSL settings
                if (requiresSSL != Node.RequiresSSL)
                {
                    Node.RequiresSSL = requiresSSL;
                    clearCache = true;
                    if (message != null)
                    {
                        message += "<br />";
                    }
                    message += ResHelper.GetAPIString("security.documentaccesssslchanged", "Document SSL settings have been modified.");
                }
            }

            DocumentManager.UpdateDocument(false);

            // Insert information about this event to event log.
            if (DocumentManager.Tree.LogEvents && (message != null))
            {
                EventLog.LogEvent(EventLogProvider.EVENT_TYPE_INFORMATION, DateTime.Now, "Content", "DOCACCESSMODIFIED", currentUser.UserID, currentUser.UserName, Node.NodeID, DocumentName, ipAddress, message, Node.NodeSiteID, eventUrl);
            }

            // Clear cache if security settings changed
            if (clearCache)
            {
                CacheHelper.ClearPageInfoCache(Node.NodeSiteName);
                CacheHelper.ClearFileNodeCache(Node.NodeSiteName);
            }
        }
    }

    #endregion


    #region "Async processing"

    protected void ctlAsync_OnRequestLog(object sender, EventArgs e)
    {
        ctlAsync.Log = CurrentLog.Log;
    }


    protected void ctlAsync_OnError(object sender, EventArgs e)
    {
        CurrentLog.Close();
        securityElem.LoadOperators(true);
        securityElem.ErrorLabel.Text = CurrentError;
        securityElem.InfoLabel.Text = CurrentInfo;
    }


    protected void ctlAsync_OnFinished(object sender, EventArgs e)
    {
        CurrentLog.Close();
        securityElem.LoadOperators(true);
        securityElem.ErrorLabel.Text = CurrentError;
        securityElem.InfoLabel.Text = CurrentInfo;
    }


    protected void ctlAsync_OnCancel(object sender, EventArgs e)
    {
        CurrentLog.Close();
        securityElem.LoadOperators(true);
        securityElem.ErrorLabel.Text = CurrentError;
        securityElem.InfoLabel.Text = CurrentInfo;
    }

    #endregion


    #region "Log handling"

    /// <summary>
    /// Adds the log information.
    /// </summary>
    /// <param name="newLog">New log information</param>
    protected void AddLog(string newLog)
    {
        EnsureLog();
        LogContext.AppendLine(newLog);
    }


    /// <summary>
    /// Adds the log error.
    /// </summary>
    /// <param name="newLog">New log information</param>
    protected void AddErrorLog(string newLog)
    {
        AddErrorLog(newLog, null);
    }


    /// <summary>
    /// Adds the log error.
    /// </summary>
    /// <param name="newLog">New log information</param>
    /// <param name="errorMessage">Error message</param>
    protected void AddErrorLog(string newLog, string errorMessage)
    {
        LogContext.AppendLine(newLog);
    }


    /// <summary>
    /// Ensures the logging context.
    /// </summary>
    protected LogContext EnsureLog()
    {
        LogContext log = LogContext.EnsureLog(ctlAsync.ProcessGUID);
        log.Reversed = true;
        log.LineSeparator = "<br />";
        return log;
    }

    #endregion
}