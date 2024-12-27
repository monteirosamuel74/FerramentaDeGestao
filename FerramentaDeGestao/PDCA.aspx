<%@ Page Title="" Language="C#" MasterPageFile="~/Pagina.master" AutoEventWireup="true" CodeFile="Inicial.aspx.cs" Inherits="Inicial" EnableEventValidation="false" ValidateRequest="false" %>

<script runat="server">

    protected void btnAdicionar_Click(object sender, EventArgs e)
    {

    }
</script>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script type="text/javascript">
        function AdicionarPDCA() {
            document.getElementById("<%= btnAdicionar.ClientID%>").click();
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">


    <form id="form1" runat="server">
        <div>
            <h3>Adicionar Novo PDCA</h3>
            <label for="txtPlano">Plano: </label>
            <input type="text" id="txtPlano" runat="server" /><br />
            <label for="txtDesignacao">Designação: </label>
            <input type="text" id="txtDesignacao" runat="server" /><br />
            <label for="txtChecar">Checar: </label>
            <input type="text" id="txtChecar" runat="server" /><br />
            <label for="txtAcao">Ação: </label>
            <input type="text" id="txtAcao" runat="server" /><br />
            <label for="txtParticipantes">Participantes: </label>
            <input type="text" id="txtParticipantes" runat="server" /><br />
            <button type="button" onclick="AdicionarPDCA()">Adicionar</button>
        </div>
        <div>
            PDCA<br />
            <div class="col-sm-9">
                <table class="table table-bordered table-condensed table-responsive small">
                    <thead>
                        <tr>
                            <th>PLANO</th>
                            <th>DESIGNAÇÃO</th>
                            <th>CHECAR</th>
                            <th>AÇÃO</th>
                        </tr>
                    </thead>
                    <tbody id="conteudoPDCA" style="visibility: hidden">
                        <asp:Repeater runat="server" ID="rptPDCA">
                            <ItemTemplate>
                                <asp:TableRow>
                                    <td class="text-center"><%# DataBinder.Eval(Container.DataItem, "Plano") %></td>
                                    <td class="text-center"><%# DataBinder.Eval(Container.DataItem, "Designacao") %></td>
                                    <td class="text-center"><%# DataBinder.Eval(Container.DataItem, "Checar") %></td>
                                    <td class="text-center"><%# DataBinder.Eval(Container.DataItem, "Acao") %></td>
                                </asp:TableRow>
                            </ItemTemplate>
                        </asp:Repeater>
                    </tbody>
                </table>
            </div>
        </div>
    </form>
    <asp:Button ID="btnAdicionar" runat="server" Text="Adicionar" OnClick="btnAdicionar_Click" style="display:none;" />
</asp:Content>
