<%@ Control Language="C#" AutoEventWireup="true" CodeFile="AnalyticsReportViewer.ascx.cs"
    Inherits="CMSModules_WebAnalytics_Controls_AnalyticsReportViewer" %>
<%@ Register Src="~/CMSModules/WebAnalytics/Controls/SelectGraphTypeAndPeriod.ascx"
    TagName="GraphTypeAndPeriod" TagPrefix="cms" %>
<asp:Panel runat="server" ID="pnlPeriodSelectors" CssClass="PageHeaderLine">
    <cms:GraphTypeAndPeriod runat="server" ID="ucGraphTypePeriod" />
</asp:Panel>
<div class="ReportBody ReportBodyAnalytics" runat="server" id="divGraphArea">
    <cms:MessagesPlaceHolder ID="plcMess" runat="server" />        
</div>
