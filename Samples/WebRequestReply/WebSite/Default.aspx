<%@ Page Language="C#" AutoEventWireup="true"  CodeFile="Default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Sample Request/Reply Site</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
		<asp:Label ID="Label1" runat="server" Text="Request:"></asp:Label>
		<asp:TextBox ID="requestText" runat="server" Width="361px"></asp:TextBox><br />
		<br />
		Response:
		<asp:TextBox ID="responseBox" runat="server" Width="350px"></asp:TextBox><br />
		<br />
		<asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="Sync Request" />
		<br />
		<asp:Button ID="Button2" runat="server" OnClick="Button2_Click" Text="Async Request" /></div>
    </form>
</body>
</html>
