﻿using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.GlobalHelper;
using CMS.OnlineMarketing;
using CMS.SettingsProvider;
using CMS.UIControls;

[Security(Resource = ModuleEntry.CONTACTMANAGEMENT, Permission = "ReadActivities")]
public partial class CMSModules_ContactManagement_Pages_Tools_Activities_Frameset : CMSContactManagementPage
{
    protected int siteId = 0;
    protected string sitemanager = string.Empty;


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Get form ID from url
        siteId = QueryHelper.GetInteger("siteid", 0);
        // Check 'issitemanager' parameter

        if (ContactHelper.IsSiteManager)
        {
            sitemanager = "&issitemanager=1";
            rowsFrameset.Attributes["rows"] = TabsBreadFrameHeight + ", *";
        }
        else
        {
            rowsFrameset.Attributes["rows"] = TabsFrameHeight + ", *";
        }
    }
}