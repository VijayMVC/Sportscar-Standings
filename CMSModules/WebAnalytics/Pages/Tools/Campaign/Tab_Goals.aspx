<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Tab_Goals.aspx.cs" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Theme="Default" Inherits="CMSModules_WebAnalytics_Pages_Tools_Campaign_Tab_Goals" %>

<%@ Register Src="~/CMSFormControls/Basic/TextBoxControl.ascx" TagName="TextBoxControl"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Basic/CheckBoxControl.ascx" TagName="CheckBoxControl"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Basic/RadioButtonsControl.ascx" TagName="RadioButtonsControl"
    TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:UIForm runat="server" ID="EditForm" ObjectType="analytics.campaign" DefaultFieldLayout="Inline">
        <SecurityCheck Resource="CMS.WebAnalytics" Permission="ManageCampaigns" />
        <LayoutTemplate>
            <table>
                <tr>
                    <td>
                    </td>
                    <td class="FieldLabel CampaignRedFlag">
                        <cms:LocalizedLabel ID="lblRedFalg" runat="server" EnableViewState="false" ResourceString="campaign.redflag" />
                    </td>
                    <td class="FieldLabel CampaignGoal">
                        <cms:LocalizedLabel ID="lblFinalGoal" runat="server" EnableViewState="false" ResourceString="campaign.finalgoal" />
                    </td>
                    <td>
                    </td>
                </tr>
                <tr>
                    <td class="FieldLabel">
                        <cms:FormLabel ID="lblVisitorsWin" runat="server" EnableViewState="false" ResourceString="campaign.numofvisitors"
                            DisplayColon="true" Field="CampaignGoalVisitorsMin" />
                        <cms:FormLabel ID="lblVisitors" runat="server" EnableViewState="false" ResourceString="campaign.numofvisitors"
                            DisplayColon="true" Field="CampaignGoalVisitors" Visible="false" />
                    </td>
                    <td>
                        <cms:FormField runat="server" ID="fVisitorsMin" Field="CampaignGoalVisitorsMin">
                            <cms:TextBoxControl ID="txtVisitorsMin" runat="server" CssClass="SmallTextBox" MaxLength="50" />
                        </cms:FormField>
                    </td>
                    <td>
                        <cms:FormField runat="server" ID="fVistors" Field="CampaignGoalVisitors">
                            <cms:TextBoxControl ID="txtVisitors" runat="server" CssClass="SmallTextBox" MaxLength="50" />
                        </cms:FormField>
                    </td>
                    <td>
                        <cms:FormField runat="server" ID="fVistorsPercent" Field="CampaignGoalVisitorsPercent">
                            <cms:RadioButtonsControl ID="rbVisitorsPercent" runat="server" />
                        </cms:FormField>
                    </td>
                </tr>
                <tr>
                    <td class="FieldLabel">
                        <cms:FormLabel ID="lblConversionsWin" runat="server" EnableViewState="false" ResourceString="campaign.conversions"
                            DisplayColon="true" Field="CampaignGoalConversionsMin" />
                        <cms:FormLabel ID="lblConversions" runat="server" EnableViewState="false" ResourceString="campaign.conversions"
                            DisplayColon="true" Field="CampaignGoalConversions" Visible="false" />
                    </td>
                    <td>
                        <cms:FormField runat="server" ID="fConversionsMin" Field="CampaignGoalConversionsMin">
                            <cms:TextBoxControl ID="txtConversionsMin" runat="server" CssClass="SmallTextBox"
                                MaxLength="50" />
                        </cms:FormField>
                    </td>
                    <td>
                        <cms:FormField runat="server" ID="fConversions" Field="CampaignGoalConversions">
                            <cms:TextBoxControl ID="txtConversions" runat="server" CssClass="SmallTextBox" MaxLength="50" />
                        </cms:FormField>
                    </td>
                    <td>
                        <cms:FormField runat="server" ID="fConversionsPercent" Field="CampaignGoalConversionsPercent">
                            <cms:RadioButtonsControl ID="rbConversionsPercent" runat="server" />
                        </cms:FormField>
                    </td>
                </tr>
                <tr>
                    <td class="FieldLabel">
                        <cms:FormLabel ID="lblValueMin" runat="server" EnableViewState="false" ResourceString="campaign.value"
                            DisplayColon="true" Field="CampaignGoalValueMin" />
                        <cms:FormLabel ID="lblValue" runat="server" EnableViewState="false" ResourceString="campaign.value"
                            DisplayColon="true" Field="CampaignGoalValue" Visible="false" />
                    </td>
                    <td>
                        <cms:FormField runat="server" ID="fValueMin" Field="CampaignGoalValueMin">
                            <cms:TextBoxControl ID="txtValueMin" runat="server" CssClass="SmallTextBox" MaxLength="50" />
                        </cms:FormField>
                    </td>
                    <td>
                        <cms:FormField runat="server" ID="fValue" Field="CampaignGoalValue">
                            <cms:TextBoxControl ID="txtValue" runat="server" CssClass="SmallTextBox" MaxLength="50" />
                        </cms:FormField>
                    </td>
                    <td>
                        <cms:FormField runat="server" ID="fValuePercent" Field="CampaignGoalValuePercent">
                            <cms:RadioButtonsControl ID="rbValuePercent" runat="server" />
                        </cms:FormField>
                    </td>
                </tr>
                <tr>
                    <td class="FieldLabel">
                        <cms:FormLabel ID="lblPerVisitorMin" runat="server" EnableViewState="false" ResourceString="campaign.pervisitor"
                            DisplayColon="true" Field="CampaignGoalPerVisitorMin" />
                        <cms:FormLabel ID="lblPerVisitor" runat="server" EnableViewState="false" ResourceString="campaign.pervisitor"
                            DisplayColon="true" Field="CampaignGoalPerVisitor" Visible="false" />
                    </td>
                    <td>
                        <cms:FormField runat="server" ID="fPerVisitorMin" Field="CampaignGoalPerVisitorMin">
                            <cms:TextBoxControl ID="txtVPerVisitorMin" runat="server" CssClass="SmallTextBox"
                                MaxLength="50" />
                        </cms:FormField>
                    </td>
                    <td>
                        <cms:FormField runat="server" ID="fPerVisitor" Field="CampaignGoalPerVisitor">
                            <cms:TextBoxControl ID="txtPerVisitor" runat="server" CssClass="SmallTextBox" MaxLength="50" />
                        </cms:FormField>
                    </td>
                    <td>
                        <cms:FormField runat="server" ID="fPerVisitorPercent" Field="CampaignGoalPerVisitorPercent">
                            <cms:RadioButtonsControl ID="rbPerVisitorPercent" runat="server" />
                        </cms:FormField>
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td>
                        <cms:FormSubmitButton ID="btnOK" runat="server" OnClick="btnOk_Click" />
                    </td>
                </tr>
            </table>
        </LayoutTemplate>
    </cms:UIForm>
</asp:Content>
