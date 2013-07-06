using System;

using CMS.OnlineMarketing;
using CMS.WebAnalytics;

public partial class CMSModules_ContactManagement_Controls_UI_ActivityDetails_Rating : ActivityDetail
{
    #region "Methods"

    public override bool LoadData(ActivityInfo ai)
    {
        if ((ai == null) || (ai.ActivityType != PredefinedActivityType.RATING))
        {
            return false;
        }

        // Load data to control
        int nodeId = ai.ActivityNodeID;
        ucDetails.AddRow("om.activitydetails.documenturl", GetLinkForDocument(nodeId, ai.ActivityCulture), false);
        ucDetails.AddRow("om.activitydetails.ratingvalue", String.Format("{0:00.0}", ai.ActivityValue));

        return ucDetails.IsDataLoaded;
    }

    #endregion
}