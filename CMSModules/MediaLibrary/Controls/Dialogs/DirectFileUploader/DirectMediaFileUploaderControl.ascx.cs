using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Data;

using CMS.CMSHelper;
using CMS.DataEngine;
using CMS.EventLog;
using CMS.ExtendedControls;
using CMS.GlobalHelper;
using CMS.IO;
using CMS.MediaLibrary;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.Synchronization;
using CMS.UIControls;

public partial class CMSModules_MediaLibrary_Controls_Dialogs_DirectFileUploader_DirectMediaFileUploaderControl : DirectFileUploader
{
    #region "Variables"

    private MediaLibraryInfo mLibraryInfo = null;

    #endregion


    #region "Private properties"

    /// <summary>
    /// Library info uploaded file is related to.
    /// </summary>
    private MediaLibraryInfo LibraryInfo
    {
        get
        {
            if (LibraryID > 0)
            {
                mLibraryInfo = MediaLibraryInfoProvider.GetMediaLibraryInfo(LibraryID);
            }
            return mLibraryInfo;
        }
        set
        {
            mLibraryInfo = value;
        }
    }

    #endregion


    #region "Public properties"

    /// <summary>
    /// Uploaded file.
    /// </summary>
    public override HttpPostedFile PostedFile
    {
        get
        {
            return ucFileUpload.PostedFile;
        }
    }


    /// <summary>
    /// File upload user control.
    /// </summary>
    public override CMSFileUpload FileUploadControl
    {
        get
        {
            return ucFileUpload;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            // Do nothing
            Visible = false;
        }
        else
        {
            Page.Error += new EventHandler(Page_Error);

            // Initialize uploader
            FileUploadControl.Attributes.Add("class", "fileUpload");
            FileUploadControl.Attributes.Add("style", "cursor: pointer;");
            FileUploadControl.Attributes.Add("onchange", String.Format("if (typeof(parent.DFU) !== 'undefined') {{ parent.DFU.OnUploadBegin('{0}'); {1}; }}", QueryHelper.GetString("containerid", ""), Page.ClientScript.GetPostBackEventReference(btnHidden, null, false)));

            // DFU init script
            string dfuScript = String.Format("if (typeof(parent.DFU) !== 'undefined'){{parent.DFU.init(document.getElementById('{0}')); window.resize = parent.DFU.init(document.getElementById('{0}'));}}", ucFileUpload.ClientID);
            ScriptHelper.RegisterStartupScript(this, typeof(string), "DFUScript_" + ucFileUpload.ClientID, ScriptHelper.GetScript(dfuScript));

            btnHidden.Attributes.Add("style", "display:none;");
            ScriptHelper.RegisterStartupScript(this, typeof(string), "DFU_Display_" + ucFileUpload.ClientID, "document.getElementById('uploaderDiv').style.display = 'block';", true);
        }
    }


