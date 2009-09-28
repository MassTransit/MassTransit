<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="CodeCamp.Web._Default" MasterPageFile="~/Masters/TulsaTechFest.Master" %>

<asp:Content ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <fieldset>
        <legend>Sign In</legend>
        
        <p>
            <label>Username:</label>
            <asp:TextBox runat="server" ID="username" CssClass="text"></asp:TextBox>
        </p>
        
        <p>
            <label>Password:</label>
            <asp:TextBox runat="server" ID="password" CssClass="text" TextMode="Password"></asp:TextBox>
        </p>
        
        <p>
            Not a member? <a href="/registration/">Register Now</a>
        </p>
        <asp:Button ID="submitButton" runat="server" OnClick="submitButton_Click" Text="Logon" />
        <br />
        <br />
        <br />    
    </fieldset>
    <asp:Label ID="timerLabel" runat="server" Text="Elapsed Time:"></asp:Label>
</asp:Content>