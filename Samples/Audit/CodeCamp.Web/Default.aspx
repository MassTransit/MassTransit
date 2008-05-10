<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="CodeCamp.Web._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Tulsa School of Dev Sample</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Label ID="usernameLabel" runat="server">Username:</asp:Label>
        <asp:TextBox runat="server" ID="username"></asp:TextBox>
        <br />
        <asp:Label ID="passwordLabel" runat="server">Password:</asp:Label>
        <asp:TextBox runat="server" ID="password"></asp:TextBox>
        <br />
        <br />
        <asp:Label ID="resultLabel" runat="server" Text="Result"></asp:Label><br />
        <br />
        <asp:Button ID="submitButton" runat="server" OnClick="submitButton_Click" Text="Logon" />
        <br />
        <br />
        <br />
        <asp:Label ID="timerLabel" runat="server" Text="Elapsed Time:"></asp:Label></div>
    </form>
</body>
</html>
