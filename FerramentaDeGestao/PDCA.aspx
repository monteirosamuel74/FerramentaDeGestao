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
                <div class="col-sm">
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
                                        <td class="text-center"><%# DataBinder.Eval(Container.DataItem, "PRAZO_PLANO", "{0:dd/MM/yyyy}") %></td>
                                        <td class="text-center"><%# DataBinder.Eval(Container.DataItem, "Desempenhar") %></td>
                                        <td class="text-center"><%# DataBinder.Eval(Container.DataItem, "PRAZO_DESEMPENHAR", "{0:dd/MM/yyyy}") %></td>
                                        <td class="text-center"><%# DataBinder.Eval(Container.DataItem, "Checar") %></td>
                                        <td class="text-center"><%# DataBinder.Eval(Container.DataItem, "PRAZO_CHECAR", "{0:dd/MM/yyyy}") %></td>
                                        <td class="text-center"><%# DataBinder.Eval(Container.DataItem, "Acao") %></td>
                                        <td class="text-center"><%# DataBinder.Eval(Container.DataItem, "PRAZO_ACAO", "{0:dd/MM/yyyy}") %></td>
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
                <label for="txtPrazoPlano">Prazo Plano: </label>
                <input type="date" id="dataPlano" runat="server" class="form-control col-sm-2" /><br />
                <label for="txtDesempenhar">Desempenhar: </label>
                <input type="text" id="txtDesempenhar" runat="server" class="form-control" /><br />
                <label for="txtPrazoDesempenhar">Prazo Desempenhar: </label>
                <input type="date" id="dataDesempenhar" runat="server" class="form-control col-sm-2" /><br />
                <label for="txtChecar">Checar: </label>
                <input type="text" id="txtChecar" runat="server" class="form-control" /><br />
                <label for="txtPrazoChecar">Prazo Checar: </label>
                <input type="date" id="dataChecar" runat="server" class="form-control col-sm-2" /><br />
                <label for="txtAcao">Ação: </label>
                <input type="text" id="txtAcao" runat="server" class="form-control" /><br />
                <label for="txtPrazoAcao">Prazo Ação: </label>
                <input type="date" id="dataAcao" runat="server" class="form-control col-sm-2" /><br />
                <label for="txtParticipantes">Participantes: </label>
                <div class="input-group">
                            <asp:TextBox ID="txtParticipante" runat="server" CssClass="caixaTexto" ReadOnly="true"></asp:TextBox>
                            <asp:Button ID="btnBuscarPessoa" runat="server" CssClass="botaoPesquisar" Text="Buscar" OnClientClick="abrirPopup();" />
                </div>

                <input type="hidden" id="participantesSelecionados" runat="server" />

                <%--<div class="modal" id="modalBuscaParticipantes" tabindex="-1" role="definition">
                    <div class="modal-dialog" role="document">
                        <div class="modal-content">
                            <div class="modal-header">
                                <h5 class="modal-title">Buscar Participantes</h5>
                                <button type="button" class="btn-close" data-dismiss="modal" aria-label="Close">
                                    <span aria-hidden="true">&times;</span>
                                </button>
                            </div>
                            <div class="modal-body">
                                <input type="text" id="txtBusca" runat="server" class="form-control" placeholder="Digite o nome do participante" />
                                <asp:Button ID="btnConsultar" runat="server" Text="Buscar" OnClick="btnConsultar_Click"/>
                                <ul id="resultadosBusca" class="list-group mt-3"></ul>
                                    <asp:DataGrid CssClass="rotulo" ID="gdrResultados" runat="server" AutoGenerateColumns="false"
                                        Width="100%" AllowPaging="true" CellPadding="2" PageSize="25" OnItemCreated="gdrResultados_ItemCreated" 
                                        OnItemDataBound="gdrResultados_ItemDataBound" Font-Size="Smaller" OnItemCommand="gdrResultados_ItemCommand" >
                                        <HeaderStyle Font-Bold="true" ForeColor="Black" BackColor="#E0DFE3" BorderColor="#CCCCCC" />
                                        <HeaderStyle Font-Bold="true" ForeColor="Black" BackColor="#E0DFE3" BorderColor="#CCCCCC" />
                                        <AlternatingItemStyle BackColor="WhiteSmoke" />
                                        <Columns>
                                            <asp:BoundColumn DataField="COLABORADOR_ID" HeaderText="C&#243;digo" Visible="false">
                                                <ItemStyle Width="50px" />
                                            </asp:BoundColumn>
                                            <asp:BoundColumn HeaderText="Nome" DataField="Nome" />
                                            <asp:BoundColumn HeaderText="Email" DataField="Email" />
                                        </Columns>
                                    </asp:DataGrid>
                            </div>
                            <div class="modal-footer">
                                <button type="button" class="btn-secondary" data-dismiss="modal">Fechar</button>
                                <button type="button" class="btn-primary" onclick="confirmarSelecao()">Confirmar Seleção</button>
                            </div>
                        </div>
                    </div>
                </div>--%>
            </div>
            <asp:Button ID="btnAdicionar" runat="server" Text="Adicionar" OnClick="btnAdicionar_Click" Style="display: none;" />
        </div>
    </form>
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.5.1/jquery.min.js"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.3.1/js/bootstrap.min.js"></script>
    <script type="text/javascript">
        function abrirPopup() {
            window.open('../PesqColab.aspx', 'popupWindow', 'width=700,height=500,scrollbars=no,resizable=no');
        }


        function confirmarSelecao() {
            var participantes = [];
            $("#resultadosBusca .list-group-item.active").each(function () {
                var participante = {
                    COLABORADOR_ID: $(this).data("id"),
                    Nome: $(this).text(),
                    Email: $(this).data("email")
                };
                participantes.push(participante);
            });
            if (participantes.length > 0) {
                var nomes = participantes.map(function (p) { return p.Nome; }).join(", ");
                $("#txtParticipantes").val(nomes);
                $("#participantesSelecionados").val("");
            } else {
                $("#txtParticipantes").val("");
                $("#participantesSelecionados").val("");
            }
            $("#modalBuscaParticipantes").modal('hide');
        }
        function selecionarParticipante(nome, id, email) {
            $("#txtParticipantes").val(nome);

        }

    </script>


</body>
</html>
