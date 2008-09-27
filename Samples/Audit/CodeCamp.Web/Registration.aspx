<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Registration.aspx.cs" Inherits="CodeCamp.Web.Registration" MasterPageFile="~/Masters/TulsaTechFest.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <fieldset>
        <legend>Register</legend>
        <p><label>Name</label><asp:TextBox runat="server" ID="inputName" CssClass="text" TextMode="SingleLine"></asp:TextBox></p>
        <p><label>Username</label><asp:TextBox runat="server" ID="inputUsername" CssClass="text" TextMode="SingleLine"></asp:TextBox></p>
        <p><label>Password</label><asp:TextBox runat="server" ID="inputPassword" CssClass="text" TextMode="Password"></asp:TextBox></p>
    </fieldset>

</asp:Content>