using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.FormEngine;
using CMS.GlobalHelper;
using CMS.UIControls;

public partial class CMSModules_DocumentTypes_Pages_AlternativeForms_AlternativeForms_Fields : SiteManagerPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        CurrentMaster.BodyClass += " FieldEditorBody";
        int altFormId = QueryHelper.GetInteger("altformid", 0);

        altFormFieldEditor.Mode = FieldEditorModeEnum.ClassFormDefinition;
        altFormFieldEditor.AlternativeFormID = altFormId;
        altFormFieldEditor.DisplayedControls = FieldEditorControlsEnum.DocumentTypes;
        altFormFieldEditor.EnableSystemFields = true;
    }
}