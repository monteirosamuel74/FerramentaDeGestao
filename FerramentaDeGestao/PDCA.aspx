<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="~/PDCA.aspx.cs" Inherits="FerramentaDeGestao.PDCA" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>PDCA - Ferramenta de Gestão</title>
    <link href="https://stackpath.bootstrapcdn.com/bootstrap/4.3.1/css/bootstrap.min.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <h2>PDCA</h2>
            <div class="row">
                <div class="col-sm-9">
                    <table class="table table-bordered table-condensed table-responsive small">
                        <thead>
                            <tr>
                                <th>PLANO</th>
                                <th>PRAZO PLANO</th>
                                <th>DESEMPENHAR</th>
                                <th>PRAZO DESEMPENHAR</th>
                                <th>CHECAR</th>
                                <th>PRAZO CHECAR</th>
                                <th>AÇÃO</th>
                                <th>PRAZO AÇÃO</th>
                                <th>PARTICIPANTES</th>
                            </tr>
                        </thead>
                        <tbody id="conteudoPDCA">
                            <asp:Repeater runat="server" ID="rptPDCA">
                                <ItemTemplate>
                                    <tr>
                                        <td class="text-center"><%# DataBinder.Eval(Container.DataItem, "Plano") %></td>
                                        <td class="text-center"><%# DataBinder.Eval(Container.DataItem, "PRAZO_PLANO") %></td>
                                        <td class="text-center"><%# DataBinder.Eval(Container.DataItem, "Desempenhar") %></td>
                                        <td class="text-center"><%# DataBinder.Eval(Container.DataItem, "PRAZO_DESEMPENHAR") %></td>
                                        <td class="text-center"><%# DataBinder.Eval(Container.DataItem, "Checar") %></td>
                                        <td class="text-center"><%# DataBinder.Eval(Container.DataItem, "PRAZO_CHECAR") %></td>
                                        <td class="text-center"><%# DataBinder.Eval(Container.DataItem, "Acao") %></td>
                                        <td class="text-center"><%# DataBinder.Eval(Container.DataItem, "PRAZO_ACAO") %></td>
                                        <td class="text-center"><%# DataBinder.Eval(Container.DataItem, "Participantes") %></td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tbody>
                    </table>
                </div>
            </div>

            <!-- Formulário para inserir dados do PDCA -->
            <div class="form-group">
                <h3>Adicionar Novo PDCA</h3>
                <label for="txtPlano">Plano: </label>
                <input type="text" id="txtPlano" runat="server" class="form-control" /><br />
                <label for="txtPrazoPlano">Prazo conclusão do Planejamento: </label>
                <input type="text" id="txtPrazoPlano" runat="server" class="form-control" /><br />
                <label for="txtDesempenhar">Desempenhar: </label>
                <input type="text" id="txtDesempenhar" runat="server" class="form-control" /><br />
                <label for="txtPrazoDesempenhar">Prazo conclusão da Ação: </label>
                <input type="text" id="txtPrazoDesempenhar" runat="server" class="form-control" /><br />
                <label for="txtChecar">Checar: </label>
                <input type="text" id="txtChecar" runat="server" class="form-control" /><br />
                <label for="txtPrazoChecar">Prazo conclusão da Checagem: </label>
                <input type="text" id="txtPrazoChecar" runat="server" class="form-control" /><br />
                <label for="txtAcao">Ação: </label>
                <input type="text" id="txtAcao" runat="server" class="form-control" /><br />
                <label for="txtPrazoAcao">Prazo conclusão da Ação: </label>
                <input type="text" id="txtPrazoAcao" runat="server" class="form-control" /><br />
                <label for="txtParticipantes">Participantes: </label>
                <input type="text" id="txtParticipantes" runat="server" class="form-control" /><br />
                <button type="button" class="btn btn-primary" onclick="AdicionarPDCA()">Adicionar</button>
            </div>
            <asp:Button ID="btnAdicionar" runat="server" Text="Adicionar" OnClick="btnAdicionar_Click" Style="display: none;" />
        </div>
    </form>

    <script type="text/javascript">
        function AdicionarPDCA() {
            document.getElementById("<%= btnAdicionar.ClientID %>").click();
        }
    </script>


</body>
</html>
