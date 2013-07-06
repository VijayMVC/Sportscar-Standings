using System;
using System.Collections;

using CMS.FormControls;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.MediaLibrary;
using CMS.IO;
using CMS.UIControls;

public partial class CMSModules_MediaLibrary_FormControls_Cloning_Media_LibrarySettings : CloneSettingsControl
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

    protected void Page_Load(object sender, EventArgs e)
    {
        lblFiles.ToolTip = GetString("clonning.settings.medialibrary.files.tooltip");
        lblFolderName.ToolTip = GetString("clonning.settings.medialibrary.foldername.tooltip");

        if (!RequestHelper.IsPostBack())
        {
            string originalPath = MediaLibraryInfoProvider.GetMediaLibraryFolderPath(InfoToClone.Generalized.ObjectID);
            txtFolderName.Text = DirectoryInfo.New(FileHelper.GetUniqueDirectoryName(originalPath)).Name;
        }
    }


    /// <summary>
    /// Returns properties hashtable.
    /// </summary>
    private Hashtable GetProperties()
    {
        Hashtable result = new Hashtable();
        result[MediaLibraryObjectType.MEDIALIBRARY + ".foldername"] = txtFolderName.Text;
        result[MediaLibraryObjectType.MEDIALIBRARY + ".files"] = chkFiles.Checked;
        return result;
    }

    #endregion
}