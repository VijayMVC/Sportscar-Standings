<%@ Page Title="" Language="C#" MasterPageFile="~/CMSAPIExamples/Pages/APIExamplesPage.Master"
    Theme="Default" AutoEventWireup="true" Inherits="CMSAPIExamples_Code_Administration_Badges_Default" CodeFile="Default.aspx.cs" %>

<%@ Register Src="~/CMSAPIExamples/Controls/APIExample.ascx" TagName="APIExample" TagPrefix="cms" %>
<%@ Register Assembly="CMS.UIControls" Namespace="CMS.UIControls" TagPrefix="cms" %>
<asp:Content ID="contentLeft" ContentPlaceHolderID="plcLeftContainer" runat="server">
    <%-- Badge --%>
    <cms:APIExamplePanel ID="pnlCreateBadge" runat="server" GroupingText="Badge">
        <cms:APIExample ID="apiCreateBadge" runat="server" ButtonText="Create badge" InfoMessage="Badge 'My new badge' was created." />
        <cms:APIExample ID="apiGetAndUpdateBadge" runat="server" ButtonText="Get and update badge" APIExampleType="ManageAdditional" InfoMessage="Badge 'My new badge' was updated." ErrorMessage="Badge 'My new badge' was not found." />
        <cms:APIExample ID="apiGetAndBulkUpdateBadges" runat="server" ButtonText="Get and bulk update badges" APIExampleType="ManageAdditional" InfoMessage="All badges matching the condition were updated." ErrorMessage="Badges matching the condition were not found." />
    </cms:APIExamplePanel>
    <cms:APIExamplePanel ID="pnlBadgeUser" runat="server" GroupingText="Badge on user">
        <cms:APIExample ID="apiAddBadgeToUser" runat="server" ButtonText="Add badge to user" APIExampleType="ManageAdditional" InfoMessage="Badge 'My new badge' was added to current user." ErrorMessage="Badge 'My new badge' was not found."/>
        <cms:APIExample ID="apiUpdateActivityPoints" runat="server" ButtonText="Update user's activity points" APIExampleType="ManageAdditional" InfoMessage="Activity points for current user were updated." ErrorMessage="Badge 'My new badge' was not found." />
    </cms:APIExamplePanel>
</asp:Content>
<asp:Content ID="contentRight" ContentPlaceHolderID="plcRightContainer" runat="server">
    <%-- Badge on user--%>
    <cms:APIExamplePanel ID="pnlRemoveBadgeFromUser" runat="server" GroupingText="Badge on user">
        <cms:APIExample ID="apiRemoveBadgeFromUser" runat="server" ButtonText="Remove badge from user" APIExampleType="CleanUpMain" InfoMessage="Badge 'My new badge' was removed from current user." ErrorMessage="Badge 'My new badge' was not found." />
    </cms:APIExamplePanel>
    <%-- Badge --%>
    <cms:APIExamplePanel ID="pnlDeleteBadge" runat="server" GroupingText="Badge">
        <cms:APIExample ID="apiDeleteBadge" runat="server" ButtonText="Delete badge" APIExampleType="CleanUpMain" InfoMessage="Badge 'My new badge' and all its dependencies were deleted." ErrorMessage="Badge 'My new badge' was not found." />
    </cms:APIExamplePanel>
</asp:Content>
