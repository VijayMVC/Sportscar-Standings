<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_PortalEngine_UI_PageTemplates_DeviceLayout_Frameset"
    CodeFile="DeviceLayout_Frameset.aspx.cs" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Frameset//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-frameset.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server" enableviewstate="false">
    <title>Development - Page templates - device layouts</title>
</head>
<frameset border="0" rows="<%=headerHeight%>, *" framespacing="0" id="rowsFrameset">
    <frame name="deviceLayoutHeader" src="<%=deviceLayoutHeaderUrl%>" frameborder="0" scrolling="no"
        noresize="noresize" />
    <frame name="deviceLayoutContent" src="<%=deviceLayoutContentUrl%>" frameborder="0" />
    <cms:NoFramesLiteral ID="ltlNoFrames" runat="server" /></frameset>
</html>
