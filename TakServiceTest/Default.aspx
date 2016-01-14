<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="TakServiceTest._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <table><tr><td>
    Paste PTN here:<br />
    <asp:TextBox ID="ptn" runat="server" Rows="30" TextMode="MultiLine"></asp:TextBox><br />
    <asp:Button ID="go" runat="server" Text="Go!" OnClick="go_Click" /></td><td></td>
    The next move is:
    <asp:TextBox ID="move" runat="server"></asp:TextBox></td>
    </tr></table>
</asp:Content>
