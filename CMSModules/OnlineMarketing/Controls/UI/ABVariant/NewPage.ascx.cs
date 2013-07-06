using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.OnlineMarketing;
using CMS.PortalEngine;
using CMS.DocumentEngine;
using CMS.UIControls;

using TreeNode = CMS.DocumentEngine.TreeNode;

public partial class CMSModules_OnlineMarketing_Controls_UI_ABVariant_NewPage : ContentActionsControl
{
    #region "Variables"

    protected int nodeId = 0;
    protected TreeNode node = null;
    protected TreeProvider tree = null;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets the error label.
    /// </summary>
    public Label ErrorLabel
    {
        get
        {
            EnsureChildControls();
            return lblError;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        rfvDocumentName.ErrorMessage = GetString("om.enterdocumentname");
        nodeId = QueryHelper.GetInteger("parentnodeid", 0);
        ucABTestSelector.NodeID = nodeId;
        // Get alias path
        tree = new TreeProvider(CMSContext.CurrentUser);
        node = tree.SelectSingleNode(nodeId);

        if (!URLHelper.IsPostback())
        {
            if (node != null)
            {
                ucPath.Value = node.NodeAliasPath;
            }
        }
    }


    /// <summary>
    /// PreRender event handler.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        ScriptHelper.RegisterShortcuts(Page);

        if (!String.IsNullOrEmpty(lblError.Text))
        {
            lblError.Visible = true;
        }
    }


    /// <summary>
    /// Show error message.
    /// </summary>
    /// <param name="errorMessage">Error message</param>
    protected override void AddError(string errorMessage)
    {
        lblError.Visible = true;
        lblError.Text = errorMessage;
    }


    /// <summary>
    /// Creates document.
    /// </summary>
    /// <param name="createAnother">If false user will be redirected to created document</param>
    public int Save(bool createAnother)
    {
        // Validate input data
        string message = new Validator().NotEmpty(txtDocumentName.Text.Trim(), GetString("om.enterdocumentname")).Result;

        if (message == String.Empty)
        {
            if (node != null)
            {
                // Select parent node
                TreeNode parent = tree.SelectSingleNode(CMSContext.CurrentSiteName, ucPath.Value.ToString(), TreeProvider.ALL_CULTURES, false, null, false);
                if (parent != null)
                {
                    // Check security
                    if (!CMSContext.CurrentUser.IsAuthorizedToCreateNewDocument(parent.NodeID, node.NodeClassName))
                    {
                        RedirectToAccessDenied(GetString("cmsdesk.notauthorizedtocreatedocument"));
                        return 0;
                    }
                    TreeNode newNode = ProcessAction(node, parent, "copynode", false, true, true);

                    if (newNode != null)
                    {
                        newNode.SetValue("DocumentMenuItemHideInNavigation", !chkShowInNavigation.Checked);
                        newNode.SetValue("DocumentShowInSiteMap", chkShowInSiteMap.Checked);
                        newNode.SetValue("DocumentSearchExcluded", chkExcludeFromSearch.Checked);
                        // Limit length to 100 characters
                        string nodeAlias = TextHelper.LimitLength(txtDocumentName.Text.Trim(), 100, String.Empty);
                        newNode.NodeAlias = nodeAlias;
                        newNode.DocumentName = nodeAlias;

                        // Update menu item name
                        newNode.SetDocumentNameSource(nodeAlias);

                        newNode.Update();

                        // If ABTest selected - create new variant
                        int abTestID = ValidationHelper.GetInteger(ucABTestSelector.Value, 0);
                        if (abTestID != 0)
                        {
                            ABTestInfo abTest = ABTestInfoProvider.GetABTestInfo(abTestID);
                            if (abTest != null)
                            {
                                string defaultCodeName = TextHelper.LimitLength(ValidationHelper.GetCodeName(newNode.GetDocumentName()), 45, String.Empty);
                                string codeName = defaultCodeName;
                                ABVariantInfo info = ABVariantInfoProvider.GetABVariantInfo(codeName, abTest.ABTestName, CMSContext.CurrentSiteName);

                                // Find non existing variant code name 
                                int index = 0;
                                while (info != null)
                                {
                                    index++;
                                    codeName = defaultCodeName + "-" + index;
                                    info = ABVariantInfoProvider.GetABVariantInfo(codeName, abTest.ABTestName, CMSContext.CurrentSiteName);
                                }

                                // Save AB Variant 
                                ABVariantInfo variantInfo = new ABVariantInfo();
                                variantInfo.ABVariantTestID = abTestID;
                                variantInfo.ABVariantPath = newNode.NodeAliasPath;
                                variantInfo.ABVariantName = codeName;
                                variantInfo.ABVariantDisplayName = newNode.GetDocumentName();
                                variantInfo.ABVariantSiteID = CMSContext.CurrentSiteID;

                                ABVariantInfoProvider.SetABVariantInfo(variantInfo);
                            }
                        }

                        // Get the page mode
                        if (CMSContext.ViewMode != ViewModeEnum.EditLive)
                        {
                            CMSContext.ViewMode = ViewModeEnum.EditForm;
                        }

                        txtDocumentName.Text = String.Empty;
                        return newNode.NodeID;
                    }
                }
                else
                {
                    message = GetString("om.pathdoesnotexists");
                }
            }
        }

        if (message != String.Empty)
        {
            lblError.Visible = true;
            lblError.Text = message;
        }
        return 0;
    }

    #endregion
}