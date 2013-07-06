<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Title="Alternative forms - General properties" Inherits="CMSModules_BizForms_Tools_AlternativeForms_AlternativeForms_Edit_General"
    Theme="Default" CodeFile="AlternativeForms_Edit_General.aspx.cs" %>

<%@ Register Src="~/CMSModules/AdminControls/Controls/Class/AlternativeFormEdit.ascx"
    TagName="AlternativeFormEdit" TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:AlternativeFormEdit runat="server" ID="altFormEdit" />
</asp:Content>
