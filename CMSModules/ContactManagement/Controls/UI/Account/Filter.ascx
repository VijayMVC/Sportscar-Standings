<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Filter.ascx.cs" Inherits="CMSModules_ContactManagement_Controls_UI_Account_Filter" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/Filters/TextSimpleFilter.ascx" TagName="TextSimpleFilter"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/ContactManagement/FormControls/AccountStatusSelector.ascx"
    TagName="AccountStatusSelector" TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Sites/SiteSelector.ascx" TagName="SiteSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Sites/SiteOrGlobalSelector.ascx" TagName="SiteOrGlobalSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Filters/DateTimeFilter.ascx" TagName="DateTimeFilter"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/Filters/TimeSimpleFilter.ascx" TagName="TimeSimpleFilter"
    TagPrefix="cms" %>
<asp:Panel ID="pnl" runat="server" DefaultButton="btnSearch">
    <table cellpadding="0" cellspacing="2">
        <tr>
            <td>
                <cms:LocalizedLabel ID="lblName" runat="server" ResourceString="om.account.name"
                    DisplayColon="true" EnableViewState="false" />
            </td>
            <td>
                <cms:TextSimpleFilter ID="fltName" runat="server" Column="AccountName" />
            </td>
        </tr>
        <tr>
            <td>
                <cms:LocalizedLabel ID="lblAccountStatus" runat="server" ResourceString="om.accountstatus"
                    DisplayColon="true" EnableViewState="false" />
            </td>
            <td>
                <cms:AccountStatusSelector ID="fltAccountStatus" runat="server" IsLiveSite="false" />
            </td>
        </tr>
        <tr>
            <td>
                <cms:LocalizedLabel ID="lblEmail" runat="server" ResourceString="general.emailaddress"
                    DisplayColon="true" EnableViewState="false" />
            </td>
            <td>
                <cms:TextSimpleFilter ID="fltEmail" runat="server" Column="AccountEmail" />
            </td>
        </tr>
        <tr>
            <td>
                <cms:LocalizedLabel ID="lblContactName" runat="server" ResourceString="om.contact.name"
                    DisplayColon="true" EnableViewState="false" />
            </td>
            <td>
                <cms:TextSimpleFilter ID="fltContactName" runat="server" />
            </td>
        </tr>
        <asp:PlaceHolder ID="plcSite" runat="server" Visible="false">
            <tr>
                <td>
                    <cms:LocalizedLabel ID="lblSite" runat="server" ResourceString="general.site" DisplayColon="true"
                        EnableViewState="false" />
                </td>
                <td>
                    <cms:SiteSelector ID="siteSelector" runat="server" IsLiveSite="false" AllowGlobal="true"
                        Visible="false" DropDownCssClass="DropDownFieldFilter" />
                    <cms:SiteOrGlobalSelector ID="siteOrGlobalSelector" runat="server" IsLiveSite="false"
                        Visible="false" DropDownCSSClass="DropDownFieldFilter" AutoPostBack="false" />
                </td>
            </tr>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plcAdvancedSearch" runat="server" Visible="false">
            <tr>
                <td>
                    <cms:LocalizedLabel ID="lblPhone" runat="server" ResourceString="general.phone" DisplayColon="true"
                        EnableViewState="false" />
                </td>
                <td>
                    <cms:TextSimpleFilter ID="fltPhone" runat="server" />
                </td>
            </tr>
            <tr>
                <td>
                    <cms:LocalizedLabel ID="lblOwner" runat="server" ResourceString="om.account.owner"
                        DisplayColon="true" EnableViewState="false" />
                </td>
                <td>
                    <cms:TextSimpleFilter ID="fltOwner" runat="server" Column="FullName" />
                </td>
            </tr>
            <tr>
                <td>
                    <cms:LocalizedLabel ID="lblCountry" runat="server" ResourceString="general.country"
                        DisplayColon="true" EnableViewState="false" />
                </td>
                <td>
                    <cms:TextSimpleFilter ID="fltCountry" runat="server" Column="CountryDisplayName" />
                </td>
            </tr>
            <tr>
                <td>
                    <cms:LocalizedLabel ID="lblState" runat="server" ResourceString="general.state" DisplayColon="true"
                        EnableViewState="false" />
                </td>
                <td>
                    <cms:TextSimpleFilter ID="fltState" runat="server" Column="StateDisplayName" />
                </td>
            </tr>
            <tr>
                <td>
                    <cms:LocalizedLabel ID="lblCity" runat="server" ResourceString="general.city" DisplayColon="true"
                        EnableViewState="false" />
                </td>
                <td>
                    <cms:TextSimpleFilter ID="fltCity" runat="server" Column="AccountCity" />
                </td>
            </tr>
            <tr>
                <td>
                    <cms:LocalizedLabel ID="lblCreated" runat="server" ResourceString="filter.createdbetween"
                        DisplayColon="true" EnableViewState="false" />
                </td>
                <td>
                    <cms:TimeSimpleFilter ID="fltCreated" runat="server" Column="AccountCreated" />
                </td>
            </tr>
            <asp:PlaceHolder ID="plcMerged" runat="server">
                <tr>
                    <td>
                        <cms:LocalizedLabel ID="lblMerged" runat="server" ResourceString="om.account.merged"
                            DisplayColon="true" EnableViewState="false" />
                    </td>
                    <td>
                        <cms:LocalizedCheckBox ID="chkMerged" runat="server" ResourceString="om.account.alsomerged" />
                    </td>
                </tr>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="plcChildren" runat="server" Visible="false">
                <tr>
                    <td>
                        <cms:LocalizedLabel ID="lblChildren" runat="server" ResourceString="om.account.listchildren"
                            DisplayColon="true" EnableViewState="false" />
                    </td>
                    <td>
                        <cms:LocalizedCheckBox ID="chkChildren" runat="server" ResourceString="om.account.listchildrencheck" />
                    </td>
                </tr>
            </asp:PlaceHolder>
        </asp:PlaceHolder>
        <tr>
            <td>
            </td>
            <td>
                <cms:LocalizedButton ID="btnSearch" runat="server" CssClass="ContentButton" ResourceString="general.search" />
                <asp:LinkButton ID="btnReset" runat="server" Style="line-height: 2em; margin: 1em"
                    EnableViewState="false" />
            </td>
        </tr>
    </table>
    <br />
</asp:Panel>
<asp:Panel ID="pnlAdvanced" runat="server" Visible="true">
    <asp:Image runat="server" ID="imgShowSimpleFilter" CssClass="NewItemImage" />
    <asp:LinkButton ID="lnkShowSimpleFilter" runat="server" OnClick="lnkShowSimpleFilter_Click" />
</asp:Panel>
<asp:Panel ID="pnlSimple" runat="server" Visible="false">
    <asp:Image runat="server" ID="imgShowAdvancedFilter" CssClass="NewItemImage" />
    <asp:LinkButton ID="lnkShowAdvancedFilter" runat="server" OnClick="lnkShowAdvancedFilter_Click" />
</asp:Panel>
<br />
