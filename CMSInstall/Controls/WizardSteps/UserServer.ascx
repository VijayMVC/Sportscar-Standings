<%@ Control Language="C#" AutoEventWireup="true" CodeFile="UserServer.ascx.cs" Inherits="CMSInstall_Controls_WizardSteps_UserServer" %>
<div class="InstallContent">
    <table class="InstallWizard" border="0" cellpadding="0" cellspacing="0">
        <tr>
            <td colspan="2">
                <asp:Label ID="lblSQLServer" runat="server" CssClass="InstallGroupTitle UserServer" />
            </td>
        </tr>
        <tr>
            <td nowrap="nowrap" align="right" style="padding-right: 8px">
                <asp:Label ID="lblServerName" AssociatedControlID="txtServerName" runat="server" />
            </td>
            <td width="100%">
                <cms:CMSTextBox ID="txtServerName" CssClass="InstallFormTextBox" runat="server" />
            </td>
        </tr>
        <asp:PlaceHolder ID="plcRadSQL" runat="server">
            <tr>
                <td colspan="2" align="left">
                    <asp:RadioButton ID="radSQLAuthentication" runat="server" AutoPostBack="True" GroupName="AuthenticationType"
                        Checked="True"></asp:RadioButton>
                </td>
            </tr>
        </asp:PlaceHolder>
        <tr class="InstallSQLName">
            <td nowrap="nowrap" align="right" style="padding-right: 8px">
                <asp:Label ID="lblDBUsername" AssociatedControlID="txtDBUsername" runat="server" />
            </td>
            <td>
                <cms:CMSTextBox ID="txtDBUsername" CssClass="InstallFormTextBox" runat="server" />
            </td>
        </tr>
        <tr class="InstallSQLPwd">
            <td nowrap="nowrap" align="right" style="padding-right: 8px">
                <asp:Label ID="lblDBPassword" AssociatedControlID="txtDBPassword" runat="server" />
            </td>
            <td>
                <cms:CMSTextBox ID="txtDBPassword" CssClass="InstallFormTextBox" runat="server" TextMode="Password" />
            </td>
        </tr>
        <asp:PlaceHolder ID="plcWinAuth" runat="server">
            <tr>
                <td colspan="2" align="left">
                    <asp:RadioButton ID="radWindowsAuthentication" runat="server" AutoPostBack="True"
                        GroupName="AuthenticationType"></asp:RadioButton>
                </td>
            </tr>
        </asp:PlaceHolder>
        <tr>
            <td colspan="2">
                &nbsp;
            </td>
        </tr>
        <asp:PlaceHolder ID="plcSeparationError" runat="server" EnableViewState="False" Visible="False">
            <tr>
                <td colspan="2" align="left">
                    <asp:Label ID="lblError" runat="server" CssClass="ErrorLabel" />
                </td>
            </tr>
        </asp:PlaceHolder>
    </table>
</div>
