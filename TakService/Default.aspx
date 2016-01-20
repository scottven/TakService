<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="TakService.Default" validateRequest="false" %>

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
                    <td>Paste code here:<br />
                        <asp:RadioButton ID="tps_false" GroupName="code_type" runat="server" Text="PTN" checked="true" />
                        <asp:RadioButton ID="tps_true" groupname="code_type" runat="server" Text="TPS" /><br />
                        <asp:TextBox ID="ptn" runat="server" Rows="30" TextMode="MultiLine"></asp:TextBox><br />
                        <asp:Button ID="go" runat="server" Text="Go!" OnClick="go_Click" />
                    </td>
                    <td valign="top">
                        <br /><br />
                        AI level: <asp:TextBox ID="aiLevel" runat="server" MaxLength="1" Type="number">3</asp:TextBox><br />
                        Flat Score: <asp:TextBox ID="flatScore" runat="server" MaxLength="1" Type="number">9000</asp:TextBox><br />
                        <b>The next move is: <asp:TextBox ID="move" runat="server"></asp:TextBox></b>
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
