﻿using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.PortalControls;
using CMS.PortalEngine;
using CMS.WebAnalytics;


public partial class CMSWebParts_WebAnalytics_CustomStatistics : CMSAbstractWebPart
{
    #region "Properties"

    /// <summary>
    /// If true, statistics is loged only once per user session
    /// </summary>
    public bool OncePerUser
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("OncePerUser"), false);
        }
        set
        {
            SetValue("OncePerUser", value);
        }
    }


    /// <summary>
    /// Name of statistics to log
    /// </summary>
    public String StatisticsName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("StatisticsName"), String.Empty);
        }
        set
        {
            SetValue("StatisticsName", value);
        }
    }


    /// <summary>
    /// Object name (holds additional info about log) 
    /// </summary>
    public String StatisticsObjectName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("StatisticsObjectName"), String.Empty);
        }
        set
        {
            SetValue("StatisticsObjectName", value);
        }
    }


    /// <summary>
    /// Statistics value to log
    /// </summary>
    public double StatisticsValue
    {
        get
        {
            return ValidationHelper.GetDoubleSystem(GetValue("StatisticsValue"), 0);
        }
        set
        {
            SetValue("StatisticsValue", value);
        }
    }

    #endregion


    #region "Methods"

    /// </summary>
    /// Content loaded event handler
    /// </summary>
    public override void OnContentLoaded()
    {
        base.OnContentLoaded();
        SetupControl();
    }


    /// <summary>
    /// Initializes the control properties.
    /// </summary>
    protected void SetupControl()
    {
        if (!StopProcessing && AnalyticsHelper.IsLoggingEnabled(CMSContext.CurrentSiteName, CMSContext.CurrentAliasPath))
        {
            // Log only for non empty statistics name
            if (!String.IsNullOrEmpty(StatisticsName))
            {
                // Log only for live site
                if (CMSContext.ViewMode == ViewModeEnum.LiveSite)
                {
                    // If once per user is set - log only once per session
                    if (OncePerUser)
                    {
                        // If already in the session - do not log
                        if (SessionHelper.GetValue("CustomStatisticsLogged_" + StatisticsName) != null)
                        {
                            return;
                        }

                        SessionHelper.SetValue("CustomStatisticsLogged_" + StatisticsName, true);
                    }

                    HitLogProvider.LogHit(StatisticsName, CMSContext.CurrentSiteName, CMSContext.CurrentPageInfo.DocumentCulture, StatisticsObjectName, CMSContext.CurrentPageInfo.NodeID, 1, StatisticsValue);
                }
            }
        }
    }

    #endregion
}