<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Frameset.aspx.cs" Inherits="CMSModules_MyDesk_WaitingForApproval_Frameset" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/TabsFrameset.ascx" TagName="TabsFrameset" TagPrefix="cms" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Frameset//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-frameset.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server" enableviewstate="false">
    <title>Pending objects</title>
</head>
<cms:TabsFrameset ID="frm" runat="server" ContentUrl="~/CMSPages/blank.aspx" />
</html>
