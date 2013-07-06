using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.FormControls;
using CMS.UIControls;

public partial class CMSModules_PortalEngine_FormControls_PageLayouts_PageLayoutSelector : FormEngineUserControl
{
    #region "Properties"

    /// <summary>
    /// Whether control is enabled.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return uniselect.Enabled;
        }
        set
        {
            uniselect.Enabled = value;
        }
    }


    /// <summary>
    /// Value.
    /// </summary>
    public override object Value
    {
        get
        {
            return uniselect.Value;
        }
        set
        {
            uniselect.Value = value;
        }
    }


    /// <summary>
    /// Gets the UniSelector control
    /// </summary>
    public UniSelector UniSelector
    {
        get
        {
            return uniselect;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Page load event.
    /// </summary>    
    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            uniselect.StopProcessing = true;
            return;
        }

        uniselect.IconPath = GetImageUrl("Objects/CMS_Layout/object.png");
        uniselect.DropDownSingleSelect.AutoPostBack = true;
        uniselect.IsLiveSite = IsLiveSite;
    }


    /// <summary>
    /// On changed.
    /// </summary>
    protected void uniselect_OnSelectionChanged(object sender, EventArgs e)
    {
        RaiseOnChanged();
    }

    #endregion
}