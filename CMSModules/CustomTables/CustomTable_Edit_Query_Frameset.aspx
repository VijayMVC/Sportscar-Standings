<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CustomTable_Edit_Query_Frameset.aspx.cs"
    Inherits="CMSModules_CustomTables_CustomTable_Edit_Query_Frameset" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Frameset//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-frameset.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server" enableviewstate="false">
    <title>Development - Custom tables - Edit query</title>
</head>
<frameset border="0" rows="<%=TabsBreadFrameHeight%>, *" id="rowsFrameset">
    <frame name="ctq_edit_menu" src="CustomTable_Edit_Query_Header.aspx?<%=QueryHelper.EncodedQueryString%>"
        scrolling="no" frameborder="0" noresize="noresize" />
    <frame name="ctq_edit_content" src="CustomTable_Edit_Query_Edit.aspx?<%=QueryHelper.EncodedQueryString%>"
        frameborder="0" />
    <cms:NoFramesLiteral ID="ltlNoFrames" runat="server" />
</frameset>
</html>
