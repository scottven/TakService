﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="TakService.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Tak AI Move Generator</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <table>
                <tr>
                    <td>Paste PTN here:<br />
                        <asp:TextBox ID="ptn" runat="server" Rows="30" TextMode="MultiLine"></asp:TextBox><br />
                        <asp:Button ID="go" runat="server" Text="Go!" OnClick="go_Click" />
                    </td>
                    <td valign="top">
                        <br />
                        AI level: <asp:TextBox ID="aiLevel" runat="server" MaxLength="1" Type="number">3</asp:TextBox><br />
                        The next move is: <asp:TextBox ID="move" runat="server"></asp:TextBox>
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
