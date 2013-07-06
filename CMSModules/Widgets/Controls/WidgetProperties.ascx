<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Widgets_Controls_WidgetProperties"
    CodeFile="WidgetProperties.ascx.cs" %>
<asp:PlaceHolder ID="plcToolbar" runat="server" Visible="false" EnableViewState="false">
    <div id="CMSHeaderDiv"">
        <div id="CKToolbar" class="CKToolbar">
        </div>
    </div>

    <script type="text/javascript">
        //<![CDATA[

        var cmsHeader = null, cmsHeaderPad = null, cmsFooter = null, cmsFooterPad = null, disableQim = true;
        var resizeInterval = setInterval('if (window.ResizeToolbar) { ResizeToolbar(); }', 300);

        //]]>
    </script>

</asp:PlaceHolder>
<asp:Panel runat="server" ID="pnlTab" CssClass="TabsPageContent">
    <asp:Panel runat="server" ID="pnlFormArea" CssClass="WebPartForm">
        <asp:Label ID="lblInfo" runat="server" CssClass="InfoLabel" Visible="false" EnableViewState="false" />
        <cms:LocalizedLabel runat="server" ID="lblError" Visible="false" CssClass="ErrorLabel" />
        <cms:BasicForm runat="server" ID="formCustom" HtmlAreaToolbarLocation="Out:CKToolbar"
            Enabled="true" DefaultFormLayout="Tables" DefaultCategoryName="Default" MarkRequiredFields="true" />
    </asp:Panel>
</asp:Panel>
<asp:HiddenField runat="server" ID="hidRefresh" Value="0" />
<asp:Literal ID="ltlScript" runat="server" EnableViewState="false" />
<cms:CMSButton ID="btnOnApply" runat="server" Visible="false" EnableViewState="false" />
<cms:CMSButton ID="btnOnOK" runat="server" Visible="false" EnableViewState="false" />
<asp:HiddenField ID="hdnIsNewWebPart" runat="server" />
<asp:HiddenField ID="hdnInstanceGUID" runat="server" />
<asp:HiddenField ID="hdnWidgetDefinition" runat="server" />

<script type="text/javascript">
    //<![CDATA[
    // cmsedit.js function override for CKEditor
    function SaveDocument() { }
    //]]>
</script>