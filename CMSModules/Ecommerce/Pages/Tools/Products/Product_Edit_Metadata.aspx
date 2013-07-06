<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Product_Edit_Metadata.aspx.cs"
    Inherits="CMSModules_Ecommerce_Pages_Tools_Products_Product_Edit_Metadata" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Theme="Default" %>

<%@ Register Src="~/CMSModules/Content/Controls/editmenu.ascx" TagName="editmenu"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Content/Controls/MetaData.ascx" TagName="MetaData"
    TagPrefix="cms" %>
<asp:Content ContentPlaceHolderID="plcBeforeContent" runat="server">
    <cms:editmenu ID="menuElem" runat="server" ShowApprove="true" ShowReject="true" ShowSubmitToApproval="true"
        ShowProperties="false" IsLiveSite="false" />
</asp:Content>
<asp:Content runat="server" ID="content" ContentPlaceHolderID="plcContent">
    <div class="PropertiesPanel">
        <cms:MetaData ID="metaDataElem" runat="server" UIModuleName="CMS.Ecommerce" UIPageElementName="Products.MetaData.Page"
            UITagsElementName="Products.MetaData.Tags" />
    </div>
</asp:Content>