    protected void btnHidden_Click(object sender, EventArgs e)
    {
        if (!StopProcessing)
        {
            try
            {
                switch (SourceType)
                {
                    case MediaSourceEnum.MediaLibraries:
                        HandleLibrariesUpload();
                        break;

                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                EventLogProvider.LogException("DIRECTFILEUPLOADER", "UPLOADFILE", ex);
                OnError(e);
            }
        }
    }

    #endregion


    #region "Media libraries"

    /// <summary>
    /// Provides operations necessary to create and store new library file.
    /// </summary>
    private void HandleLibrariesUpload()
    {
        // Get related library info        
        if (LibraryInfo != null)
        {
            MediaFileInfo mediaFile = null;

            // Get the site name
            SiteInfo si = SiteInfoProvider.GetSiteInfo(LibraryInfo.LibrarySiteID);
            string siteName = (si != null) ? si.SiteName : CMSContext.CurrentSiteName;

            string message = string.Empty;
            try
            {
                // Check the allowed extensions
                CheckAllowedExtensions();

                if (MediaFileID > 0)
                {
                    #region "Check permissions"

                    if (!MediaLibraryInfoProvider.IsUserAuthorizedPerLibrary(LibraryInfo, "FileModify"))
                    {
                        throw new Exception(GetString("media.security.nofilemodify"));
                    }

                    #endregion


                    mediaFile = MediaFileInfoProvider.GetMediaFileInfo(MediaFileID);
                    if (mediaFile != null)
                    {
                        // Ensure object version
                        SynchronizationHelper.EnsureObjectVersion(mediaFile);

                        if (IsMediaThumbnail)
                        {
                            string newFileExt = Path.GetExtension(ucFileUpload.FileName).TrimStart('.');
                            if ((ImageHelper.IsImage(newFileExt)) && (newFileExt.ToLowerCSafe() != "ico") &&
                                (newFileExt.ToLowerCSafe() != "wmf"))
                            {
                                // Update or creation of Media File update
                                string previewSuffix = MediaLibraryHelper.GetMediaFilePreviewSuffix(siteName);

                                if (!String.IsNullOrEmpty(previewSuffix))
                                {
                                    string previewExtension = Path.GetExtension(ucFileUpload.PostedFile.FileName);
                                    string previewName = Path.GetFileNameWithoutExtension(MediaLibraryHelper.GetPreviewFileName(mediaFile.FileName, mediaFile.FileExtension, previewExtension, siteName, previewSuffix));
                                    string previewFolder = DirectoryHelper.CombinePath(MediaLibraryHelper.EnsurePath(LibraryFolderPath.TrimEnd('/')), MediaLibraryHelper.GetMediaFileHiddenFolder(siteName));

                                    byte[] previewFileBinary = new byte[ucFileUpload.PostedFile.ContentLength];
                                    ucFileUpload.PostedFile.InputStream.Read(previewFileBinary, 0, ucFileUpload.PostedFile.ContentLength);

                                    // Delete current preview thumbnails
                                    MediaFileInfoProvider.DeleteMediaFilePreview(siteName, mediaFile.FileLibraryID, mediaFile.FilePath, false);

                                    // Save preview file
                                    MediaFileInfoProvider.SaveFileToDisk(siteName, LibraryInfo.LibraryFolder, previewFolder, previewName, previewExtension, mediaFile.FileGUID, previewFileBinary, false, false);

                                    // Log synchronization task
                                    SynchronizationHelper.LogObjectChange(mediaFile, TaskTypeEnum.UpdateObject);
                                }

                                // Drop the cache dependencies
                                CacheHelper.TouchKeys(MediaFileInfoProvider.GetDependencyCacheKeys(mediaFile, true));
                            }
                            else
                            {
                                message = GetString("media.file.onlyimgthumb");
                            }
                        }
                        else
                        {
                            // Get folder path
                            string path = Path.GetDirectoryName(DirectoryHelper.CombinePath(MediaLibraryInfoProvider.GetMediaLibraryFolderPath(LibraryInfo.LibraryID), mediaFile.FilePath));

                            // If file system permissions are sufficient for file update
                            if (DirectoryHelper.CheckPermissions(path, false, true, true, true))
                            {
                                // Delete existing media file
                                MediaFileInfoProvider.DeleteMediaFile(LibraryInfo.LibrarySiteID, LibraryInfo.LibraryID, mediaFile.FilePath, true, false);

                                // Update media file preview
                                if (MediaLibraryHelper.HasPreview(siteName, LibraryInfo.LibraryID, mediaFile.FilePath))
                                {
                                    // Get new unique file name
                                    string newName = URLHelper.GetSafeFileName(ucFileUpload.PostedFile.FileName, siteName);

                                    // Get new file path
                                    string newPath = DirectoryHelper.CombinePath(path, newName);
                                    newPath = MediaLibraryHelper.EnsureUniqueFileName(newPath);
                                    newName = Path.GetFileName(newPath);

                                    // Rename preview
                                    MediaLibraryHelper.MoveMediaFilePreview(mediaFile, newName);

                                    // Delete preview thumbnails
                                    MediaFileInfoProvider.DeleteMediaFilePreviewThumbnails(mediaFile);
                                }

                                // Receive media info on newly posted file
                                mediaFile = GetUpdatedFile(mediaFile.Generalized.DataClass);

                                // Save media file information
                                MediaFileInfoProvider.SetMediaFileInfo(mediaFile);
                            }
                            else
                            {
                                // Set error message
                                message = String.Format(GetString("media.accessdeniedtopath"), path);
                            }
                        }
                    }
                }
                else
                {
                    #region "Check permissions"

                    // Creation of new media file
                    if (!MediaLibraryInfoProvider.IsUserAuthorizedPerLibrary(LibraryInfo, "FileCreate"))
                    {
                        throw new Exception(GetString("media.security.nofilecreate"));
                    }

                    #endregion


                    // No file for upload specified
                    if (!ucFileUpload.HasFile)
                    {
                        throw new Exception(GetString("media.newfile.errorempty"));
                    }

                    // Create new media file record
                    mediaFile = new MediaFileInfo(ucFileUpload.PostedFile, LibraryID, LibraryFolderPath, ResizeToWidth, ResizeToHeight, ResizeToMaxSideSize, LibraryInfo.LibrarySiteID);

                    mediaFile.FileDescription = "";

                    // Save the new file info
                    MediaFileInfoProvider.SetMediaFileInfo(mediaFile);
                }
            }
            catch (Exception ex)
            {
                // Creation of new media file failed
                message = ex.Message;
            }
            finally
            {
                // Create media file info string
                string mediaInfo = "";
                if ((mediaFile != null) && (mediaFile.FileID > 0) && (IncludeNewItemInfo))
                {
                    mediaInfo = mediaFile.FileID + "|" + LibraryFolderPath.Replace('\\', '>').Replace("'", "\\'");
                }

                // Ensure message text
                message = HTMLHelper.EnsureLineEnding(message, " ");

                if (RaiseOnClick)
                {
                    ScriptHelper.RegisterStartupScript(Page, typeof(Page), "UploaderOnClick", ScriptHelper.GetScript("if (parent.UploaderOnClick) { parent.UploaderOnClick('" + MediaFileName.Replace(" ", "").Replace(".", "").Replace("-", "") + "'); }"));
                }

                string script = String.Format(@"
if (typeof(parent.DFU) !== 'undefined') {{ 
    parent.DFU.OnUploadCompleted('{0}'); 
}} 
if ((window.parent != null) && (/parentelemid={1}/i.test(window.location.href)) && (window.parent.InitRefresh_{1} != null)){{
    window.parent.InitRefresh_{1}({2}, false, '{3}'{4});
}}", QueryHelper.GetString("containerid", ""), ParentElemID, ScriptHelper.GetString(message.Trim()), mediaInfo, (InsertMode ? ", 'insert'" : ", 'update'"));

                // Call function to refresh parent window                                                     
                ScriptHelper.RegisterStartupScript(Page, typeof(Page), "RefreshParrent", ScriptHelper.GetScript(script));
            }
        }
    }


    /// <summary>
    /// Gets media file info object representing the updated version of original file.
    /// </summary>
    /// <param name="originalFile">Original file data</param>
    private MediaFileInfo GetUpdatedFile(IDataClass originalFile)
    {
        // Get info on media file from uploaded file
        MediaFileInfo mediaFile = new MediaFileInfo(ucFileUpload.PostedFile, LibraryID, LibraryFolderPath, ResizeToWidth, ResizeToHeight, ResizeToMaxSideSize, LibraryInfo.LibrarySiteID);

        // Create new file based on original
        MediaFileInfo updatedMediaFile = new MediaFileInfo(originalFile)
                                             {
                                                 // Update necessary information
                                                 FileName = mediaFile.FileName,
                                                 FileExtension = mediaFile.FileExtension,
                                                 FileSize = mediaFile.FileSize,
                                                 FileMimeType = mediaFile.FileMimeType,
                                                 FilePath = mediaFile.FilePath,
                                                 FileModifiedByUserID = mediaFile.FileModifiedByUserID,
                                                 FileBinary = mediaFile.FileBinary,
                                                 FileImageHeight = mediaFile.FileImageHeight,
                                                 FileImageWidth = mediaFile.FileImageWidth,
                                                 FileBinaryStream = mediaFile.FileBinaryStream
                                             };

        return updatedMediaFile;
    }

    #endregion
}