<%@ Page Title="" Language="C#" MasterPageFile="~/Paginas.master" AutoEventWireup="true" CodeFile="Master.aspx.cs" Inherits="Master" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">

    <div class="row ofs" runat="server" id="divAviso" visible="False">
        <div class="col-md-12 text-center">
            <h1>A conteúdo desta página não é permitida para seu usuário</h1>
            <h3>
                <asp:Label runat="server" ID="LblAviso" />
            </h3>
            <a class="btn btn-danger" href="Inicial.aspx">Retornar para tela inicial</a>
        </div>
    </div>

    <div class="row" runat="server" id="divParent">
        <asp:UpdatePanel runat="server" ID="Update">
            <ContentTemplate>
                <div class="col-md-12">
                    <!-- Controle -->
                    <div class="panel panel-default">
                        <div class="panel-heading">
                            <h3 class="panel-title">Dados de Acesso</h3>
                        </div>
                        <div class="panel-body">
                            <div class="col-md-12" style="margin-bottom: 2%">
                                <strong>Nome</strong><br />
                                <asp:Label runat="server" ID="LblNome" />
                            </div>
                            <div class="col-md-6">
                                <strong>Selecione o Colaborador</strong>
                                <asp:DropDownList runat="server" CssClass="form-control select2" ID="DropColaborador" />
                            </div>
                            <div class="col-md-3">
                                <strong>Visualizar</strong>
                                <asp:DropDownList runat="server" CssClass="form-control select2" ID="DropPage">
                                    <asp:ListItem Text="[C.I] - Fluxo de Controle" Value="0" />
                                    <asp:ListItem Text="[C.I] - Fluxo Enviadas" Value="1" />
                                    <asp:ListItem Text="[C.I] - Fluxo Recebidas" Value="2" />
                                    <asp:ListItem Text="[C.I] - Fluxo Arquivadas" Value="3" />
                                </asp:DropDownList>
                            </div>
                            <div class="col-md-3 top1">
                                <asp:LinkButton runat="server" CssClass="btn btn-info" ID="LnkConsultar" OnClick="LnkConsultar_OnClick">
                            Consultar &nbsp;<i class="glyphicon glyphicon-arrow-right"></i>
                                </asp:LinkButton>
                            </div>
                        </div>
                    </div>

                    <!-- Histórico -->
                    <div class="panel panel-default">
                        <div class="panel-heading">
                            <h3 class="panel-title">Histórico</h3>
                        </div>
                        <div class="panel-body no-padding">
                            <table class="table table-hover table-striped table-bordered">
                                <thead>
                                    <tr>
                                        <th>Nome</th>
                                        <th class="text-center">Data</th>
                                        <th class="text-center">Tipo</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <asp:Repeater runat="server" ID="RptAcessos">
                                        <ItemTemplate>
                                            <tr>
                                                <td>
                                                    <%# DataBinder.Eval(Container.DataItem, "NOME")%>
                                                </td>
                                                <td class="text-center">
                                                    <%# DataBinder.Eval(Container.DataItem, "DATA")%>
                                                </td>
                                                <td class="text-center">
                                                    <%# DataBinder.Eval(Container.DataItem, "TIPO")%>
                                                </td>
                                            </tr>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </tbody>
                            </table>
                            <div id="divPagina" runat="server">
                                <ul class="pagination">
                                    <li>
                                        <asp:LinkButton ID="lnkFirst" runat="server" OnClick="lnkFirst_Click">Primeira</asp:LinkButton></li>
                                    <li>
                                        <asp:LinkButton ID="lnkPrevious" runat="server" OnClick="lnkPrevious_Click">&laquo;</asp:LinkButton></li>
                                    <asp:Repeater ID="RepeaterPaging" runat="server" OnItemCommand="RepeaterPaging_ItemCommand"
                                        OnItemDataBound="RepeaterPaging_ItemDataBound">
                                        <ItemTemplate>
                                            <li id="li" runat="server">
                                                <asp:LinkButton ID="Pagingbtn" runat="server" CommandArgument='<%# Eval("PageIndex") %>'
                                                    CommandName="newpage" Text='<%# Convert.ToString(Convert.ToInt32(DataBinder.Eval(Container.DataItem, "PageText")) + 1) %>'></asp:LinkButton>
                                            </li>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                    <li>
                                        <asp:LinkButton ID="lnkNext" runat="server" OnClick="lnkNext_Click">&raquo;</asp:LinkButton></li>
                                    <li>
                                        <asp:LinkButton ID="lnkLast" runat="server" OnClick="lnkLast_Click">Última</asp:LinkButton></li>
                                </ul>
                            </div>
                        </div>
                    </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>

    <style>
        .ofs {
            min-height: 500px;
            padding-top: 15%;
        }

        .top1 {
            margin-top: 18px;
        }
    </style>
</asp:Content>
