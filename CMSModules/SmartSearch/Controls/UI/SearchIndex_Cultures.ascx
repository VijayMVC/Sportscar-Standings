<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_SmartSearch_Controls_UI_SearchIndex_Cultures"
    CodeFile="SearchIndex_Cultures.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Sites/SiteSelector.ascx" TagName="SiteSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/Basic/DisabledModuleInfo.ascx" TagPrefix="cms"
    TagName="DisabledModule" %>
<asp:Panel ID="pnlBody" runat="server">
    <cms:CMSUpdatePanel ID="pnlUpdate" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="pnlContent" runat="server" CssClass="PageContent">
                <cms:DisabledModule runat="server" ID="ucDisabledModule" />
                <cms:MessagesPlaceHolder ID="plcMess" runat="server" />
                <asp:Panel ID="pnlForm" runat="server">
                    <strong>
                        <cms:LocalizedLabel runat="server" ID="lblInfoLabel" CssClass="InfoLabel" EnableViewState="false"
                            ResourceString="srch.index.cultures" DisplayColon="true" /></strong>
                    <cms:UniSelector ID="uniSelector" runat="server" IsLiveSite="false" ObjectType="cms.culture"
                        OrderBy="CultureName" SelectionMode="Multiple" OnOnSelectionChanged="uniSelector_OnSelectionChanged" />
                </asp:Panel>
                <cms:CMSTextBox ID="hidItem" runat="server" Style="display: none;" />
                <cms:CMSTextBox ID="hidAllItems" runat="server" Style="display: none;" />
            </asp:Panel>
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Panel>
