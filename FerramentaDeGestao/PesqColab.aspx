<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PesqColab.aspx.cs" Inherits="FerramentaDeGestao.PesqColab" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <table>
                <tr>
                    <td>
                        <asp:TextBox ID="txtPesqColab" runat="server" Width="300px" ValueField="Descrição" Obrigatorio="true"></asp:TextBox>
                        <asp:RequiredFieldValidator runat="server" ID="rfvPesquisa" Display="None" EnableClientScript="true"
                            ControlToValidate="pesqColab" ErrorMessage="Informe o nome para pesquisa."></asp:RequiredFieldValidator>
                        <asp:Button ID="btnPesqColab" runat="server" Text="Pesquisar" OnClick="btnPesqColab_Click" />
                    </td>
                </tr>
            </table>
            <asp:DataGrid ID="grdResultados" runat="server" AutoGenerateColumns="false" Width="100%" AllowPaging="true" CellPadding="2" PageSize="25"
                OnItemCreated="grdResultados_ItemCreated" OnItemDataBound="grdResultados_ItemDataBound" OnItemCommand="grdResultados_ItemCommand">
                <HeaderStyle Font-Bold="true" ForeColor="Black" BackColor="WhiteSmoke" BorderColor="White" />
                <ItemStyle ForeColor="Black" BorderColor="WhiteSmoke" BackColor="White" />
                <AlternatingItemStyle BackColor="Wheat" />
                <Columns>
                    <asp:BoundColumn DataField="COLABORADOR_ID" HeaderText="C&#243;digo">
                        <ItemStyle Width="50px" />
                    </asp:BoundColumn>
                    <asp:BoundColumn DataField="Nome" Visible="false"></asp:BoundColumn>
                    <asp:TemplateColumn HeaderText="Nome" SortExpression="Nome">
                        <ItemTemplate>
                            <asp:LinkButton ID="lkbNome" runat="server" CommandName="Selecionar" CausesValidation="false"
                                ToolTip="Clique para selecionar esse registro."></asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:BoundColumn DataField="Email" HeaderText="E-mail" Visible="true" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center"></asp:BoundColumn>
                </Columns>
                <PagerStyle Mode="NumericPages" Visible="false" />
            </asp:DataGrid>
        </div>
    </form>
</body>
</html>
