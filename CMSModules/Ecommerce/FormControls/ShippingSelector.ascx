<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_FormControls_ShippingSelector"
    CodeFile="ShippingSelector.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector"
    TagPrefix="cms" %>
<cms:UniSelector ID="uniSelector" runat="server" DisplayNameFormat="{%ShippingOptionDisplayName%}" EnabledColumnName="ShippingOptionEnabled"
    ReturnColumnName="ShippingOptionID" ObjectType="ecommerce.shippingoption" ResourcePrefix="shippingselector"
    SelectionMode="SingleDropDownList" AllowEmpty="false" />
