using System;
using System.Collections;

using CMS.DataEngine;
using CMS.FormControls;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.UIControls;

public partial class CMSModules_BizForms_FormControls_Cloning_CMS_FormSettings : CloneSettingsControl
{
    #region "Properties"

    /// <summary>
    /// Gets properties hashtable.
    /// </summary>
    public override Hashtable CustomParameters
    {
        get
        {
            return GetProperties();
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Returns true if custom settings are valid against given clone setting.
    /// </summary>
    /// <param name="settings">Clone settings</param>
    public override bool IsValid(CloneSettings settings)
    {
        if (!ValidationHelper.IsIdentifier(txtTableName.Text))
        {
            ShowError(GetString("BizForm_Edit.ErrorFormTableNameInIdentifierFormat"));
            return false;
        }
        return true;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        lblCloneItems.ToolTip = GetString("clonning.settings.form.tooltip");
        lblCloneAlternativeForms.ToolTip = GetString("clonning.settings.class.alternativeform");

        if (!RequestHelper.IsPostBack())
        {
            DataClassInfo classInfo = DataClassInfoProvider.GetDataClass(InfoToClone.GetIntegerValue("FormClassID", 0));
            if (classInfo != null)
            {
                TableManager tm = new TableManager(null);

                txtTableName.Text = tm.GetUniqueTableName(classInfo.ClassTableName);
            }
        }
    }


    /// <summary>
    /// Returns properties hashtable.
    /// </summary>
    private Hashtable GetProperties()
    {
        Hashtable result = new Hashtable();
        result[SettingsObjectType.CLASS + ".data"] = chkCloneItems.Checked;
        result[SettingsObjectType.CLASS + ".tablename"] = txtTableName.Text;
        result[SettingsObjectType.CLASS + ".alternativeforms"] = chkCloneAlternativeForms.Checked;
        return result;
    }

    #endregion
}