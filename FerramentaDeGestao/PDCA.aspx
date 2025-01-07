<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="~/PDCA.aspx.cs" Inherits="FerramentaDeGestao.PDCA" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css" rel="stylesheet"> 
    <script src="https://code.jquery.com/jquery-3.5.1.slim.min.js"></script> 
    <script src="https://cdn.jsdelivr.net/npm/@popperjs/core@2.5.4/dist/umd/popper.min.js"></script> 
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.min.js"></script>

    <title>PDCA - Ferramenta de Gestão</title>
    
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

                <label for="rptParticipantes">Participantes: </label>
                <div class="input-group">
                    <asp:Repeater ID="rptParticipantes" runat="server">
                        <ItemTemplate>
                            <div><%# Container.DataItem %></div>
                        </ItemTemplate>
                    </asp:Repeater>
                    <asp:Button ID="btnBuscarPessoa" runat="server" CssClass="botaoPesquisar" Text="Buscar" OnClientClick="abrirPopup();" />
                </div>
                <input type="hidden" id="participantesSelecionados" runat="server" />

                <label for="txtPlano">Plano: </label>
                <div class="input-group">
                    <input type="text" id="txtPlano" runat="server" class="form-control" /><br />
                    <div class="input-group-append">
                        <span id="helpPlano" class="input-group-text" data-toggle="popover" title="Dica">
                            <i class="fa fa-question-circle"></i>
                        </span>
                    </div>
                </div>
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

            </div>
            <asp:Button ID="btnAdicionar" runat="server" Text="Adicionar" OnClick="btnAdicionar_Click" Style="display: none;" />
        </div>
    </form>
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.5.1/jquery.min.js"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.3.1/js/bootstrap.min.js"></script>
    <script type="text/javascript">
        function abrirPopup() {
            window.open('PesqColab.aspx', 'popupWindow', 'width=700,height=500,scrollbars=no,resizable=no');
        }

        function atualizarParticipantes() {
            __doPostBack('atualizarParticipantes', '');
        }

        var dicaPlano = "Plan (planejar)<br>Na primeira etapa, você vai planejar a mudança que vai fazer. Pode ser a ação corretiva de um risco, uma oportunidade de melhoria ou um projeto.<br>"+
            "Partindo da identificação da situação atual e da coleta de dados, você vai definir o que deseja mudar, por que deseja mudar e como vai mudar.<br>" +
            "Isso se traduzirá no desenho de um projeto com escopo, abordagem, recursos, pessoas envolvidas e indicadores.";
        $(document).ready(function () {
            $('#helpPlano').popover({ content: dicaPlano, html: true });
        });

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
