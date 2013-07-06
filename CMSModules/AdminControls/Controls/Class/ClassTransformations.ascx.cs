using System;
using System.Data;

using CMS.ExtendedControls;
using CMS.GlobalHelper;
using CMS.PortalEngine;
using CMS.SettingsProvider;
using CMS.UIControls;
using CMS.VirtualPathHelper;

public partial class CMSModules_AdminControls_Controls_Class_ClassTransformations : CMSUserControl
{
    #region "Private fields"

    private int mClassID = 0;
    private string mEditPageUrl = null;
    private bool mIsSiteManager = false;

    #endregion


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
    /// Gets or sets the class id.
    /// </summary>
    public int ClassID
    {
        get
        {
            return mClassID;
        }
        set
        {
            mClassID = value;
        }
    }


    /// <summary>
    /// URL of the page holding the editing tasks.
    /// </summary>
    public string EditPageUrl
    {
        get
        {
            return mEditPageUrl;
        }
        set
        {
            mEditPageUrl = value;
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
            uniGrid.IsLiveSite = value;
            plcMess.IsLiveSite = value;
            base.IsLiveSite = value;
        }
    }


    /// <summary>
    /// Indicate whether is site manager
    /// </summary>
    public bool IsSiteManager
    {
        get
        {
            return mIsSiteManager;
        }
        set
        {
            mIsSiteManager = value;
        }
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
        }
        else
        {
             // Check whether virtual objects are allowed
            if (!VirtualPathHelper.VirtualObjectsAllowed)
            {
                ShowWarning(GetString("VirtualPathProvider.NotRunning"), null, null);
            }

            // Initialize the controls
            uniGrid.OnAction += new OnActionEventHandler(uniGrid_OnAction);
            uniGrid.OnExternalDataBound += new OnExternalDataBoundEventHandler(uniGrid_OnExternalDataBound);
            uniGrid.GridName = "~/CMSModules/AdminControls/Controls/Class/ClassTransformations.xml";
            uniGrid.ZeroRowsText = GetString("general.nodatafound");
            uniGrid.IsLiveSite = IsLiveSite;

            // If the ClassID was specified
            if (ClassID > 0)
            {
                uniGrid.WhereCondition = "TransformationClassID=" + ClassID;
            }
            else
            {
                // Otherwise hide the UniGrid to avoid unexpected behavior
                uniGrid.Visible = false;
            }
        }
    }


    #region "GridView handling"

    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that throws event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void uniGrid_OnAction(string actionName, object actionArgument)
    {
        if (actionName.ToLowerCSafe() == "edit")
        {
            int transID = ValidationHelper.GetInteger(actionArgument, 0);
            if (transID != 0)
            {
                TransformationInfo ti = TransformationInfoProvider.GetTransformation(transID);
                if (ti != null)
                {
                    if (ti.TransformationIsHierarchical)
                    {
                        string isManager = IsSiteManager ? "&sitemanager=true" : String.Empty;
                        URLHelper.Redirect(ResolveUrl("~/CMSModules/DocumentTypes/Pages/Development/HierarchicalTransformations_Frameset.aspx?transid=" + transID + "&classid=" + ClassID.ToString() + "&tabmode=1" + isManager));
                    }
                    else
                    {
                        URLHelper.Redirect(EditPageUrl + "?transformationid=" + transID.ToString() + "&classid=" + ClassID.ToString() + "&hash=" + QueryHelper.GetHash("?transformationid=" + transID + "&classid=" + ClassID));
                    }
                }
            }
        }
        else if (actionName.ToLowerCSafe() == "delete")
        {
            int transformationId = ValidationHelper.GetInteger(actionArgument, -1);
            if (transformationId > 0)
            {
                TransformationInfoProvider.DeleteTransformation(transformationId);
            }
        }
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Initializes the controls on the page.
    /// </summary>
    private void SetupControl()
    {
    }


    protected object uniGrid_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName)
        {
            case "transformationtype":
                DataRowView dr = (DataRowView)parameter;
                bool isHierarchical = ValidationHelper.GetBoolean(dr["TransformationIsHierarchical"], false);
                string type = ValidationHelper.GetString(dr["TransformationType"], String.Empty);
                if (isHierarchical)
                {
                    return GetString("transformation.hierarchical");
                }
                switch (type.ToLowerCSafe())
                {
                    case "ascx":
                        return "ASCX";

                    case "text":
                        return "Text / XML";

                    case "xslt":
                        return "XSLT";

                    case "html":
                        return "HTML";

                    case "jquery":
                        return "jQuery";
                }
                break;
        }
        return parameter;
    }

    #endregion
}