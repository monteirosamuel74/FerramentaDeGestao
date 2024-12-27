using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using System.IO;
using Microsoft.AspNet.SignalR.Hosting;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Infrastructure;
using ChatSample;
using System.Globalization;
using System.Security.Cryptography;
using System.Web.UI.HtmlControls;
using NITGEN.SDK.NBioBSP;
using System.Drawing;

public partial class Paginas : System.Web.UI.MasterPage
{
    NBioAPI m_NBioAPI;
    NBioAPI.Type.WINDOW_OPTION m_WinOption;
    static uint qualidade = 0;

    public string TituloPagina
    {
        get { return ltTitulo.Text; }
        set { ltTitulo.Text = value; }
    }

    protected string javascript
    {
        get
        {
            string javascript = string.Empty;

            if (Session["javascript"] != null)
                javascript = Session["javascript"].ToString();

            return javascript;
        }
        set
        {
            Session["javascript"] = value;
        }
    }

    protected string javascript2
    {
        get
        {
            string javascript2 = string.Empty;

            if (Session["javascript2"] != null)
                javascript2 = Session["javascript2"].ToString();

            return javascript2;
        }
        set
        {
            Session["javascript2"] = value;
        }
    }

    protected string javascript3
    {
        get
        {
            string javascript3 = string.Empty;

            if (Session["javascript3"] != null)
                javascript3 = Session["javascript3"].ToString();

            return javascript3;
        }
        set
        {
            Session["javascript3"] = value;
        }
    }

    protected string javascript4
    {
        get
        {
            string javascript4 = string.Empty;

            if (Session["javascript4"] != null)
                javascript4 = Session["javascript4"].ToString();

            return javascript4;
        }
        set
        {
            Session["javascript4"] = value;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["ACESSO_GARANTIDO"] != null)
        {
            LnkModoMaster.Visible = true;
        }

        if (Session["DTLOGADO"] == null)
        {
            Response.Redirect("Login.aspx");
        }

        Titulo();
        Notificacoes();

        liBonus.Visible = (DadosUsuarioLogado.SETOR_ID == 1 || DadosUsuarioLogado.SETOR_ID == 13 || DadosUsuarioLogado.DESENVOLVEDOR == "S" || (DadosUsuarioLogado.SETOR_ID == 61 && (DadosUsuarioLogado.CARGO_ID == 36 || DadosUsuarioLogado.CARGO_ID == 37 || DadosUsuarioLogado.CARGO_ID == 1078)) && DadosUsuarioLogado.NIVEL == 3);

        if (DadosUsuarioLogado.TEMBIOMETRIA == 1)
        {
            btnRegistrar.Visible = false;
            VisualizarPonto(DadosUsuarioLogado.COLABORADOR_ID.ToString());
            UpdatePonto.Update();
        }

        if (DadosUsuarioLogado.HORA_ALMOCO == 1 || (DadosUsuarioLogado.TIPO_PESSOA == "J" && DadosUsuarioLogado.SETOR_ID == 1))
        {
            divMenu.Visible = false;
            divContainer.Attributes.Remove("class");
            divContainer.Attributes.Add("class", "page-container sidebar-collapsed");
            ScriptManager.RegisterStartupScript(this, GetType(), "alll", "document.getElementById('menuside').style.display='none';", true);
            liHolerites.Visible = false;
            ulMenuTopo.Visible = false;
            lnkSairPJ.Visible = true;
            ulOpcoes.Visible = false;
        }

        if (Request.Url.AbsolutePath.ToLower().Contains("inicial.aspx"))
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "alll", "document.getElementById('DivMenuMobile').style.display='none';", true);
        }

        lb_nome.Text = DadosUsuarioLogado.NOME_COLABORADOR;

        //ScriptManager.RegisterStartupScript(this, GetType(), "", "$('html, body').animate({ scrollTop: $(\"#" + ContentPlaceHolder1.ClientID + "\").offset().top }, 10);", true);

        liConfig.Visible = DadosUsuarioLogado.COLABORADOR_ID == 79;
        lblSenhaAcesso.Text = Utilitarios.getIdentMensalNew(DateTime.Today.Month, DateTime.Today.Year, Convert.ToInt32(DadosUsuarioLogado.COLABORADOR_ID)).ToLower();

        if (Request.QueryString["janela"] != null && Request.QueryString["janela"].ToString() != "")
        {
            divMenu.Visible = false;
            divContainer.Attributes.Remove("class");
            divContainer.Attributes.Add("class", "page-container sidebar-collapsed");
            ScriptManager.RegisterStartupScript(this, GetType(), "alll", "document.getElementById('menuside').style.display='none';document.getElementById('divTopo').style.display='none';", true);
        }

        //search.Visible = true;
        if (DadosUsuarioLogado.MENU_MINIMIZADO > 0)
        {
            divContainer.Attributes.Remove("class");
            divContainer.Attributes.Add("class", "page-container sidebar-collapsed");
            //search.Visible = false;
        }

        liColRH.Visible = DadosUsuarioLogado.NIVEL < 3 || DadosUsuarioLogado.SETOR_ID == 18 || DadosUsuarioLogado.SETOR_ID == 2;
        liFilaDepuracao.Visible = DadosUsuarioLogado.NIVEL < 3 || DadosUsuarioLogado.SETOR_ID == 1 || DadosUsuarioLogado.LIDER_REGIAO;

        tdColaboradores.Visible = liColRH.Visible;
        tdViagem.Visible = liColRH.Visible;

        liAvaliacao.Visible = DadosUsuarioLogado.COLABORADOR_ID == 79 || DadosUsuarioLogado.COLABORADOR_ID == 1181 || DadosUsuarioLogado.COLABORADOR_ID == 1075
            || DadosUsuarioLogado.COLABORADOR_ID == 15 || DadosUsuarioLogado.NIVEL < 2;

        liPlanilhaCusto.Visible = DadosUsuarioLogado.NIVEL <= 2 || DadosUsuarioLogado.SETOR_ID == 12;
        liConfigInternas.Visible = DadosUsuarioLogado.COLABORADOR_ID == 79;

        if (!IsPostBack)
        {
            txtNome.Text = DadosUsuarioLogado.NOME_COLABORADOR;
            txtRamal.Text = DadosUsuarioLogado.RAMAL;
            ddlSexo.SelectedValue = DadosUsuarioLogado.SEXO;
            txtBiografia.InnerText = DadosUsuarioLogado.BIOGRAFIA;

            //Verifica se tem senha no tramite
            if (DadosUsuarioLogado.SENHA_TRAMITE == "N")
            {
                btnSenhaTramite.Attributes.Add("class", "btn btn-danger btn-xs");
                btnSenhaTramite.Text = "Tramite de chamados não requer senha";
            }
            else
            {
                btnSenhaTramite.Attributes.Add("class", "btn btn-success btn-xs");
                btnSenhaTramite.Text = "Tramite de chamados requer senha";
            }

            //Verificação para a justificava - liberado somente para Thiago Araújo
            //if (DadosUsuarioLogado.COLABORADOR_ID == 393 || DadosUsuarioLogado.COLABORADOR_ID == 79)
            //    lnkJustificativa.Visible = true;
            //else
            lnkJustificativa.Visible = false;

            if (DadosUsuarioLogado.NIVEL < 3 || DadosUsuarioLogado.SETOR_ID == 19 || DadosUsuarioLogado.SETOR_ID == 5 || DadosUsuarioLogado.CARGO_ID == 1077)
            {
                liComercial.Visible = true;
            }
            else if (DadosUsuarioLogado.LIDER_REGIAO)
            {
                liComercial.Visible = true;
                liContratos.Visible = false;
            }

            int[] CARGOS = { 2, 3, 4, 5, 15, 16, 22, 23, 24 };
            int[] SETOR_DITEC = { 1, 2, 11, 13, 14, 19, 24, 35, 45, 46, 48, 49, 61, 62, 63, 96, 33, 64 };

            if (DadosUsuarioLogado.NIVEL < 3 || DadosUsuarioLogado.SETOR_ID == 18 || CARGOS.Contains(DadosUsuarioLogado.CARGO_ID))
            {
                liApoio.Visible = true;
            }

            if ((DadosUsuarioLogado.NIVEL < 3 && SETOR_DITEC.Contains(DadosUsuarioLogado.SETOR_ID)) || DadosUsuarioLogado.COLABORADOR_ID == 999)
            {
                liDiretoria.Visible = true;
                liFilaManutencaoCorretiva.Visible = true;

                DataTable dtDire = Utilitarios.Pesquisar(@"SELECT * FROM VW_CHAMADOS_GERAL G 
                        CROSS APPLY (SELECT TOP 1 D.DATA AS DATA_PRIORIDADE, C.NOME AS COLABORADOR_PRIORIDADE FROM PRIORIDADE_DIRETORIA D 
						JOIN EXTRANET.COLABORADOR C ON C.COLABORADOR_ID = D.COLABORADOR_ID
						WHERE D.CODIGO_HELPDESK=G.CODIGO AND TIPO='P' ORDER BY 1 DESC) PD
                        WHERE COALESCE(G.PRIORIDADE_DIRETORIA, 'N') = 'S' AND G.STATUS NOT IN (2,7)
                        ORDER BY G.DATA_PREVISTA_DT ASC");

                rptDiretoria.DataSource = dtDire;
                rptDiretoria.DataBind();
                //liDiretoria.Visible = dtDire.Rows.Count > 0;
                ltDiretoria.Text = dtDire.Rows.Count.ToString();
            }

            lnkHistorico.Visible = (DadosUsuarioLogado.SETOR_ID == 1 || DadosUsuarioLogado.SETOR_ID == 13 || DadosUsuarioLogado.NIVEL <= 2);

            //Coordenadores Codes e CQS
            if (DadosUsuarioLogado.SETOR_ID == 1 || DadosUsuarioLogado.SETOR_ID == 13 || DadosUsuarioLogado.NIVEL < 2)
                liSprints.Visible = true;

            // libera o pesquisa e satisfação apenas para coordenadores,lideres de região 176944, SETOR COMERCIAL E SETOR AGQ 183854          
            if (DadosUsuarioLogado.NIVEL < 3 || DadosUsuarioLogado.LIDER_REGIAO || DadosUsuarioLogado.SETOR_ID == 5 || DadosUsuarioLogado.SETOR_ID == 45)
            {
                lnkPesquisaSatisfacao.Visible = true;
            }

            ltAcomp.Text = getQtdeChamados("lnkAcompanho");
            ltRec.Text = getQtdeChamados("lnkReceber");
            ltResp.Text = getQtdeChamados("lnkResponsabilidade");

            string nivel = DadosUsuarioLogado.NIVEL.ToString();
            if (!string.IsNullOrEmpty(nivel))
            {
                if (Convert.ToInt32(nivel) < 3)
                {
                    liAdm.Visible = true;
                    liPainel.Visible = true;
                    liGeradorSql.Visible = true;
                }
                else
                {
                    liPainel.Visible = false;
                    liAdm.Visible = false;
                    liGeradorSql.Visible = false;
                }
            }

            if (DadosUsuarioLogado.NIVEL < 2)
            {
                liMetas.Visible = true;
            }


            if ((DadosUsuarioLogado.COLABORADOR_ID > 0 && DadosUsuarioLogado.META_ENTREGA == "S") || DadosUsuarioLogado.NIVEL < 3)
            {
                liMetasEntregas.Visible = true;
            }

            if (DadosUsuarioLogado.LIDER_REGIAO)
                liPainel.Visible = true;

            CarregaPendencias();

            if (DadosUsuarioLogado.COLABORADOR_ID == 393 || DadosUsuarioLogado.COLABORADOR_ID == 79 || DadosUsuarioLogado.SETOR_ID == 18 ||
                DadosUsuarioLogado.CARGO_ID == 24 || DadosUsuarioLogado.CARGO_ID == 2 || DadosUsuarioLogado.CARGO_ID == 4 || DadosUsuarioLogado.NIVEL < 2)
            {
                liMotivacional.Visible = true;
            }

            if (DadosUsuarioLogado.COLABORADOR_ID == 18)
                liMaster.Visible = true;

            if (DadosUsuarioLogado.PERMITEPONTO == 1)
                divPonto.Visible = true;

            if (DadosUsuarioLogado.SETOR_ID == 18 || DadosUsuarioLogado.COLABORADOR_ID == 79)
            {
                liFerias.Visible = true;
                ltFolhaPonto.Visible = true;
            }

            if (!string.IsNullOrEmpty(DadosUsuarioLogado.CODIGO_PAPER))
            {
                LerArquivosHolerite();
            }

            ConsultaCrpAnoCorrente();

            //Dados de assinatura de email
            bool AssinaturaEmail = true;
            fsAssinaturaEmail.Visible = AssinaturaEmail;
            if (AssinaturaEmail)
            {
                DataTable _dt = Utilitarios.Pesquisar("SELECT nome_ass_email, cargo_ass_email, setor_ass_email FROM extranet.colaborador WHERE colaborador_id = " + DadosUsuarioLogado.COLABORADOR_ID);
                txtNomeAssEmail.Text = _dt.Rows[0]["nome_ass_email"] != null ? _dt.Rows[0]["nome_ass_email"].ToString() : string.Empty;
                txtCargoAssEmail.Text = _dt.Rows[0]["cargo_ass_email"] != null ? _dt.Rows[0]["cargo_ass_email"].ToString() : string.Empty;
                txtSetorAssEmail.Text = _dt.Rows[0]["setor_ass_email"] != null ? _dt.Rows[0]["setor_ass_email"].ToString() : string.Empty;
            }

            //Coordenador Codes/Cqs, chamados aguardando estimativa
            if ((DadosUsuarioLogado.SETOR_ID == 1 || DadosUsuarioLogado.SETOR_ID == 13 || DadosUsuarioLogado.CARGO_ID == 1105) && DadosUsuarioLogado.NIVEL < 3)
                CarregarChamadosAguardandoEstimativa();
            //Copet ou Coac
            else if (DadosUsuarioLogado.SETOR_ID == 19 || DadosUsuarioLogado.SETOR_ID == 21)
                CarregarChamadosEstimados();


            if (DadosUsuarioLogado.NIVEL < 3 || DadosUsuarioLogado.COLABORADOR_ID == 802)
            {
                LiBotaoRelatorio.Visible = true;
            }

            //Habilita o gerenciamento dos Arquivos Auxiliares para o CODES
            //http://extranet.agendaassessoria.com.br/agendaassessoria/downloads.aspx
            if (DadosUsuarioLogado.SETOR_ID == 1 || DadosUsuarioLogado.NIVEL < 3 || DadosUsuarioLogado.DESENVOLVEDOR == "S")
                liDownloadArquivosAuxiliares.Visible = true;

            //Dados Carteirinha de Saúde Bradesco
            if (!string.IsNullOrWhiteSpace(DadosUsuarioLogado.NR_CARTEIRINHA_PLANO_SAUDE))
            {
                divCarteirinhaPlano.Visible = true;
                lblNumeroCarteirinha.Text = DadosUsuarioLogado.NR_CARTEIRINHA_PLANO_SAUDE;
            }

            if (DadosUsuarioLogado.TIPO_PESSOA == "H")
            {
                bool HorarioPermitido = Utilitarios.VerificaDiaHorario(DadosUsuarioLogado.COLABORADOR_ID);

                liWorkFlow.Visible = HorarioPermitido;
                liSGQ.Visible = HorarioPermitido;
                lIControle.Visible = HorarioPermitido;
                liPainel.Visible = HorarioPermitido;
                liMensagens.Visible = HorarioPermitido;
                liVideos.Visible = HorarioPermitido;
            }


            if ((Session["AVALIACAO_VISTO"] == null || Session["AVALIACAO_VISTO"].ToString() != "S") &&
                Utilitarios.Pesquisar("SELECT * FROM EXTRANET.COLABORADOR WHERE COLABORADOR_ID NOT IN " +
                "(18, 15, 1, 13, 156, 438, 7, 274) AND COLABORADOR_ID=" + DadosUsuarioLogado.COLABORADOR_ID.ToString()).Rows.Count > 0)
            {
                ConsultaAvaliacao("true");
            }

            GerenciarModais();
        }

    }

    private void CarregarChamadosAguardandoEstimativa()
    {
        string sql = @"SELECT HE.CODIGO_HELPDESK, FORMAT(HE.DATA_INSERT, 'dd/MM/yyyy HH:mm') AS DATA_INSERT, H.TITULO, CASE WHEN DATEDIFF(HOUR, HE.DATA_INSERT, GETDATE()) >= 24 THEN 'estimativa-expirada' END AS CSS_ESTIMATIVA_EXPIRADA 
            FROM HELPDESK_ESTIMATIVA HE JOIN HELPDESK H ON HE.CODIGO_HELPDESK = H.CODIGO WHERE H.STATUS NOT IN(2, 7)";
        if (DadosUsuarioLogado.SETOR_ID == 1 || DadosUsuarioLogado.CARGO_ID == 1105)
            sql += " AND COALESCE(H.HORASCODES,0) <= 0 AND HE.DATA_ESTIMOU_CODES IS NULL"; //Não estimados CODES
        else if (DadosUsuarioLogado.SETOR_ID == 13)
            sql += " AND COALESCE(H.HORASCQS,0) <= 0 AND HE.DATA_ESTIMOU_CQS IS NULL"; //Não estimados CQS
        else
            sql += " AND ((COALESCE(H.HORASCODES,0) <= 0 AND HE.DATA_ESTIMOU_CODES IS NULL) OR(COALESCE(H.HORASCQS, 0) <= 0 AND HE.DATA_ESTIMOU_CQS IS NULL))"; //Não estimados CODES ou CQS

        sql += " ORDER BY HE.DATA_INSERT";

        DataTable dt = Utilitarios.Pesquisar(sql);

        rptChamadosAguardandoEstimativa.DataSource = dt;
        rptChamadosAguardandoEstimativa.DataBind();

        liChamadosAguardandoEstimativa.Visible = dt.Rows.Count > 0;
        ltEstimar.Text = dt.Rows.Count.ToString();
    }

    private void CarregarChamadosEstimados()
    {
        string sql = @"SELECT HE.CODIGO_HELPDESK, FORMAT(HE.DATA_INSERT, 'dd/MM/yyyy HH:mm') AS DATA_INSERT, H.TITULO 
            FROM HELPDESK_ESTIMATIVA HE 
            JOIN HELPDESK H ON HE.CODIGO_HELPDESK = H.CODIGO 
            WHERE H.STATUS NOT IN(2, 7)
            AND HE.DATA_ESTIMOU_CODES IS NOT NULL 
            AND HE.DATA_ESTIMOU_CQS IS NOT NULL
            AND HE.USR_INSERT = " + DadosUsuarioLogado.COLABORADOR_ID + @" 
            AND HE.USR_INSERT_DATA_VISUALIZOU IS NULL
            ORDER BY HE.DATA_INSERT";

        DataTable dt = Utilitarios.Pesquisar(sql);

        rptEstimados.DataSource = dt;
        rptEstimados.DataBind();

        liEstimados.Visible = dt.Rows.Count > 0;
        ltEstimados.Text = dt.Rows.Count.ToString();
    }

    protected void rptEstimados_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case "visualizar":
                try
                {
                    Utilitarios.Exec_StringSql("update helpdesk_estimativa set usr_insert_data_visualizou = getdate() where codigo_helpdesk = " + e.CommandArgument + " and usr_insert = " + DadosUsuarioLogado.COLABORADOR_ID);
                    Response.Redirect("Chamado.aspx?CODIGO=" + e.CommandArgument, false);
                }
                catch (Exception ex)
                {
                    Response.Redirect("Chamado.aspx?CODIGO=" + e.CommandArgument, false);
                }
                break;
        }
    }

    //Retorna true se exibir o modal
    private bool MensagemMotivacao()
    {
        if (Session["VISTO"] != null)
        {
            return false;
        }

        //pega mensagem ativa
        var msg = "set dateformat dmy; " +
                  "select* from MensagemMotivacional where cast(DataInicio as date) " +
                  "<= cast(getdate() as date) and cast(DataFim as date) >= cast(getdate() as date) " +
                  "and IdMensagem not in (select IdMensagem from MensagemMotivacionalLidas where colaboradorId = " + DadosUsuarioLogado.COLABORADOR_ID + ")";

        var dt = Utilitarios.Pesquisar(msg);
        if (dt == null || dt.Rows.Count < 1) return false;

        LblTituloMensagem.Text = dt.Rows[0]["Titulo"].ToString();
        LblMensagem.Text = dt.Rows[0]["Mensagem"].ToString();
        HddMensagemId.Value = dt.Rows[0]["IdMensagem"].ToString();

        Session["VISTO"] = true;

        ScriptManager.RegisterStartupScript(this, GetType(), "modal", "$('#modalMotivacao').modal('show');", true);
        return true;
    }

    private void CarregaPendencias()
    {
        string sql = string.Empty;
        string where = "(RESPONSAVEL_VIAGEM = " + DadosUsuarioLogado.COLABORADOR_ID +
            " OR SETOR_RESPONSAVEL = " + DadosUsuarioLogado.SETOR_ID +
            ") AND 1 = CASE WHEN COALESCE(AUTORIZA,'')='S' AND " + DadosUsuarioLogado.SETOR_ID + " IN (7,10) OR (RESPONSAVEL_VIAGEM = " +
            DadosUsuarioLogado.COLABORADOR_ID + ") OR (ADM_VISTO = 'S' AND RESPONSAVEL_VIAGEM = SOLICITANTE_ID)  THEN 1 ELSE 2 END ";

        sql = "SELECT * FROM EXTRANET.VIAGEM A JOIN EXTRANET.CLIENTE B ON A.CLIENTE_ID = B.CLIENTE_ID" +
              " JOIN EXTRANET.COLABORADOR C ON A.COLABORADOR_ID = C.COLABORADOR_ID " +
              "WHERE " + where + " AND COALESCE(FINALIZA_VIAGEM, 'N') <> 'S' AND ARQUIVADA = 0 ORDER BY VIAGEM_ID DESC";

        DataTable dt = Utilitarios.Pesquisar(sql);

        if (dt.Rows.Count > 0)
        {
            idViagens.Visible = true;
            RptViagens.DataSource = dt;
            RptViagens.DataBind();

            LtlViagens.Text = dt.Rows.Count.ToString();
            Session["ViagemRec"] = LtlViagens.Text;
        }
        else
        {
            LtlViagens.Text = "0";
            idViagens.Visible = false;
        }
    }

    public void AtualizaAcompanhar()
    {
        ltAcomp.Text = getQtdeChamados("lnkAcompanho");
    }

    protected void lnkSair_Click(object sender, EventArgs e)
    {
        Session.Abandon();
        Session["VISTO"] = null;
        Response.Redirect("Default.aspx");
    }

    protected void rptSetor_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
        {
            Repeater rpt = (Repeater)e.Item.FindControl("rptCol");
            HiddenField hdd = (HiddenField)e.Item.FindControl("hddSetorId");

            string ativo = chkRptSetorInativos.Checked ? "" : " AND ATIVO='S'";
            DataTable dt = Utilitarios.Pesquisar("SELECT C.COLABORADOR_ID, UPPER(C.NOME) AS NOME, C.EMAIL, C.RAMAL, CA.NOME AS CARGO, C.FOTO FROM EXTRANET.COLABORADOR C " +
                "JOIN EXTRANET.CARGO CA ON CA.CARGO_ID = C.CARGO_ID JOIN EXTRANET.SETOR ES ON C.SETOR_ID = ES.SETOR_ID WHERE C.SETOR_ID=" + hdd.Value + " " + ativo + " AND C.COLABORADOR_ID>0 " +
                "AND (C.NOME LIKE '%" + txtProcColab.Text.Replace("'", "") + "%' OR ES.NOME LIKE '%" + txtProcColab.Text.Replace("'", "") +
                "%') AND C.NOME NOT LIKE 'FILA%' ORDER BY CA.NIVEL, C.NOME");
            rpt.DataSource = dt;
            rpt.DataBind();
        }
    }

    protected void btnOCulto_Click(object sender, EventArgs e)
    {
        if (txtBusca.Text.Trim() != string.Empty && ValidaNumero(txtBusca.Text.Trim()))
        {
            int existe = Convert.ToInt32(Utilitarios.Exec_StringSql_Return("SELECT COUNT(*) FROM HELPDESK WHERE CODIGO=" + txtBusca.Text.Trim()));
            if (existe > 0)
                Response.Redirect("Chamado.aspx?CODIGO=" + txtBusca.Text.Trim());
            else
                ScriptManager.RegisterClientScriptBlock(this, GetType(), "msg", "<script>alert('Número do chamado não existe!');</script>", false);
        }
        else if (txtBusca.Text.Trim() != string.Empty && !ValidaNumero(txtBusca.Text.Trim()))
            Response.Redirect("Chamados.aspx?TipoBusca=BuscaAvancada&texto=" + txtBusca.Text.Trim());
        else
            ScriptManager.RegisterClientScriptBlock(this, GetType(), "msg", "<script>alert('Digite o número do chamado para buscar!');</script>", false);
        // ScriptManager.RegisterStartupScript(this, GetType(), "asd", "alert('Digite para buscar!')", false);
    }

    private void Titulo()
    {
        string titulo = Request.ServerVariables["PATH_INFO"].ToString().Replace("/", "").Replace(".aspx", "").ToUpper();

        if (!IsPostBack)
        {
            DataTable dt = Utilitarios.Pesquisar("SELECT SETOR_ID, NOME FROM EXTRANET.SETOR WHERE SETOR_ID IN (SELECT SETOR_ID FROM EXTRANET.COLABORADOR WHERE ATIVO='S') ORDER BY NOME");
            rptSetor.DataSource = dt;
            rptSetor.DataBind();

            chkRptSetorInativos.Visible = DadosUsuarioLogado.NIVEL < 3;
        }

        if (Request.QueryString["titulo"] != null && Request.QueryString["titulo"].ToString() != "")
        {
            titulo = Request.QueryString["titulo"].ToString();

            if (Request.QueryString["tbl"] != null)
            {
                liUST.Attributes.Add("class", "opened");
                liClientes.Attributes.Add("class", "opened");

                if (Request.QueryString["tbl"].Contains("CLIENTE") || Request.QueryString["tbl"].Contains("UNIDADE_SERVICOS") || Request.QueryString["tbl"].Contains("PONTUACAO"))
                {
                    liUSTComp.Attributes.Add("class", "opened");
                }
                else
                {
                    liUSTTab.Attributes.Add("class", "opened");
                }
            }
        }

        if (titulo.Contains("CHAMADO") || titulo.Contains("CADCATEGORIAS"))
        {
            liChamados.Attributes.Add("class", "opened");
            titulo = "CHAMADOS";

            if (titulo.Contains("CADCATEGORIAS"))
                liConfig.Attributes.Add("class", "opened");

            if (Request.QueryString["texto"] != null && Request.QueryString["texto"].ToString() != string.Empty)
            {
                Session["TipoBusca"] = "lnkSimples";
                titulo += " - <b>Resultado de Busca por: " + Request.QueryString["texto"].ToString() + "</b>";
            }
            else if (Request.QueryString["TipoBusca"] != null && Request.QueryString["TipoBusca"].ToString() != string.Empty)
            {
                if (Request.QueryString["TipoBusca"].ToString() == "lnkResponsabilidade")
                {
                    titulo += " - <b>MINHA RESPONSABILIDADE</b>";
                }
                else if (Request.QueryString["TipoBusca"].ToString() == "lnkReceber")
                {
                    titulo += " - <b>A RECEBER</b>";
                }
                else if (Request.QueryString["TipoBusca"].ToString() == "BuscaAvancada")
                    titulo += " - <b>BUSCA AVANÇADA</b>";
                else if (Request.QueryString["TipoBusca"].ToString() == "lnkAcompanho")
                {
                    titulo += " - <b>QUE ACOMPANHO</b>";
                }
            }
        }
        else if (titulo.Contains("FILAMANUTENCAOCORRETIVA"))
        {
            titulo = "FILA MANUTENÇÃO CORRETIVA / EVOLUTIVA (COMPLEXIDADE BAIXA)";
        }
        else if (titulo.Contains("MURAL"))
        {
            titulo = "MURAL DE RECADOS";
        }
        else if (titulo.Contains("AVALIACAO"))
        {
            if (titulo.Contains("CAD"))
                titulo = "CADASTRO DE PESQUISA DE AVALIAÇÃO";
            else
                titulo = "PESQUISA DE AVALIAÇÃO";
        }
        else if (titulo.Contains("FOLHAPONTO"))
        {
            titulo = "REGISTRO DE ACEITES ASSINADOS DA FOLHA DE PONTO";
        }
        else if (titulo.Contains("DEPURACAO"))
        {
            titulo = "CONFIGURAÇÃO DE PRIORIDADES - DEPURAÇÕES, IMPORTAÇÕES/IMPLANTAÇÃO/INFRA E SCRIPTS";
        }
        else if (titulo.Contains("FILADESENV"))
        {
            if (Request.QueryString["suporte"] != null)
                titulo = "FILA DE SUPORTE";
            else if (Request.QueryString["cqs"] != null)
                titulo = "FILA DE QA'S";
            else if (Request.QueryString["cat"] != null)
                titulo = "FILA DE ATENDIMENTO";
            else if (Request.QueryString["copet"] != null)
                titulo = "Fila DO NET";
            else if (Request.QueryString["coterc"] != null)
                titulo = "Fila COTERC";
            else if (Request.QueryString["coac"] != null)
                titulo = "FILA DE ATENDIMENTO COAC";
            else
                titulo = "FILA DE DESENVOLVIMENTO";
            liChamados.Attributes.Add("class", "opened");
        }
        else if (titulo.Contains("RELATORIOS"))
        {
            titulo = "RELATÓRIOS DE CHAMADOS";
            liChamados.Attributes.Add("class", "opened");
        }
        else if (titulo.Contains("PAINEL"))
        {
            liPainel.Attributes.Add("class", "opened");
            titulo = "Painel Suporte / " + titulo.Replace("Painel", "");
        }
        else if (titulo.Contains("CLIENTE") || titulo.Contains("CONTRATO"))
        {
            liClientes.Attributes.Add("class", "opened");
            liComercial.Attributes.Add("class", "opened");

            if (titulo.Contains("CLIENTE"))
                titulo = "Gestão / Comercial / Clientes";
            else
                titulo = "Gestão / Comercial / Contratos";
        }
        else if (titulo.Contains("COLABORADOR") || titulo.Contains("CURRICULO") || titulo.Contains("FERIAS") || titulo.Contains("HOLERITE"))
        {
            liClientes.Attributes.Add("class", "opened");
            liApoio.Attributes.Add("class", "opened");

            if (titulo.Contains("FERIAS"))
                titulo = "Gestão / Apoio / Férias e Licenças";
            else
                titulo = "Gestão / Apoio / Colaboradores";
        }
        else if (titulo.Contains("VIAGE") || titulo.Contains("DETALHES") || titulo.Contains("RELATOS") || titulo.Contains("RPP"))
        {
            liClientes.Attributes.Add("class", "opened");

            if (Request.QueryString["tipo"] == null)
                liOperacional.Attributes.Add("class", "opened");
            else
                liApoio.Attributes.Add("class", "opened");

            if ((titulo.Contains("VIAGE") || titulo.Contains("DETALHES")) && Request.QueryString["tipo"] == null)
                titulo = "Gestão / Operacional / Viagens";
            else if (Request.QueryString["tipo"] != null)
                titulo = "Gestão / Apoio / Férias e Licenças";
            else if (titulo.Contains("RPP"))
                titulo = "Gestão / Operacional / Relatório RPP";
            else
                titulo = "Gestão / Operacional / Relato Técnico";
        }
        else if (titulo.Contains("BACKLOG"))
        {
            titulo = "BackLog";
        }
        else if (titulo.Contains("PESQUISA"))
        {
            titulo = "Pesquisa de Satisfação";
        }
        else if (titulo.ToUpper().Contains("WORKFLOW") || titulo.ToUpper() == "CI")
        {
            double numero = 0;
            if (titulo.ToUpper() == "CI")
                titulo = "WORKFLOW - NOVA CI";
            else if (Request.QueryString["TIPO"] != null &&
                     double.TryParse(Request.QueryString["TIPO"].ToString(), out numero) == false)
            {
                titulo = "WORKFLOW (CI) > " + Request.QueryString["TIPO"].ToString().ToUpper();

                if (Request.QueryString["CODIGO"] != null)
                {
                    ltSubTitulo.Text = " > CÓDIGO: <b style='color:#000'>" + Request.QueryString["CODIGO"].ToString() +
                                       "</b>";
                }
            }
            else if (Request.QueryString["nometipo"] != null &&
                     double.TryParse(Request.QueryString["nometipo"].ToString(), out numero) == false)
            {
                titulo = "WORKFLOW (CI) > <b style='color:#000'>" + Request.QueryString["nometipo"].ToString().ToUpper() +
                         "</b>";
            }
            else
                titulo = "WORKFLOW (CI)";

            liWorkFlow.Attributes.Add("class", "opened");

        }
        else if (Request.Url.AbsoluteUri.ToLower().Contains("versoes") || Request.Url.AbsoluteUri.ToLower().Contains("sprint") || Request.Url.AbsoluteUri.Contains("VersaoDownload"))
        {
            titulo = "Controle de Versões";

            lIControle.Attributes.Add("class", "opened");

            if (Request.QueryString["pagina"] != null && Request.QueryString["pagina"].ToString() != string.Empty)
            {
                if (Request.QueryString["download"] != null &&
                    Request.QueryString["download"].ToString() != string.Empty)
                {
                    liDownload.Attributes.Add("class", "opened");

                    titulo += " / DOWNLOADS ";

                    if (Request.QueryString["pagina"].ToString().ToUpper().IndexOf("SISPREVWEB") > -1)
                    {
                        liDownloadSisprevWeb.Attributes.Add("class", "opened");

                        titulo += " / PROJETO " +
                                  Request.QueryString["pagina"].ToString().ToUpper().Replace("SISPREVWEB", "");

                        if (Request.QueryString["pagina"].ToString().ToUpper().Replace("SISPREVWEB", "") == string.Empty)
                            titulo += "MUNICÍPIO";
                    }
                    else if (Request.QueryString["pagina"].ToString().ToUpper().IndexOf("SISPREVINTEGRA") > -1)
                    {
                        liDownloadSisprevWebIntegra.Attributes.Add("class", "opened");
                        titulo += " / " + Request.QueryString["pagina"].ToString().ToUpper();
                    }
                }
                else
                {
                    liDistribuicao.Attributes.Add("class", "opened");
                }

                if (Request.QueryString["pagina"].ToString() == "distribuicao")
                    titulo += " / DISTRIBUIÇÃO / SISPREVWEB";
                else if (Request.QueryString["pagina"].ToString() == "distribuicao_contabil")
                    titulo += " / DISTRIBUIÇÃO / SISPREV CONTÁBIL/INTREGRA";
            }
        }
        else if (titulo.Contains("DOCUMENTOS"))
        {
            titulo += " / DOCUMENTOS ";
        }
        else if (titulo.Contains("MULTIMIDIAVIDEOS"))
        {
            titulo = "MULTIMIDIA - VÍDEOS ";
        }
        else if (titulo.Contains("SPRINTS"))
        {
            titulo = "CONTROLE DE VERSÕES / SPRINTS";
        }
        else if (titulo.Contains("CADCATEGORIAS.ASPX"))
        {
            liConfig.Attributes.Add("class", "opened");
            titulo = "CONFIGURAÇÕES";
        }
        else if (Request.Url.AbsoluteUri.Contains("DownloadsArquivosAuxiliares"))
        {
            lIControle.Attributes.Add("class", "opened");
            liDownload.Attributes.Add("class", "opened");
            titulo = "Arquivos Auxiliares";
        }
        else if (Request.Url.AbsoluteUri.Contains("Treinamentos"))
        {
            liClientes.Attributes.Add("class", "opened");
            liGestaoTreinamentos.Attributes.Add("class", "opened");

            if (Request.Url.AbsoluteUri.Contains("TreinamentosCad"))
                titulo = "GESTÃO / GESTÃO DE TREINAMENTOS / CADASTRO";
            else if (Request.Url.AbsoluteUri.Contains("Agendamentos"))
                titulo = "GESTÃO / GESTÃO DE TREINAMENTOS / AGENDAMENTOS";
            else if (Request.Url.AbsoluteUri.Contains("Avaliacoes"))
                titulo = "GESTÃO / GESTÃO DE TREINAMENTOS / AVALIAÇÕES";
            else
                titulo = "GESTÃO / GESTÃO DE TREINAMENTOS";
        }
        else if (Request.Url.AbsoluteUri.Contains("EnviosAplic"))
        {
            liClientes.Attributes.Add("class", "opened");
            liOperacional.Attributes.Add("class", "opened");
            liControleEnviosAplic.Attributes.Add("class", "opened");

            if (Request.Url.AbsoluteUri.Contains("EnviosAplicCad"))
                titulo = "GESTÃO / OPERACIONAL / CONTROLE DE ENVIOS APLIC / CADASTRO";
            else
                titulo = "GESTÃO / OPERACIONAL / CONTROLE DE ENVIOS APLIC";
        }
        else if (Request.Url.AbsoluteUri.Contains("BasesConhecimento"))
        {
            titulo = "Bases de Conhecimento";
        }
        else if (Request.Url.AbsoluteUri.Contains("PlanilhaCusto"))
        {
            titulo = "PLANILHA DE CUSTO";
        }

        divTitulo.Visible = Request.ServerVariables["PATH_INFO"].ToString().ToUpper().Contains("INICIAL") == false;

        if (divTitulo.Visible)
        {
            ltTitulo.Text = titulo.ToUpper().Replace("NOVAEXTRANET", "");
        }
    }

    public string getQtdeChamados(string tipo)
    {
        string Where = " where 1=1";

        if (tipo == "lnkResponsabilidade")
        {
            Where += " and status not in (2,7) and codigo_criador=" + DadosUsuarioLogado.COLABORADOR_ID;
        }
        else if (tipo == "lnkReceber")
        {
            Where += " and status not in (7)  and codigo in (SELECT distinct codigo_helpdesk from helpdesk_tramite WHERE (DATA_RECEBIMENTO IS NULL OR DATA_RECEBIMENTO = '') AND LOGIN=" +
                   DadosUsuarioLogado.COLABORADOR_ID + ") ";
        }
        else if (tipo == "lnkAcompanho")
        {
            Where += " and status not in (2,7) and /*(codigo in (SELECT codigo_helpdesk FROM helpdesk_tramite WHERE COALESCE(DATA_RECEBIMENTO,'') = '' AND LOGIN=" +
                     DadosUsuarioLogado.COLABORADOR_ID + ") or codigo_usuario = " + DadosUsuarioLogado.CODIGO +
                     " or*/ codigo in (select codigo_helpdesk from helpdesk_acompanhamento where login = '" + DadosUsuarioLogado.COLABORADOR_ID + "') /*)*/";
        }

        string n = " <b>(" + Utilitarios.Exec_StringSql_Return("SELECT COUNT(*) FROM HELPDESK " + Where) + ")</b>";

        return n;
    }

    public void MudarLabel(string titulo)
    {
        ltTitulo.Text = titulo;
    }

    private void Notificacoes()
    {
        // Receber
        string sql = @"select distinct  h.codigo, h.titulo, '' as data_tramite, h.categoria, h.prioridade from helpdesk_tramite ht
            cross apply (
	            SELECT distinct  h.codigo, h.titulo, '' as data_tramite, h.categoria, h.prioridade FROM VW_CHAMADOS_GERAL h 
	            where h.codigo = ht.codigo_helpdesk and h.status not in (2,7)
            ) h
            where ht.LOGIN = " + DadosUsuarioLogado.COLABORADOR_ID + " and ht.DATA_RECEBIMENTO is null";

        DataTable dt = Utilitarios.Pesquisar(sql);

        ltQtdeRec.Text = dt.Rows.Count.ToString();

        int qtde = 0;

        for (int i = 0; i < dt.Rows.Count; i++)
        {
            if (dt.Rows[i]["data_tramite"].ToString() != string.Empty && dt.Rows[i]["data_tramite"] != DBNull.Value)
            {
                if (Convert.ToDateTime(dt.Rows[i]["data_tramite"]).ToShortDateString() == DateTime.Now.ToShortDateString())
                    qtde++;
            }

        }

        ltQtdeRecHoje.Text = qtde.ToString();

        rptChamadosRec.DataSource = dt;
        rptChamadosRec.DataBind();

        if (dt.Rows.Count == 0)
        {
            LinkRec.Attributes.Remove("class");
            LinkRec.Attributes.Remove("data-toggle");
            LinkRec.Attributes.Remove("data-hover");
            LinkRec.Attributes.Remove("data-close-others");
        }

        // CI

        sql = "set dateformat dmy SELECT processos.data, relacionamento.codigo as codrelac, processos.codigo, processos.titulo, relacionamento.de, " +
            "processos.criador, C.NOME AS NOME_DE, C2.NOME AS NOME_CRIADOR, SE.NOME AS NOME_SETOR  FROM processos INNER JOIN relacionamento ON " +
            "processos.codigo = relacionamento.codigo_processo LEFT JOIN EXTRANET.COLABORADOR C ON C.COLABORADOR_ID = relacionamento.de LEFT JOIN " +
            "EXTRANET.COLABORADOR C2 ON C2.COLABORADOR_ID = processos.criador INNER JOIN EXTRANET.SETOR SE ON SE.SETOR_ID = C2.SETOR_ID WHERE ((relacionamento.recebido=0) " +
            "and (relacionamento.status<>-1 and relacionamento.status<>1 and relacionamento.status<>3) AND (relacionamento.para='148' or relacionamento.para='19' or relacionamento.para=0)) " +
            "AND processos.data <= '13/02/2008' union SELECT processos.data, relacionamento.codigo as codrelac, processos.codigo, processos.titulo, relacionamento.de, processos.criador, " +
            "C.NOME AS NOME_DE, C2.NOME AS NOME_CRIADOR, SE.NOME AS NOME_SETOR FROM processos INNER JOIN relacionamento ON processos.codigo = relacionamento.codigo_processo " +
            "LEFT JOIN EXTRANET.COLABORADOR C ON C.COLABORADOR_ID = relacionamento.de LEFT JOIN EXTRANET.COLABORADOR C2 ON C2.COLABORADOR_ID = processos.criador " +
            "INNER JOIN EXTRANET.SETOR SE ON SE.SETOR_ID = C2.SETOR_ID WHERE ((relacionamento.recebido=0) and (relacionamento.status<>-1 and relacionamento.status<>1 and " +
            "relacionamento.status<>3) AND (relacionamento.para='" + DadosUsuarioLogado.COLABORADOR_ID + "' ))";

        string sql_aux = " OR relacionamento.para = 297";

        if (DadosUsuarioLogado.COLABORADOR_ID == 164)
        {
            sql = "SELECT   relacionamento.codigo as codrelac,   processos.codigo, processos.titulo, relacionamento.de, processos.criador, C.NOME AS NOME_DE, C2.NOME AS NOME_CRIADOR, " +
                "SE.NOME AS NOME_SETOR     FROM processos      INNER JOIN relacionamento ON processos.codigo = relacionamento.codigo_processo  " +
                "LEFT JOIN EXTRANET.COLABORADOR C ON C.COLABORADOR_ID = relacionamento.de LEFT JOIN EXTRANET.COLABORADOR C2 ON C2.COLABORADOR_ID = processos.criador " +
                "INNER JOIN EXTRANET.SETOR SE ON SE.SETOR_ID = C2.SETOR_ID WHERE relacionamento.recebido=0 AND relacionamento.status<>-1 AND relacionamento.status<>1 AND " +
                "relacionamento.status<>3  AND (relacionamento.para=" + DadosUsuarioLogado.COLABORADOR_ID + " OR relacionamento.para=0 " + sql_aux + ")  " +
                "AND C.NOME NOT LIKE '%anonimo%' AND C2.NOME NOT LIKE '%anonimo%'";
        }

        dt = Utilitarios.Pesquisar(sql);
        ltCIRec.Text = dt.Rows.Count.ToString();
        rptCIRec.DataSource = dt;
        rptCIRec.DataBind();

        if (dt.Rows.Count == 0)
        {
            linkCI.Attributes.Remove("class");
            linkCI.Attributes.Remove("data-toggle");
            linkCI.Attributes.Remove("data-hover");
            linkCI.Attributes.Remove("data-close-others");
        }

        //JUSTIFICATIVAS - THIAGO ARAÚJO
        /*if (DadosUsuarioLogado.CARGO_ID == 5 || DadosUsuarioLogado.CARGO_ID == 2 || DadosUsuarioLogado.CARGO_ID == 3 || DadosUsuarioLogado.CARGO_ID == 4 || DadosUsuarioLogado.CARGO_ID == 24 || DadosUsuarioLogado.COLABORADOR_ID == 393)
        {
            liJusti.Visible = true;
            calculaJusti();
        }
        else
        {
            liJusti.Visible = false;
        }*/

    }

    protected void rptChamadosRec_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
            HiddenField hddData = (HiddenField)e.Item.FindControl("hddData");
            HiddenField hddCodigo = (HiddenField)e.Item.FindControl("hddCodigo");
            HiddenField hddTitulo = (HiddenField)e.Item.FindControl("hddTitulo");
            HiddenField hddCategoria = (HiddenField)e.Item.FindControl("hddCategoria");
            HiddenField hddPerioridade = (HiddenField)e.Item.FindControl("hddPerioridade");
            Literal ltLink = (Literal)e.Item.FindControl("ltLink");

            string prioridade = "FFF";
            string dsc_prioridade = "Baixa";

            if (hddPerioridade.Value == "3")
            {
                prioridade = "FFFF00";
                dsc_prioridade = "Urgente";
            }
            else if (hddPerioridade.Value == "2")
            {
                prioridade = "FEF8C4";
                dsc_prioridade = "Alta";
            }

            string link = "<li class='notification-info' title='Prioridade " + dsc_prioridade + "'><a href=\"Chamado.aspx?CODIGO=" + hddCodigo.Value +
                "\"><i class='pull-right' style=\"border:1px solid #CCC;background-color:#" + prioridade + "\"></i><span style='color:#000'>" + hddTitulo.Value + "</span><br>" + hddCategoria.Value + "</a></li>";

            /*if (hddData.Value != "")
            {
                if (Convert.ToDateTime(hddData.Value).ToShortDateString() == DateTime.Now.ToShortDateString())
                    link = "<b>" + link + "</b>";
            }*/

            ltLink.Text = link;
        }
    }

    protected void btnVersaoAnterior_Click(object sender, EventArgs e)
    {


    }

    protected void btnCardapio_Click(object sender, EventArgs e)
    {
        string horario = DateTime.Now.ToString("HH:00").Replace(":", "");
        if (Convert.ToInt32(horario) >= Convert.ToInt32("1145"))
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "msg", "<script>alert('Horário não permitido para acesso ao Cardápio!')</script>", false);
        }
        else
        {
            DataTable dt = Utilitarios.Pesquisar("SELECT DATA FROM EXTRANET.CARDAPIO WHERE DATA = CONVERT(VARCHAR(12),GETDATE())");

            if (dt.Rows.Count != 0)
            {

                string JANELA = "<script>function Janela(){" +
                                "LeftPosition = (screen.width) ? (screen.width-770)/2 : 0;" +
                                "TopPosition = (screen.height) ? (screen.height-600)/2 : 0;" +
                                "window.open('Cardapio/Confirmacao.aspx','relniver','top='+TopPosition+'," +
                                "left='+LeftPosition+',status=yes,scrollbars=yes,resizable=0,width='+((770)-1)+',height='+(600-1));" +
                                "return false; } Janela(); </script>";

                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "msg", JANELA, false);
            }
            else
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "msg", "<script>alert('Não foi cadastrado o cardápio do dia até o momento!')</script>", false);
            }
        }
    }

    protected void btnCriaCardapio_Click(object sender, EventArgs e)
    {
        /*string JANELA = "<script>function Janela(){" +
                            "LeftPosition = (screen.width) ? (screen.width-765)/2 : 0;" +
                            "TopPosition = (screen.height) ? (screen.height-620)/2 : 0;" +
                            "window.open('ConfirmarCardapio.aspx','relniver','top='+TopPosition+'," +
                            "left='+LeftPosition+',status=yes,scrollbars=yes,resizable=0,width='+((765)-1)+',height='+(590-1));" +
                            "return false; } Janela(); </script>";

        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "msg", JANELA, false);*/
        Response.Redirect("ConfirmarCardapio.aspx");
    }

    protected void RelatorioEstrategico_Click(object sender, EventArgs e)
    {
        Response.Redirect("RelatorioEstrategico.aspx");
    }

    public bool ValidaNumero(string numero)
    {
        Regex rx = new Regex(@"^\d+$");
        return rx.IsMatch(numero);
    }

    //FUNÇÃO DA JUSTIFICATIVA - THIAGO ARAÚJO 2015 \\
    protected void btnEnviar_Click(object sender, EventArgs e)
    {
        //Verifica infos
        Boolean periodoMat = false, periodoVesp = false;
        if (txtHoraMat.Text != "" || txtHoraMatSai.Text != "")
            periodoMat = true;

        if (txtHoraVesp.Text != "" || txtHoraVespSai.Text == "")
            periodoVesp = true;

        string sql = string.Empty;
        string arquivo = string.Empty;
        string endereco = string.Empty;
        string nome_usuario = DadosUsuarioLogado.NOME_COLABORADOR;
        int id_usuario = DadosUsuarioLogado.COLABORADOR_ID;
        int setor = DadosUsuarioLogado.SETOR_ID;
        string motivo = txtJustificativa.Text.Replace("'", "");
        string periodo;

        if (periodoMat == true && periodoVesp == true)
            periodo = "Matutino e Vespertino";
        if (periodoMat == true)
            periodo = "Matutino";
        else
            periodo = "Vespertino";

        //imagem
        if (arquivoJustificativa.PostedFile.FileName != string.Empty)
        {
            string ano = DateTime.Now.Year.ToString();
            string mes = DateTime.Now.Month.ToString();
            DirectoryInfo di = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory.Replace("novaextranet", "") + @"agendaassessoria\\ArquivosAtestados\\" + ano + "");
            DirectoryInfo di1 = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory.Replace("novaextranet", "") + @"agendaassessoria\\ArquivosAtestados\\" + ano + "\\" + mes);

            try
            {
                if (di.Exists == false)
                {
                    System.IO.Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory.Replace("novaextranet", "") + @"agendaassessoria\\ArquivosAtestados\\" + ano + "");
                    System.IO.Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory.Replace("novaextranet", "") + @"agendaassessoria\\ArquivosAtestados\\" + ano);
                }
                else if (di1.Exists == false)
                    System.IO.Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory.Replace("novaextranet", "") + @"agendaassessoria\\ArquivosAtestados\\" + ano);
            }
            catch (Exception)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "msg", "<script>alert('Ocorreu uma falha ao enviar o atestado!')</script>", false);
            }

            endereco = "ArquivosAtestados/" + ano;
            arquivo = "AnexoJustificativa" + hddImagem.Value + DateTime.Now.Ticks.ToString() + Path.GetExtension(arquivoJustificativa.PostedFile.FileName).ToLower();
            arquivoJustificativa.SaveAs(AppDomain.CurrentDomain.BaseDirectory.Replace("novaextranet", "") + @"agendaassessoria\ArquivosAtestados\\" + ano + "\\" + arquivo);
        }

        if (motivo.Length > 2 && txtDataJusti.Text != "")
        {
            try
            {
                sql = "INSERT INTO JUSTIFICATIVAS (NOME, ID_COLABORADOR, MOTIVO, ARQUIVO, PERIODO, ENT_MAT, SAI_MAT, ENT_VES, SAI_VES, DATA, AUTORIZADO, ABONADO, ATIVO, END_IMAGEM, SETOR_ID, TIPO) VALUES " +
                "('" + nome_usuario + "', " + id_usuario + ", '" + motivo + "', '" + arquivo + "', '" + periodo + "', '" + txtHoraMat.Text + "', '" + txtHoraMatSai.Text + "', " +
                "'" + txtHoraVesp.Text + "', '" + txtHoraVespSai.Text + "', '" + txtDataJusti.Text + "', 'N', 'N', 'S', '" + endereco + "', " + setor + ", '" + RadioTipo.SelectedValue + "')";
                Utilitarios.Exec_StringSql(sql);
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "msg", "<script>alert('A justificativa foi enviada ao seu coordenador!')</script>", false);
            }
            catch (Exception)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "msg", "<script>alert('A justificativa não foi enviada pois uma falha ocorreu!')</script>", false);
            }
            finally
            {
                //zera campos
                txtDataJusti.Text = "";
                txtHoraMat.Text = "";
                txtHoraMatSai.Text = "";
                txtHoraVesp.Text = "";
                txtHoraVespSai.Text = "";
                txtJustificativa.Text = "";
                RadioTipo.Text = "";
            }

        }
        else
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "msg", "<script>alert('O motivo e a data não podem ficar em branco!')</script>", false);
        }
    }
    //conta as justificativas
    protected void calculaJusti()
    {
        string sql = "SELECT * FROM JUSTIFICATIVAS WHERE SETOR_ID = " + DadosUsuarioLogado.SETOR_ID + " AND (AUTORIZADO = 'N' AND ABONADO = 'N') ORDER BY ID_JUSTIFICATIVAS DESC";
        DataTable dt = Utilitarios.Pesquisar(sql);

        ltJusti.Text = dt.Rows.Count.ToString();
        rptJustificativas.DataSource = dt;
        rptJustificativas.DataBind();
    }

    protected void btnAlterarDados_Click(object sender, EventArgs e)
    {
        if (FileUpload1.PostedFile.FileName == string.Empty)
        {
            Utilitarios.Exec_StringSql(@"update extranet.colaborador set ramal = '" + txtRamal.Text + "', " +
                "nome_ass_email = '" + txtNomeAssEmail.Text.Replace("'", "") + "', cargo_ass_email = '" + txtCargoAssEmail.Text.Replace("'", "") + "', " +
                "setor_ass_email = '" + txtSetorAssEmail.Text.Replace("'", "") + "', biografia='" + txtBiografia.InnerText.Replace("'", "") + "' " +
                "where colaborador_id=" + DadosUsuarioLogado.COLABORADOR_ID);
        }
        else
        {
            string nome = DateTime.Now.Ticks.ToString() + Path.GetExtension(FileUpload1.PostedFile.FileName).ToLower();
            string pathToSave = HttpContext.Current.Server.MapPath("~/Library/Colaborador_Foto/") + nome;

            FileUpload1.SaveAs(pathToSave + ".tmp");
            Utilitarios.Resize(pathToSave + ".tmp", pathToSave + ".tmp", 500, 500);

            Utilitarios.Exec_StringSql("update extranet.colaborador set foto='" + nome + "', ramal = '" + txtRamal.Text + "', " +
                "nome_ass_email = '" + txtNomeAssEmail.Text.Replace("'", "") + "', cargo_ass_email = '" + txtCargoAssEmail.Text.Replace("'", "") + "', " +
                "setor_ass_email = '" + txtSetorAssEmail.Text.Replace("'", "") + "', biografia='" + txtBiografia.InnerText.Replace("'", "") + "' " +
                "where colaborador_id=" + DadosUsuarioLogado.COLABORADOR_ID);

            DadosUsuarioLogado.FOTO = "Library/Colaborador_Foto/" + nome;
        }

        var dtCurriculo = Utilitarios.Pesquisar("SELECT CURRICULO_ID FROM EXTRANET.CURRICULO WHERE COLABORADOR_ID = " + DadosUsuarioLogado.COLABORADOR_ID);

        if (dtCurriculo.Rows.Count > 0)
            Utilitarios.Exec_StringSql("UPDATE EXTRANET.CURRICULO SET SEXO = '" + ddlSexo.SelectedValue + "' WHERE CURRICULO_ID=" + dtCurriculo.Rows[0]["CURRICULO_ID"]);
        else
        {
            Utilitarios.Exec_StringSql("INSERT INTO EXTRANET.CURRICULO(COLABORADOR_ID, NOME, SEXO, DATA_NASC, CPF) " +
                "SELECT COLABORADOR_ID, NOME, '" + ddlSexo.SelectedValue + "', '', '' FROM EXTRANET.COLABORADOR WHERE COLABORADOR_ID = " + DadosUsuarioLogado.COLABORADOR_ID);
        }

        //DadosUsuarioLogado.NOME_COLABORADOR = txtNome.Text;
        DadosUsuarioLogado.BIOGRAFIA = txtBiografia.InnerText;
        DadosUsuarioLogado.SEXO = ddlSexo.SelectedValue;

        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "msg", "<script>alert('Alterações efetuadas com sucesso!')</script>", false);
    }

    protected void rptCol_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
            Literal ltFoto = (Literal)e.Item.FindControl("ltFoto");
            Literal ltChat = (Literal)e.Item.FindControl("ltChat");
            Literal ltFotoMaior = (Literal)e.Item.FindControl("ltFotoMaior");
            HiddenField hddColaborador = (HiddenField)e.Item.FindControl("hddColaborador");

            //Se não tem texto de busca, é carregamento inicial, adiciona lazy loading
            if (string.IsNullOrWhiteSpace(txtProcColab.Text))
            {
                string semfoto = "<div style='float:left;margin-right:10px;background-image:url(\"Library/Images/usuario_semfoto.jpg\");" +
                    "width: 40px; height: 40px; background-size: 40px; background-position: top; background-position: center; border: 2px solid #CCC' class=\"img-circle\"></div>";

                if (ltFoto.Text != "")
                {
                    ltFoto.Text = "<div data-src=\"Library/Colaborador_Foto/" + ltFoto.Text + "\" title=\"Clique para ampliar\" style='float:left;margin-right:10px;cursor:pointer;" +
                        "width: 40px; height: 40px; background-size: 40px; background-position: top; background-position: center; border: 2px solid #CCC' class=\"lazy img-circle amplia-foto-chat\"></div>";
                    ltFotoMaior.Text = "<img class='img-responsive lazy' data-src=\"Library/Colaborador_Foto/" + ltFotoMaior.Text + "\"/>";
                }
                else
                    ltFoto.Text = semfoto;
            }
            else
            {
                string semfoto = "<div style='float:left;margin-right:10px;background-image:url(\"Library/Images/usuario_semfoto.jpg\");" +
                    "width: 40px; height: 40px; background-size: 40px; background-position: top; background-position: center; border: 2px solid #CCC' class=\"img-circle\"></div>";

                if (ltFoto.Text != "")
                {
                    ltFoto.Text = "<div title=\"Clique para ampliar\" style='float:left;margin-right:10px;cursor:pointer;background-image:url(\"Library/Colaborador_Foto/" + ltFoto.Text + "\");" +
                        "width: 40px; height: 40px; background-size: 40px; background-position: top; background-position: center; border: 2px solid #CCC' class=\"lazy img-circle amplia-foto-chat\"></div>";
                    ltFotoMaior.Text = "<img class='img-responsive lazy' src=\"Library/Colaborador_Foto/" + ltFotoMaior.Text + "\"/>";
                }
                else
                    ltFoto.Text = semfoto;
            }

            //ltChat.Text = "<a class='ChamarChat' id=\"lnkChat" + hddColaborador.Value + "\" data-userid=\"" + hddColaborador.Value + "\" href=\"#\" style='color:#fff'>" +
            //    "<img id=\"imgChat" + hddColaborador.Value + "\" Width=\"18px\" Height=\"18px\" src=\"Images/offline.png\" /> Bater Papo</a>";
        }
    }

    protected void btnProcCol_ServerClick(object sender, EventArgs e)
    {
        string ativo = chkRptSetorInativos.Checked ? "" : " AND ATIVO='S'";
        /*DataTable dt = Utilitarios.Pesquisar("SELECT SETOR_ID, NOME FROM EXTRANET.SETOR WHERE SETOR_ID IN " +
            "(SELECT SETOR_ID FROM EXTRANET.COLABORADOR WHERE 1=1 " + ativo + " AND NOME LIKE '%" + txtProcColab.Text + "%') ORDER BY NOME");*/
        DataTable dt = Utilitarios.Pesquisar("SELECT ES.SETOR_ID, ES.NOME FROM EXTRANET.SETOR ES JOIN EXTRANET.COLABORADOR EC ON ES.SETOR_ID = EC.SETOR_ID WHERE (EC.NOME LIKE '%" + txtProcColab.Text + "%' OR ES.NOME LIKE '%" + txtProcColab.Text + "%') " + ativo + " GROUP BY ES.NOME, ES.SETOR_ID ORDER BY ES.NOME");
        rptSetor.DataSource = dt;
        rptSetor.DataBind();
    }

    //FEITO POR THIAGO ARAÚJO - MARÇO 2016
    protected void btnAlterarSenha_Click(object sender, EventArgs e)
    {
        string sql = string.Empty;
        //verifica a senha atual
        string senhaDigitada = Utilitarios.Criptografar(txtSenhaAtual.Text).ToUpper();
        string senhaNova = Utilitarios.Criptografar(txtNovaSenha.Text).ToUpper();
        string senhaNovamente = Utilitarios.Criptografar(txtRepeteSenha.Text).ToUpper();
        string senhaCadastrada = Utilitarios.Exec_StringSql_Return("SELECT SENHATRM FROM ASSESSORES WHERE LOGIN = " + DadosUsuarioLogado.COLABORADOR_ID);

        if (senhaDigitada == "")
            ScriptManager.RegisterStartupScript(this, GetType(), "", "alert('O campo da senha atual não deve ficar em branco!'); $('#modalWorkflow').modal('show');", true);
        else
        {
            if (senhaCadastrada != senhaDigitada)
                ScriptManager.RegisterStartupScript
                    (this, GetType(), "",
                    "alert('Senha atual não confere com senha cadastrada!'); " +
                    "$('#modalWorkflow').modal('show'); $('#txtSenhaAtual').focus();", true);
            else
            {
                if (senhaNova == "" || senhaNovamente == "")
                    ScriptManager.RegisterStartupScript(this, GetType(), "", "alert('Nenhum campo pode ficar em branco!'); $('#modalWorkflow').modal('show');", true);
                else
                {
                    if (senhaNova == senhaNovamente)
                    {
                        sql = "UPDATE ASSESSORES SET SENHATRM = '" + senhaNovamente + "' WHERE LOGIN = " + DadosUsuarioLogado.COLABORADOR_ID;
                        Utilitarios.Exec_StringSql(sql);
                        ScriptManager.RegisterStartupScript(this, GetType(), "", "alert('Senha atualizada com sucesso!');", true);
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript
                        (this, GetType(), "",
                        "alert('Senhas digitadas não conferem!'); " +
                        "$('#modalWorkflow').modal('show'); $('#txtNovaSenha').focus();", true);
                    }
                }
            }
        }
    }

    protected void btnOCulto2_Click(object sender, EventArgs e)
    {
        if (txtBusca2.Text.Trim() != string.Empty && ValidaNumero(txtBusca2.Text.Trim()))
        {
            int existe = Convert.ToInt32(Utilitarios.Exec_StringSql_Return("SELECT COUNT(*) FROM HELPDESK WHERE CODIGO=" + txtBusca2.Text.Trim()));
            if (existe > 0)
                Response.Redirect("Chamado.aspx?CODIGO=" + txtBusca2.Text.Trim());
            else
                ScriptManager.RegisterClientScriptBlock(this, GetType(), "msg", "<script>alert('Número do chamado não existe!');</script>", false);
        }
        else if (txtBusca2.Text.Trim() != string.Empty && !ValidaNumero(txtBusca2.Text.Trim()))
            Response.Redirect("Chamados.aspx?TipoBusca=BuscaAvancada&texto=" + txtBusca2.Text.Trim());
        else
            ScriptManager.RegisterClientScriptBlock(this, GetType(), "msg", "<script>alert('Digite o número do chamado para buscar!');</script>", false);
    }

    protected void btnConfirma_Click(object sender, EventArgs e)
    {
        string senha_informada = Utilitarios.Criptografar(txtSenhaConfirma.Text).ToUpper();
        string senhaCadastrada = Utilitarios.Exec_StringSql_Return("SELECT SENHATRM FROM ASSESSORES WHERE LOGIN = " + DadosUsuarioLogado.COLABORADOR_ID + " AND usuario_tipo = 0");
        string sql = string.Empty;

        if (senha_informada.ToLower().Equals(senhaCadastrada.ToLower()))
        {
            if (DadosUsuarioLogado.SENHA_TRAMITE == "S")
            {
                sql = "UPDATE EXTRANET.COLABORADOR SET SENHA_TRAMITE = 'N' WHERE COLABORADOR_ID = " + DadosUsuarioLogado.COLABORADOR_ID;
                DadosUsuarioLogado.SENHA_TRAMITE = "N";
            }
            else
            {
                sql = "UPDATE EXTRANET.COLABORADOR SET SENHA_TRAMITE = 'S' WHERE COLABORADOR_ID = " + DadosUsuarioLogado.COLABORADOR_ID;
                DadosUsuarioLogado.SENHA_TRAMITE = "S";
            }

            Utilitarios.Exec_StringSql(sql);
            ScriptManager.RegisterStartupScript(this, GetType(), "", "$('.modal-backdrop').remove();alert('Alterações realizadas com sucesso');", true);
        }
        else
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "", "$('.modal-backdrop').remove();alert('Senha do workflow incorreta!');", true);
        }
    }

    protected void RptViagens_OnItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        HiddenField HddViagemId = (HiddenField)e.Item.FindControl("HddViagemId");
        HtmlAnchor link = (HtmlAnchor)e.Item.FindControl("LinkViagem");

        link.HRef = "NovaViagem.aspx?id=" + HddViagemId.Value;
    }

    protected void LnkModoMaster_OnClick(object sender, EventArgs e)
    {
        Session.Remove("ACESSO_GARANTIDO");
        Response.Redirect(Request.Url.AbsolutePath);
    }

    protected void btnRegistrarPonto_OnClick(object sender, EventArgs e)
    {
        m_NBioAPI = new NBioAPI();
        m_WinOption = new NBioAPI.Type.WINDOW_OPTION();
        m_WinOption.Option2 = new NBioAPI.Type.WINDOW_OPTION_2();
        m_NBioAPI.SetSkinResource("NBSP2Por.dll");

        SetInitValue();

        NBioAPI.Type.FIR_TEXTENCODE hRegisterdFIR = new NBioAPI.Type.FIR_TEXTENCODE();
        NBioAPI.Type.FIR_TEXTENCODE m_textFIR = new NBioAPI.Type.FIR_TEXTENCODE();
        m_textFIR.TextFIR = hdd_chave.Value;
        uint ret = 0;
        NBioAPI.Type.FIR_PAYLOAD myPayload = new NBioAPI.Type.FIR_PAYLOAD();
        bool result = false;

        DataTable dt = Utilitarios.Pesquisar("SELECT COLABORADOR_ID, HASH_DIGITAL FROM EXTRANET.COLABORADOR WHERE HASH_DIGITAL IS NOT NULL AND ATIVO = 'S'");

        for (int i = 0; i < dt.Rows.Count; i++)
        {
            hRegisterdFIR.TextFIR = dt.Rows[i]["HASH_DIGITAL"].ToString();
            ret = m_NBioAPI.VerifyMatch(m_textFIR, hRegisterdFIR, out result, myPayload);

            if (result)
            {
                RP rp = (hdd_tipo.Value == "1") ? RegistrarPonto(dt.Rows[i]["COLABORADOR_ID"].ToString(), DateTime.Now) : VisualizarPonto(dt.Rows[i]["COLABORADOR_ID"].ToString());

                if (hdd_tipo.Value == "1")
                {
                    ScriptManager.RegisterClientScriptBlock(this, GetType(), "msg", "alert('" + rp.Msg + "');window.location.href='" + Request.Url.AbsoluteUri + "';$('#modalBaterPonto').modal('show');", true);
                }

                lb_nome.Text = rp.Nome;
                lb_entrada_1.Text = rp.Batida1;
                lb_entrada_2.Text = rp.Batida2;
                lb_saida_1.Text = rp.Batida3;
                lb_saida_2.Text = rp.Batida4;

                // break;
            }
        }

        if (!result)
        {
            ScriptManager.RegisterClientScriptBlock(this, GetType(), "msg",
                "alert('Atenção: Nenhum colaborador foi encontrado com essa biometria, tente novamente!');" +
                "$('#modalBaterPonto').modal('show');", true);
        }
    }

    private void SetInitValue()
    {
        m_WinOption.WindowStyle = NBioAPI.Type.WINDOW_STYLE.POPUP;
        m_WinOption.WindowStyle = (uint)NBioAPI.Type.WINDOW_STYLE.NO_WELCOME;

        m_WinOption.CaptureCallBackInfo = new NBioAPI.Type.CALLBACK_INFO_0();
        m_WinOption.CaptureCallBackInfo.CallBackFunction = new NBioAPI.Type.WINDOW_CALLBACK_0(MyCaptureCallback);
    }

    private static uint MyCaptureCallback(ref NBioAPI.Type.WINDOW_CALLBACK_PARAM_0 cbParam0, IntPtr userParam)
    {
        NBioAPI.Type.WINDOW_CALLBACK_PARAM_EX ParamEx = (NBioAPI.Type.WINDOW_CALLBACK_PARAM_EX)System.Runtime.InteropServices.Marshal.PtrToStructure(cbParam0.ParamEx, typeof(NBioAPI.Type.WINDOW_CALLBACK_PARAM_EX));
        qualidade = cbParam0.Quality;

        return 0;
    }

    private bool getVerificaTempoDiferenca(DateTime data_ini, DateTime data_fim)
    {
        return data_fim > data_ini.AddMinutes(10);
    }

    private RP VisualizarPonto(string colaborador_id)
    {
        RP rp = new RP();

        var dtColaborador = Utilitarios.Pesquisar("SELECT NOME FROM EXTRANET.COLABORADOR WHERE COLABORADOR_ID = " + colaborador_id);

        lb_nome.Text = DadosUsuarioLogado.NOME_COLABORADOR;

        if (dtColaborador.Rows.Count > 0)
        {
            var sql = "SET DATEFORMAT DMY; " +
                "SELECT CONVERT(VARCHAR(5),CHEGADA_MATUTINO,108)AS CHEGADA_MATUTINO,CONVERT(VARCHAR(5),SAIDA_MATUTINO,108)AS SAIDA_MATUTINO,CONVERT(VARCHAR(5),CHEGADA_VESPERTINO,108)AS CHEGADA_VESPERTINO,CONVERT(VARCHAR(5),SAIDA_VESPERTINO,108)AS SAIDA_VESPERTINO " +
                "FROM EXTRANET.COLABORADOR_PONTO WHERE COLABORADOR_ID= " + colaborador_id + " AND DATA_PONTO='" + DateTime.Now.ToShortDateString() + "'";

            var dtBatidas = Utilitarios.Pesquisar(sql);


            if (dtBatidas.Rows.Count > 0)
            {
                rp.Batida1 = dtBatidas.Rows[0]["CHEGADA_MATUTINO"].ToString();
                lb_entrada_1.Text = rp.Batida1;

                rp.Batida2 = dtBatidas.Rows[0]["SAIDA_MATUTINO"].ToString();
                lb_entrada_2.Text = rp.Batida2;

                rp.Batida3 = dtBatidas.Rows[0]["CHEGADA_VESPERTINO"].ToString();
                lb_saida_1.Text = rp.Batida3;

                rp.Batida4 = dtBatidas.Rows[0]["SAIDA_VESPERTINO"].ToString();
                lb_saida_2.Text = rp.Batida4;
            }

            rp.Nome = dtColaborador.Rows[0]["NOME"].ToString();
        }

        return rp;
    }

    private RP RegistrarPonto(string colaborador_id, DateTime hora)
    {
        RP rp = new RP();
        DataTable dtColaborador = new DataTable();
        DataTable dtPonto = new DataTable();
        DataTable dtPonto_horas = new DataTable();
        string CAMPO = string.Empty;
        bool continua = true;

        //CHECA IP
        //string ips = new System.Net.WebClient().DownloadString("http://www.agendaassessoria.com.br/ip/").Trim().Replace("<br>", " ");
        //string[] wordss = ips.Split(' ');
        //var ip = wordss[0];

        //var dt = Utilitarios.Pesquisar("SELECT IP FROM EXTRANET.COLABORADOR WHERE COLABORADOR_ID = " + colaborador_id);
        //if (dt != null)
        //{
        //    string[] words = dt.Rows[0]["IP"].ToString().Split(';');
        //    for (int i = 0; i < words.Length; i++)
        //    {
        //        if (words[i] == ip)
        //        {
        //            continua = true;
        //        }
        //    }
        //}

        //coloca fuso horário
        var fuso =
            Utilitarios.Exec_StringSql_Return(
                "SELECT FUSO FROM FUSO_HORARIOS A JOIN EXTRANET.COLABORADOR B ON A.ID_FUSO = B.FUSO_HORARIO WHERE COLABORADOR_ID = " +
                colaborador_id);
        hora = hora.AddHours(!string.IsNullOrEmpty(fuso) ? Convert.ToInt32(fuso) : 0);

        dtColaborador = Utilitarios.Pesquisar("SELECT NOME FROM EXTRANET.COLABORADOR WHERE COLABORADOR_ID = " + colaborador_id);

        if (dtColaborador.Rows.Count > 0 && continua)
        {
            dtPonto = Utilitarios.Pesquisar("SET DATEFORMAT DMY; SELECT * FROM EXTRANET.COLABORADOR_PONTO WHERE COLABORADOR_ID = " + colaborador_id + " AND DATA_PONTO = '" + DateTime.Now.ToShortDateString() + "'");

            //VERIFICA QUANTAS VEZES BATE PONTO
            if (dtPonto.Rows.Count > 0)
            {
                dtPonto_horas = Utilitarios.Pesquisar(
                    "SELECT A.NOME, A.HORARIO_ID, B.CHEGADA_MATUTINO, B.SAIDA_MATUTINO, B.CHEGADA_VESPERTINO, B.SAIDA_VESPERTINO FROM EXTRANET.COLABORADOR A " +
                    "JOIN HORARIO B ON A.HORARIO_ID = B.HORARIO_ID " +
                    "WHERE COLABORADOR_ID = " + colaborador_id);
            }

            if (dtPonto.Rows.Count > 0)
            {
                if (dtPonto_horas.Rows[0]["CHEGADA_VESPERTINO"] != DBNull.Value && dtPonto_horas.Rows[0]["CHEGADA_VESPERTINO"].ToString() == "00:00:00"
                    && dtPonto_horas.Rows[0]["SAIDA_VESPERTINO"].ToString() == "00:00:00")
                {
                    if (dtPonto.Rows[0]["CHEGADA_MATUTINO"] == DBNull.Value)
                        if (hora.Hour > 11)
                            CAMPO = "SAIDA_MATUTINO";
                        else
                            CAMPO = "CHEGADA_MATUTINO";
                    else
                        CAMPO = "SAIDA_MATUTINO";
                }
                else
                {
                    if (dtPonto.Rows[0]["SAIDA_MATUTINO"] == DBNull.Value && dtPonto.Rows[0]["CHEGADA_VESPERTINO"] == DBNull.Value)
                    {
                        if (getVerificaTempoDiferenca(Convert.ToDateTime(Convert.ToDateTime(dtPonto.Rows[0]["DATA_PONTO"]).ToShortDateString() + " " + dtPonto.Rows[0]["CHEGADA_MATUTINO"]), hora))
                            CAMPO = "SAIDA_MATUTINO";
                        else
                        {
                            rp.Msg = "Favor tentar após " + Convert.ToDateTime(Convert.ToDateTime(dtPonto.Rows[0]["DATA_PONTO"]).ToShortDateString() + " " + dtPonto.Rows[0]["CHEGADA_MATUTINO"]).AddMinutes(10).ToShortTimeString();
                        }
                    }
                    else if (dtPonto.Rows[0]["CHEGADA_VESPERTINO"] == DBNull.Value && dtPonto.Rows[0]["SAIDA_MATUTINO"] != DBNull.Value
                        && dtPonto.Rows[0]["SAIDA_VESPERTINO"] == DBNull.Value)
                    {
                        if (getVerificaTempoDiferenca(Convert.ToDateTime(Convert.ToDateTime(dtPonto.Rows[0]["DATA_PONTO"]).ToShortDateString() + " " + dtPonto.Rows[0]["SAIDA_MATUTINO"]), hora))
                            CAMPO = "CHEGADA_VESPERTINO";
                        else
                        {
                            rp.Msg = "Favor tentar após " + Convert.ToDateTime(Convert.ToDateTime(dtPonto.Rows[0]["DATA_PONTO"]).ToShortDateString() + " " + dtPonto.Rows[0]["SAIDA_MATUTINO"]).AddMinutes(10).ToShortTimeString();
                        }
                    }
                    else if (dtPonto.Rows[0]["SAIDA_VESPERTINO"] == DBNull.Value && dtPonto.Rows[0]["CHEGADA_VESPERTINO"] != DBNull.Value)
                    {
                        if (getVerificaTempoDiferenca(Convert.ToDateTime(Convert.ToDateTime(dtPonto.Rows[0]["DATA_PONTO"]).ToShortDateString() + " " + dtPonto.Rows[0]["CHEGADA_VESPERTINO"]), hora))
                            CAMPO = "SAIDA_VESPERTINO";
                        else
                        {
                            rp.Msg = "Favor tentar após " + Convert.ToDateTime(Convert.ToDateTime(dtPonto.Rows[0]["DATA_PONTO"]).ToShortDateString() + " " + dtPonto.Rows[0]["CHEGADA_VESPERTINO"]).AddMinutes(10).ToShortTimeString();
                        }
                    }
                }

                if (CAMPO != string.Empty)
                {
                    /*if (CAMPO == "SAIDA_MATUTINO" && hora.Hour > 13)
                    {
                        CAMPO = "SAIDA_MATUTINO='11:30', CHEGADA_VESPERTINO";
                    }*/

                    using (SqlConnection conn = new SqlConnection(Utilitarios.conStr))
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand("SET DATEFORMAT DMY; UPDATE EXTRANET.COLABORADOR_PONTO SET " + CAMPO + "='" + hora + "' WHERE COLABORADOR_ID=@COLABORADOR_ID AND DATA_PONTO='" + DateTime.Now.ToShortDateString() + "'", conn))
                        {
                            cmd.Parameters.Add("@COLABORADOR_ID", SqlDbType.Int).Value = colaborador_id;
                            cmd.ExecuteNonQuery();
                            rp.Msg = "Ponto registrado com sucesso!";
                        }
                    }
                }
                else if (rp.Msg == null)
                {
                    rp.Msg = "Relógio de ponto da data de hoje está completo!";
                }
            }
            else
            {
                //Alterado a hora para 7:30 conforme pedido no email
                if (hora.Hour > 5 || (hora.Minute > 00 && hora.Hour == 5))
                {
                    CAMPO = "CHEGADA_MATUTINO";

                    if (hora.Hour > 12 || (hora.Minute > 00 && hora.Hour == 12))
                        // Caso o colaborador trabalhe apenas dois horarios
                        if (dtPonto.Rows.Count > 0 &&
                            dtPonto_horas.Rows[0]["CHEGADA_VESPERTINO"] != DBNull.Value && dtPonto_horas.Rows[0]["CHEGADA_VESPERTINO"].ToString() == "00:00:00")
                            CAMPO = "SAIDA_MATUTINO";
                        else
                            CAMPO = "CHEGADA_VESPERTINO";

                    using (SqlConnection conn = new SqlConnection(Utilitarios.conStr))
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand("SET DATEFORMAT DMY; INSERT INTO EXTRANET.COLABORADOR_PONTO (COLABORADOR_ID,DATA_PONTO," + CAMPO + ") VALUES (@COLABORADOR_ID,@DATA_PONTO,@HORARIO)", conn))
                        {
                            cmd.Parameters.Add("@COLABORADOR_ID", SqlDbType.Int).Value = colaborador_id;
                            cmd.Parameters.Add("@DATA_PONTO", SqlDbType.Date).Value = DateTime.Now;
                            cmd.Parameters.Add("@HORARIO", SqlDbType.Time).Value = hora.ToShortTimeString();
                            cmd.ExecuteNonQuery();
                            rp.Msg = "Ponto registrado com sucesso!";
                        }
                    }
                }
                else
                {
                    rp.Msg = "Favor registrar o ponto após as 05:00hs";
                }
            }

            DataTable dtBatidas = new DataTable();
            var sql = "SET DATEFORMAT DMY; SELECT CONVERT(VARCHAR(5),CHEGADA_MATUTINO,108) AS CHEGADA_MATUTINO,CONVERT(VARCHAR(5),SAIDA_MATUTINO,108)AS SAIDA_MATUTINO,CONVERT(VARCHAR(5),CHEGADA_VESPERTINO,108)AS CHEGADA_VESPERTINO,CONVERT(VARCHAR(5),SAIDA_VESPERTINO,108)AS SAIDA_VESPERTINO " +
            "FROM EXTRANET.COLABORADOR_PONTO WHERE COLABORADOR_ID = " + colaborador_id + " AND CAST(DATA_PONTO AS DATE) = CAST(GETDATE() AS DATE)";

            dtBatidas = Utilitarios.Pesquisar(sql);

            if (dtBatidas.Rows.Count > 0)
            {
                if (dtPonto_horas.Rows.Count > 0 && dtPonto_horas.Rows[0]["CHEGADA_VESPERTINO"] != DBNull.Value)
                {
                    if (dtPonto_horas.Rows[0]["CHEGADA_VESPERTINO"].ToString() == "00:00:00")
                    {
                        rp.Batida1 = dtBatidas.Rows[0]["CHEGADA_MATUTINO"].ToString();
                        rp.Batida4 = dtBatidas.Rows[0]["SAIDA_MATUTINO"].ToString();
                    }
                }
                else
                {
                    rp.Batida1 = dtBatidas.Rows[0]["CHEGADA_MATUTINO"].ToString();
                    rp.Batida2 = dtBatidas.Rows[0]["SAIDA_MATUTINO"].ToString();
                    rp.Batida3 = dtBatidas.Rows[0]["CHEGADA_VESPERTINO"].ToString();
                    rp.Batida4 = dtBatidas.Rows[0]["SAIDA_VESPERTINO"].ToString();
                }
            }

            rp.Nome = dtColaborador.Rows[0]["NOME"].ToString();
        }
        else
        {
            rp.Msg = "Colaborador não encontrado, ou IP inválido!";
        }

        return rp;
    }


    public class RP
    {
        private string _msg;
        private string _nome;
        private byte[] _foto;
        private string _batida1;
        private string _batida2;
        private string _batida3;
        private string _batida4;

        public string Msg
        {
            get { return _msg; }
            set { _msg = value; }
        }

        public string Batida1
        {
            get { return _batida1; }
            set { _batida1 = value; }
        }

        public string Batida2
        {
            get { return _batida2; }
            set { _batida2 = value; }
        }

        public string Batida3
        {
            get { return _batida3; }
            set { _batida3 = value; }
        }

        public string Batida4
        {
            get { return _batida4; }
            set { _batida4 = value; }
        }

        public string Nome
        {
            get { return _nome; }
            set { _nome = value; }
        }

        public byte[] Foto
        {
            get { return _foto; }
            set { _foto = value; }
        }
    }

    protected void btnRegistrarBiometria_OnClick(object sender, EventArgs e)
    {
        m_NBioAPI = new NBioAPI();
        m_WinOption = new NBioAPI.Type.WINDOW_OPTION();
        m_WinOption.Option2 = new NBioAPI.Type.WINDOW_OPTION_2();
        m_NBioAPI.SetSkinResource("NBSP2Por.dll");

        SetInitValue();

        NBioAPI.Type.FIR_TEXTENCODE hRegisterdFIR = new NBioAPI.Type.FIR_TEXTENCODE();
        NBioAPI.Type.FIR_TEXTENCODE m_textFIR = new NBioAPI.Type.FIR_TEXTENCODE();
        m_textFIR.TextFIR = hdd_chave.Value;
        uint ret = 0;
        NBioAPI.Type.FIR_PAYLOAD myPayload = new NBioAPI.Type.FIR_PAYLOAD();
        bool result = false;

        bool registrou = CadastrarBiometria(DadosUsuarioLogado.COLABORADOR_ID, m_textFIR.TextFIR);
        if (registrou)
        {
            DadosUsuarioLogado.TEMBIOMETRIA = 1;
            ScriptManager.RegisterStartupScript(this, GetType(), "", "alert('Biometria registrada/atualizada com sucesso');window.location.href='Inicial.aspx'", true);
        }
        else
            ScriptManager.RegisterStartupScript(this, GetType(), "", "alert('Não foi possível registrar/atualizar sua biometria');", true);
    }

    public bool CadastrarBiometria(int colaborador_id, string hash)
    {
        DataTable dt = new DataTable();

        using (SqlConnection conn = new SqlConnection(Utilitarios.conStr))
        {
            conn.Open();
            SqlDataAdapter exec = new SqlDataAdapter();
            using (SqlCommand cmd = new SqlCommand("SELECT COLABORADOR_ID FROM EXTRANET.COLABORADOR WHERE COLABORADOR_ID=@COLABORADOR_ID AND COALESCE(CADASTRAR_BIOMETRIA,'N')='S'", conn))
            {
                cmd.Parameters.Add("@COLABORADOR_ID", SqlDbType.Int).Value = colaborador_id;
                exec.SelectCommand = cmd;
                exec.Fill(dt);
            }
        }

        //if (dt.Rows.Count > 0)
        //{
        try
        {
            using (SqlConnection conn = new SqlConnection(Utilitarios.conStr))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("UPDATE EXTRANET.COLABORADOR SET HASH_DIGITAL=@HASH_DIGITAL,CADASTRAR_BIOMETRIA='N' WHERE COLABORADOR_ID=@COLABORADOR_ID", conn))
                {
                    cmd.Parameters.Add("@COLABORADOR_ID", SqlDbType.Int).Value = colaborador_id;
                    cmd.Parameters.Add("@HASH_DIGITAL", SqlDbType.VarChar).Value = hash;
                    cmd.ExecuteNonQuery();
                }
            }
            return true;
        }
        catch (Exception e)
        {
            return false;
        }

        // }
    }

    private void LerArquivosHolerite()
    {
        var dt = new DataTable();
        dt.Columns.Add("Ano", typeof(int));
        dt.Columns.Add("Mes", typeof(int));
        dt.Columns.Add("Arquivo", typeof(string));

        var fim = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory.ToString() + "Images\\Papers");

        var ano = DateTime.Now.Month == 1 ? DateTime.Now.Year - 1 : DateTime.Now.Year;
        var mes = DateTime.Now.Month == 1 ? 12 : DateTime.Now.Month;

        //var ano = DateTime.Now.Year;
        //var mes = DateTime.Now.Month;

        for (int i = 0; i < 6; i++)
        {
            var fim1 = Path.Combine(fim.ToString(), ano + "_" + mes + "\\");

            if (Directory.Exists(fim1))
            {
                string[] files = Directory.GetFiles(fim1);
                ProcuraEspecificoHolerite(files, dt);
            }

            if (mes == 1)
            {
                mes = 12;
                ano--;
            }
            else
                mes--;
        }

        ano = DateTime.Now.Year;
        mes = DateTime.Now.Month;

        if (dt.Rows.Count > 0)
        {
            RptArquivos.DataSource = dt;
            RptArquivos.DataBind();
        }
    }

    private void ProcuraEspecificoHolerite(string[] arquivos, DataTable dt)
    {
        var codigo = DadosUsuarioLogado.CODIGO_PAPER;

        foreach (var item in arquivos)
        {
            var words = item.Split('-');
            var code = words[0].Substring(words[0].Length - 2, 2).Replace("\\", "") + "-" + words[4];
            if (code == codigo)
                MontaArquivos(item, dt);
        }
    }

    private void MontaArquivos(string arquivo, DataTable dt)
    {
        var row = dt.NewRow();
        row["Ano"] = arquivo.Split('-')[2];

        if (arquivo.Contains("13I"))
            row["Mes"] = "13";
        else
            row["Mes"] = arquivo.Split('-')[1];

        row["Arquivo"] = arquivo;
        dt.Rows.Add(row);
    }

    protected void RptArquivos_OnItemCommand(object source, RepeaterCommandEventArgs e)
    {
        FileInfo fInfo = new FileInfo(e.CommandArgument.ToString());

        Response.ContentType = "application/octet-stream";
        Response.AddHeader("Content-Disposition", "attachment; filename=\"" + Guid.NewGuid() + ".pdf\"");
        Response.AddHeader("Content-Length", fInfo.Length.ToString());
        Response.WriteFile(fInfo.FullName);
        Response.End();

        fInfo = null;
    }

    //marcar mensagem de motivação como lido
    protected void BtnMarcarLido_OnClick(object sender, EventArgs e)
    {
        var sql = "insert into MensagemMotivacionalLidas values (" + HddMensagemId.Value + ", " + DadosUsuarioLogado.COLABORADOR_ID + ", getdate())";
        Utilitarios.Exec_StringSql(sql);
        Session["VISTO"] = true;

        ScriptManager.RegisterStartupScript(this, GetType(), "redirect", "location.href = location.href;", true);
    }


    protected void txtCliente_SelectedIndexChanged(object sender, EventArgs e)
    {
        ExplodeGraficoPendente(true, txtcliente.SelectedValue.ToString());
        ChamaRelatorio(true);
    }

    protected void txtClienteConcluido_SelectedIndexChanged(object sender, EventArgs e)
    {
        ExplodeGraficoConcluido(true, txtclienteConcluido.SelectedValue.ToString());
        ChamaRelatorio(true);
    }

    protected void txtClientePesqAtendimento_SelectedIndexChanged(object sender, EventArgs e)
    {
        ExplodeGraficoAtendimento(true, txtclientePesqAtendimento.SelectedValue.ToString());
        ChamaRelatorio(true);
    }

    protected void txtClientePesqSistema_SelectedIndexChanged(object sender, EventArgs e)
    {
        ExplodeGraficoSistema(true, txtclientePesqSistema.SelectedValue.ToString());
        ChamaRelatorio(true);
    }

    protected void ExplodeGraficoPendente(bool acao, string clientePendente)
    {
        // pega sempre um mês atras.
        DateTime data = DateTime.Now.AddMonths(-1);
        string MesAnterior = data.Month.ToString();


        if (txtMes.SelectedValue != MesAnterior)
        {
            MesAnterior = txtMes.SelectedValue;
        }

        CorGraficos cor = new CorGraficos();

        string aux = "";
        string cabecalho = "";
        //string javascript = "";

        if (clientePendente == "")
        {

            cabecalho = "Top 10 Chamados Pendentes por cliente";

            DataTable dtPendentes = Utilitarios.Pesquisar("SELECT " +
           "TOP 10 A.NOME_ASSOCIADO AS NOME_CLIENTE, COUNT(DISTINCT H.CODIGO) AS QUANTIDADE_PENDENTE " +
           "FROM HELPDESK H " +
           "JOIN ASSESSORES A ON A.CODIGO = H.CODIGO_CLIENTE " +
           "WHERE H.TITULO IS NOT NULL AND H.STATUS NOT IN(12, 2, 19, 20, 7, 13, 31) " +
           "AND A.NOME_ASSOCIADO NOT LIKE '%AGENDA%' " +
           "GROUP BY A.NOME_ASSOCIADO " +
           "ORDER BY COUNT(DISTINCT H.CODIGO) DESC");




            for (int i = 0; i < dtPendentes.Rows.Count; i++)
            {
                aux += "[\"" + dtPendentes.Rows[i]["NOME_CLIENTE"].ToString() + "\"," + dtPendentes.Rows[i]["QUANTIDADE_PENDENTE"].ToString() + ",\"" + cor.RetornaNomeCor() + "\"],";
            }

            if (aux != "")
            {
                aux = aux.Remove(aux.Length - 1);
            }


        }
        else
        {

            cabecalho = "Chamados Pendentes do cliente Selecionado";


            DataTable dtPendentes = Utilitarios.Pesquisar("SET DATEFORMAT DMY " +
            "SELECT " +
            "CASE " +
            "WHEN C.DEPURACAO = 'S' THEN 'DEPURAÇÃO' " +
            "WHEN C.PDR = 'S' THEN 'PDR' " +
            "WHEN C.ATENDIMENTO = 'S'  THEN 'ATENDIMENTO' " +
            "WHEN C.ERRO = 'S' THEN 'INCIDÊNCIA' " +
            "WHEN C.MELHORIA = 'S' THEN 'MELHORIA' " +
            "WHEN C.OPERACIONAL = 'S' THEN 'ERRO OPERACIONAL' " +
            " END AS NOME_CATEGORIA ,  " +
            "COUNT(DISTINCT H.CODIGO) AS QUANTIDADE_PENDENTE, " +
            "(select item from dbo.getvaloressplit2(dbo.converteMinutos2(SUM(isnull(HorasCodes,0)+isnull(HorasCqs,0))*60),':') where id=1) AS ESFORCO " +
            "FROM HELPDESK H " +
            "JOIN ASSESSORES A ON A.CODIGO = H.CODIGO_CLIENTE " +
            "JOIN CATEGORIA C ON C.CODIGO = H.CODIGO_CATEGORIA " +
            "WHERE H.TITULO IS NOT NULL " +
            "AND H.STATUS NOT IN(12, 2, 19, 20, 7, 13, 31) AND H.CODIGO_CLIENTE = " + clientePendente +
            " GROUP BY " +
            "CASE " +
            "WHEN C.DEPURACAO = 'S' THEN 'DEPURAÇÃO' " +
            "WHEN C.PDR = 'S' THEN 'PDR' " +
            "WHEN C.ATENDIMENTO = 'S'  THEN 'ATENDIMENTO' " +
            "WHEN C.ERRO = 'S' THEN 'INCIDÊNCIA' " +
            "WHEN C.MELHORIA = 'S' THEN 'MELHORIA' " +
            "WHEN C.OPERACIONAL = 'S' THEN 'ERRO OPERACIONAL' " +
            " END  " +
            "ORDER BY COUNT(DISTINCT H.CODIGO) DESC");


            if (dtPendentes.Rows.Count == 0)
            {
                dtPendentes = Utilitarios.Pesquisar("SELECT 'SEM REGISTRO' AS NOME_CATEGORIA, 0 AS QUANTIDADE_PENDENTE, 0 AS ESFORCO");
            }

            for (int i = 0; i < dtPendentes.Rows.Count; i++)
            {
                //aux += "[\"" + dtPendentes.Rows[i]["NOME_CATEGORIA"].ToString() + "\"," + dtPendentes.Rows[i]["QUANTIDADE_PENDENTE"].ToString() + ",\"" + cor.RetornaNomeCor() + "\"],";
                aux += "[\"" + dtPendentes.Rows[i]["NOME_CATEGORIA"].ToString() + "\"," + dtPendentes.Rows[i]["QUANTIDADE_PENDENTE"].ToString() + "," + dtPendentes.Rows[i]["ESFORCO"].ToString() + "],";
            }

            if (aux != "")
            {
                aux = aux.Remove(aux.Length - 1);
            }
        }


        if (clientePendente == "")
        {
            javascript =
            // "google.charts.load(\"current\", { packages:['corechart']});" +
            "google.charts.setOnLoadCallback(drawChart);" +
            "function drawChart() {" +
            "var data = google.visualization.arrayToDataTable([[\"Element\", \"Pendentes\", { role:\"style\" } ]," + aux + "]);" +
            "var view = new google.visualization.DataView(data);" +
            "view.setColumns([0, 1," +
            "{ calc: \"stringify\"," +
            "sourceColumn: 1," +
            "type: \"string\"," +
            "role: \"annotation\" }," +
            "2]);" +
            "var options = {" +
            "title: \"" + cabecalho + "\"," +
            "width: 800," +
            "height: 500," +
            "hAxis: {slantedText: true,textStyle : {fontSize:7.5}}," +
            "bar: {groupWidth: \"95% \"}," +
            "legend: { position: \"none\"},};" +
            "var chart = new google.visualization.ColumnChart(document.getElementById(\"columnchart_values\")); " +
            "chart.draw(view, options);}";
        }
        else
        {
            javascript =
        //"google.charts.load(\"current\", { packages:['corechart']});" +
        "google.charts.setOnLoadCallback(drawChart);" +
        "function drawChart() {" +
        "var data = google.visualization.arrayToDataTable([[\"Categoria\", \"Quantidade de Pendentes\", \"Esforço Estimado em Horas\"]," + aux + "]);" +
        "var view = new google.visualization.DataView(data);" +
        "view.setColumns([0, 1,{ calc: \"stringify\",sourceColumn: 1, type: \"string\",role: \"annotation\" },2,{ calc: \"stringify\",sourceColumn: 2, type: \"string\",role: \"annotation\" }]);" +
        "var options = {" +
        "title: \"" + cabecalho + "\"," +
        "width: 800," +
        "height: 500," +
        "bar: {groupWidth: \"95% \"}," +
        "legend: { position: \"top\"},};" +
        "var chart = new google.visualization.ColumnChart(document.getElementById(\"columnchart_values\")); " +
        "chart.draw(view,options);}";

        }


        //if (acao == true)
        //{
        //    ScriptManager.RegisterStartupScript(this, GetType(), "", javascript + "$('body').removeClass('modal-open');$('.modal-backdrop').remove();$(\"#modalRelatorioEstrategico\").modal(\"show\");", true);

        //}
        //else
        //{
        //    ScriptManager.RegisterStartupScript(this, GetType(), "", javascript, true);
        //}


    }

    protected void ExplodeGraficoConcluido(bool acao, string clienteConcluido)
    {

        // pega sempre um mês atras.
        DateTime data = DateTime.Now.AddMonths(-1);
        string MesAnterior = data.Month.ToString();


        if (txtMes.SelectedValue != MesAnterior)
        {
            MesAnterior = txtMes.SelectedValue;
        }

        CorGraficos cor = new CorGraficos();


        string aux2 = "";
        string cabecalho2 = "";
        // string javascript2 = "";

        if (clienteConcluido == "")
        {

            cabecalho2 = "Top 10 Chamados Concluidos por cliente";


            //DataTable dtConcluidos = Utilitarios.Pesquisar("SELECT TOP  10 A.nome_associado as NOME_CLIENTE,COUNT(DISTINCT H.CODIGO) AS QUANTIDADE_RESOLVIDO " +
            //  "FROM HELPDESK H " +
            //  "JOIN ASSESSORES A ON A.CODIGO = H.CODIGO_CLIENTE " +
            //  "CROSS APPLY(SELECT TOP 1 data FROM interacoes I WHERE I.codigo_helpdesk = H.codigo AND(comentario = 'Alterou Status para: Resolvido' OR status = 2) ORDER BY I.codigo DESC) AS DT_RESOLVIDO " +
            //  "WHERE  " +
            //  "H.STATUS = 2 " +
            //  "AND nome_associado NOT LIKE '%AGENDA%' AND A.ativo = 'S' " +
            //  "AND MONTH(DT_RESOLVIDO.data) =" + MesAnterior +
            //  "AND YEAR(DT_RESOLVIDO.data) =" + data.Year.ToString() +
            //  "GROUP BY A.nome_associado " +
            //  "ORDER BY QUANTIDADE_RESOLVIDO DESC");

            DataTable dtConcluidos = Utilitarios.Pesquisar(
                "with interacoes_auxiliar as " +
                "( " +
                "select codigo, codigo_helpdesk, data " +
                "from interacoes where (comentario = 'Alterou Status para: Resolvido') " +
                "AND MONTH(data) = " + MesAnterior +
                " AND YEAR(data) =" + data.Year.ToString() +
                ") " +
                "SELECT TOP  10 A.nome_associado as NOME_CLIENTE,COUNT(DISTINCT H.CODIGO) AS QUANTIDADE_RESOLVIDO " +
                "FROM HELPDESK H " +
                "JOIN ASSESSORES A ON A.CODIGO = H.CODIGO_CLIENTE " +
                "cross apply (SELECT TOP 1 data FROM interacoes_auxiliar I  " +
                "WHERE I.codigo_helpdesk = H.codigo " +
                "ORDER BY I.codigo DESC " +
                ") AS DT_RESOLVIDO " +
                "WHERE H.STATUS = 2 " +
                "AND nome_associado NOT LIKE '%AGENDA%' " +
                "AND A.ativo = 'S' " +
                "GROUP BY A.nome_associado " +
                "ORDER BY QUANTIDADE_RESOLVIDO DESC ");




            for (int i = 0; i < dtConcluidos.Rows.Count; i++)
            {
                aux2 += "[\"" + dtConcluidos.Rows[i]["NOME_CLIENTE"].ToString() + "\"," + dtConcluidos.Rows[i]["QUANTIDADE_RESOLVIDO"].ToString() + ",\"" + cor.RetornaNomeCor() + "\"],";
            }

            if (aux2 != "")
            {
                aux2 = aux2.Remove(aux2.Length - 1);
            }
        }
        else
        {
            cabecalho2 = "Chamados Concluidos do cliente Selecionado";



            DataTable dtConcluidos = Utilitarios.Pesquisar("SELECT " +
            "CASE " +
            "WHEN C.DEPURACAO = 'S' THEN 'DEPURAÇÃO' " +
            "WHEN C.PDR = 'S' THEN 'PDR' " +
            "WHEN C.ATENDIMENTO = 'S'  THEN 'ATENDIMENTO' " +
            "WHEN C.ERRO = 'S' THEN 'INCIDÊNCIA' " +
            "WHEN C.MELHORIA = 'S' THEN 'MELHORIA' " +
            "WHEN C.OPERACIONAL = 'S' THEN 'ERRO OPERACIONAL' " +
            " END AS NOME_CATEGORIA ,  " +
            "COUNT(DISTINCT H.CODIGO) AS QUANTIDADE_RESOLVIDO  ," +
            "(select item from dbo.getvaloressplit2(dbo.converteMinutos2(SUM(isnull(HorasCatRealizada, 0) + isnull(HorasCatRealizadaApoio, 0) + isnull(HorasCodesRealizada, 0) + isnull(HorasCopetRealizada, 0) + isnull(HorasCopetRealizadaApoio, 0) + isnull(HorasCqsRealizada, 0)) * 60), ':') where id = 1) AS ESFORCO_REALIZADO " +
            "FROM HELPDESK H  " +
            "JOIN ASSESSORES A ON A.CODIGO = H.CODIGO_CLIENTE  " +
            "JOIN CATEGORIA C ON C.CODIGO = H.CODIGO_CATEGORIA  " +
            "CROSS APPLY(SELECT TOP 1 DATA FROM INTERACOES I WHERE I.CODIGO_HELPDESK = H.CODIGO AND(COMENTARIO = 'ALTEROU STATUS PARA: RESOLVIDO' OR STATUS = 2) ORDER BY I.CODIGO DESC) AS DT_RESOLVIDO  " +
            "WHERE  " +
            "H.STATUS = 2  " +
            "AND A.ATIVO = 'S'  " +
            "AND MONTH(DT_RESOLVIDO.DATA) =" + MesAnterior +
            " AND YEAR(DT_RESOLVIDO.DATA) =" + data.Year.ToString() +
            " AND H.CODIGO_CLIENTE = " + clienteConcluido +
            " GROUP BY CASE " +
            "WHEN C.DEPURACAO = 'S' THEN 'DEPURAÇÃO' " +
            "WHEN C.PDR = 'S' THEN 'PDR' " +
             "WHEN C.ATENDIMENTO = 'S'  THEN 'ATENDIMENTO' " +
            "WHEN C.ERRO = 'S' THEN 'INCIDÊNCIA' " +
            "WHEN C.MELHORIA = 'S' THEN 'MELHORIA' " +
            "WHEN C.OPERACIONAL = 'S' THEN 'ERRO OPERACIONAL' " +
            " END " +
            "ORDER BY QUANTIDADE_RESOLVIDO DESC ");

            if (dtConcluidos.Rows.Count == 0)
            {
                dtConcluidos = Utilitarios.Pesquisar(" SELECT 'SEM REGISTRO' AS NOME_CATEGORIA,0 AS QUANTIDADE_RESOLVIDO, 0 AS ESFORCO_REALIZADO");
            }


            for (int i = 0; i < dtConcluidos.Rows.Count; i++)
            {
                //aux2 += "[\"" + dtConcluidos.Rows[i]["NOME_CATEGORIA"].ToString() + "\"," + dtConcluidos.Rows[i]["QUANTIDADE_RESOLVIDO"].ToString() + ",\"" + cor.RetornaNomeCor() + "\"],";
                aux2 += "[\"" + dtConcluidos.Rows[i]["NOME_CATEGORIA"].ToString() + "\"," + dtConcluidos.Rows[i]["QUANTIDADE_RESOLVIDO"].ToString() + "," + dtConcluidos.Rows[i]["ESFORCO_REALIZADO"].ToString() + "],";
            }

            if (aux2 != "")
            {
                aux2 = aux2.Remove(aux2.Length - 1);
            }



        }


        if (clienteConcluido == "")
        {
            javascript2 =
                "google.charts.setOnLoadCallback(drawChart2);" +
                "function drawChart2() {" +
                "var data2 = google.visualization.arrayToDataTable([[\"Element\", \"Concluidos\", { role:\"style\" } ]," + aux2 + "]);" +
                "var view2 = new google.visualization.DataView(data2);" +
                "view2.setColumns([0, 1," +
                "{ calc: \"stringify\"," +
                "sourceColumn: 1," +
                "type: \"string\"," +
                "role: \"annotation\" }," +
                "2]);" +
                "var options2 = {" +
                 "title: \"" + cabecalho2 + "\"," +
                "width: 800," +
                "height: 500," +
                "hAxis: {slantedText: true,textStyle : {fontSize:7.5}}," +
                "bar: {groupWidth: \"95% \"}," +
                "legend: { position: \"none\"},};" +
                "var chart2 = new google.visualization.ColumnChart(document.getElementById(\"columnchart_values2\")); " +
                "chart2.draw(view2, options2);}";
        }
        else
        {
            javascript2 =
          "google.charts.setOnLoadCallback(drawChart2);" +
          "function drawChart2() {" +
          "var data2 = google.visualization.arrayToDataTable([[\"Categoria\", \"Quantidade de Concluidos\", \"Esforço Realizado em Horas\"]," + aux2 + "]);" +
          "var view2 = new google.visualization.DataView(data2);" +
          "view2.setColumns([0, 1,{ calc: \"stringify\",sourceColumn: 1, type: \"string\",role: \"annotation\" },2,{ calc: \"stringify\",sourceColumn: 2, type: \"string\",role: \"annotation\" }]);" +
          "var options2 = {" +
          "title: \"" + cabecalho2 + "\"," +
          "width: 800," +
          "height: 500," +
          "bar: {groupWidth: \"95% \"}," +
          "legend: { position: \"top\"},};" +
          "var chart2 = new google.visualization.ColumnChart(document.getElementById(\"columnchart_values2\")); " +
          "chart2.draw(view2,options2);}";
        }



        //if (acao == true)
        //{
        //    ScriptManager.RegisterStartupScript(this, GetType(), "", javascript2 + "$('body').removeClass('modal-open');$('.modal-backdrop').remove();$(\"#modalRelatorioEstrategico\").modal(\"show\");", true);

        //}
        //else
        //{
        //    ScriptManager.RegisterStartupScript(this, GetType(), "", javascript2, true);
        //}



    }

    protected void ExplodeGraficoSistema(bool acao, string clienteSistema)
    {
        // pega sempre um mês atras.
        DateTime data = DateTime.Now.AddMonths(-1);
        string MesAnterior = data.Month.ToString();

        if (txtMes.SelectedValue != MesAnterior)
        {
            MesAnterior = txtMes.SelectedValue;
        }

        CorGraficos cor = new CorGraficos();



        string Concatena;
        string cabecalho3 = "";
        if (clienteSistema != "")
        {
            Concatena = " AND A.CODIGO= " + clienteSistema + " ";
            cabecalho3 = "Pesquisa e Satisfação do Sistema do Cliente Selecionado:";
        }
        else
        {
            Concatena = "";
            cabecalho3 = "Resultado Geral da Pesquisa de Satisfação do Sistema:";
        }

        DataTable PesquisaSistema = Utilitarios.Pesquisar("SELECT P.NOME AS AVALIACAO, COUNT(E.CODIGO_HELPDESK) AS QTD_AVALIACAO " +
           "FROM EXTRANET.PESQUISA_SATISFACAO_PAINEL E " +
           "JOIN ASSESSORES A ON A.CODIGO = E.CLIENTE_ID " +
           "JOIN EXTRANET.PESQUISA_SATISFACAO_PAINEL_RANKS P ON P.ID = E.AVALIACAO_SISTEMA " +
           "WHERE MONTH(E.DATA) =" + data.Month.ToString() + " AND YEAR(E.DATA) =" + data.Year.ToString() + Concatena +
        " GROUP BY P.NOME ORDER BY COUNT(E.CODIGO_HELPDESK) DESC");



        if (PesquisaSistema.Rows.Count == 0)
        {
            PesquisaSistema = Utilitarios.Pesquisar("SELECT 'SEM REGISTRO' AS AVALIACAO, 1 AS QTD_AVALIACAO");
        }


        string aux3 = "['NOME', 'TOTAL'],";

        for (int i = 0; i < PesquisaSistema.Rows.Count; i++)
        {
            //aux3 += "['" + PesquisaSistema.Rows[i]["AVALIACAO"].ToString() + "'," + PesquisaSistema.Rows[i]["QTD_AVALIACAO"].ToString() + "],";

            aux3 += "['" + PesquisaSistema.Rows[i]["AVALIACAO"].ToString() + " - " + PesquisaSistema.Rows[i]["QTD_AVALIACAO"].ToString() + "' ," + PesquisaSistema.Rows[i]["QTD_AVALIACAO"].ToString() + "],";
        }

        aux3 = aux3.Remove(aux3.Length - 1);

        javascript3 =
                            "google.charts.setOnLoadCallback(drawChart3);" +
                            "function drawChart3(){" +
                            "var data3 = google.visualization.arrayToDataTable([" +
                            aux3 +
                            "]);" +
                            "var options3 = { title: '" + cabecalho3 + "',width: 800, height: 500, 'chartArea': {'width': '80%', 'height': '80%'}}; " +
                            "var chart3 = new google.visualization.PieChart(document.getElementById('piechart')); " +
                            "chart3.draw(data3, options3);" +
                            //"};$('body').removeClass('modal-open');$('.modal-backdrop').remove();$(\"#modalGrafico\").modal(\"show\");";
                            "};$('body').removeClass('modal-open');$('.modal-backdrop').remove();";



    }

    protected void ExplodeGraficoAtendimento(bool acao, string clienteAtendimento)
    {

        DateTime data = DateTime.Now.AddMonths(-1);
        string MesAnterior = data.Month.ToString();

        if (txtMes.SelectedValue != MesAnterior)
        {
            MesAnterior = txtMes.SelectedValue;
        }

        CorGraficos cor = new CorGraficos();


        string Concatena2;
        if (clienteAtendimento != "")
        {
            Concatena2 = " AND A.CODIGO= " + clienteAtendimento + " ";
        }
        else
        {
            Concatena2 = "";
        }




        DataTable PesquisaAtedimento = Utilitarios.Pesquisar("SELECT P.NOME AS AVALIACAO, COUNT(E.CODIGO_HELPDESK) AS QTD_AVALIACAO " +
                "FROM EXTRANET.PESQUISA_SATISFACAO_PAINEL E " +
                "JOIN ASSESSORES A ON A.CODIGO = E.CLIENTE_ID " +
                "JOIN EXTRANET.PESQUISA_SATISFACAO_PAINEL_RANKS P ON P.ID = E.AVALIACAO_ATENDIMENTO " +
                "WHERE MONTH(E.DATA) =" + data.Month.ToString() + " AND YEAR(E.DATA) =" + data.Year.ToString() + Concatena2 +
                "GROUP BY P.NOME ORDER BY COUNT(E.CODIGO_HELPDESK) DESC");



        if (PesquisaAtedimento.Rows.Count == 0)
        {
            PesquisaAtedimento = Utilitarios.Pesquisar("SELECT 'SEM REGISTRO' AS AVALIACAO, 1 AS QTD_AVALIACAO");
        }


        string aux4 = "['NOME', 'TOTAL'],";

        for (int i = 0; i < PesquisaAtedimento.Rows.Count; i++)
        {
            //aux4 += "['" + PesquisaAtedimento.Rows[i]["AVALIACAO"].ToString() + "'," + PesquisaAtedimento.Rows[i]["QTD_AVALIACAO"].ToString() + "],";

            aux4 += "['" + PesquisaAtedimento.Rows[i]["AVALIACAO"].ToString() + " - " + PesquisaAtedimento.Rows[i]["QTD_AVALIACAO"].ToString() + "' ," + PesquisaAtedimento.Rows[i]["QTD_AVALIACAO"].ToString() + "],";

        }

        aux4 = aux4.Remove(aux4.Length - 1);

        javascript4 =
                            "google.charts.setOnLoadCallback(drawChart4);" +
                            "function drawChart4(){" +
                            "var data4 = google.visualization.arrayToDataTable([" +
                            aux4 +
                            "]);" +
                            "var options4 = { title: 'Resultado da Pesquisa de Satisfação do Atendimento',width: 800, height: 500, 'chartArea': {'width': '80%', 'height': '80%'}}; " +
                            "var chart4 = new google.visualization.PieChart(document.getElementById('piechart2')); " +
                            "chart4.draw(data4, options4);" +

                            "};$('body').removeClass('modal-open');$('.modal-backdrop').remove();";





        //// ScriptManager.RegisterStartupScript(this, GetType(), "", javascript4 , true);

        //if (acao == true)
        //{
        //    ScriptManager.RegisterStartupScript(this, GetType(), "", javascript+javascript2+javascript3+javascript4 + "$('body').removeClass('modal-open');$('.modal-backdrop').remove();$(\"#modalRelatorioEstrategico\").modal(\"show\");", true);

        //}
        //else
        //{
        //    ScriptManager.RegisterStartupScript(this, GetType(), "", javascript4, true);
        //}
    }

    protected void ChamaRelatorio(bool acao)
    {

        string itemgoogle = "google.charts.load(\"current\", { packages:['corechart']});";

        if (acao == true)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "", itemgoogle + javascript + javascript2 + javascript3 + javascript4 + "$('body').removeClass('modal-open');$('.modal-backdrop').remove();$(\"#modalRelatorioEstrategico\").modal(\"show\");", true);

        }
        else
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "", itemgoogle + javascript + javascript2 + javascript3 + javascript4, true);
        }
    }

    //protected void ExplodeGrafico(bool acao, string clientePendente, string clienteConcluido, string clienteSistema, string clienteAtendimento)
    //{
    //    // pega sempre um mês atras.
    //    DateTime data = DateTime.Now.AddMonths(-1);
    //    string MesAnterior = data.Month.ToString();


    //    if(txtMes.SelectedValue != MesAnterior)
    //    {
    //        MesAnterior = txtMes.SelectedValue;
    //    }

    //    CorGraficos cor = new CorGraficos();

    //    string aux = "";
    //    string cabecalho = "";
    //    string javascript = "";

    //    if (clientePendente == "")
    //    {

    //        cabecalho = "Top 10 Chamados Pendentes por cliente";


    //        DataTable dtPendentes = Utilitarios.Pesquisar("SET DATEFORMAT DMY SELECT DISTINCT TOP 10 NOME_CLIENTE,COUNT(*) AS QUANTIDADE_PENDENTE " +
    //                "FROM VW_CHAMADOS A WHERE A.TITULO IS NOT NULL " +
    //                "AND STATUS NOT IN(12, 2, 19, 20, 7, 13, 31 ) AND NOME_CLIENTE NOT LIKE '%AGENDA%' " +
    //                "GROUP BY NOME_CLIENTE " +
    //                "ORDER BY COUNT(*) DESC");




    //        for (int i = 0; i < dtPendentes.Rows.Count; i++)
    //        {
    //            aux += "[\"" + dtPendentes.Rows[i]["NOME_CLIENTE"].ToString() + "\"," + dtPendentes.Rows[i]["QUANTIDADE_PENDENTE"].ToString() + ",\"" + cor.RetornaNomeCor() + "\"],";
    //        }

    //        if (aux != "")
    //        {
    //            aux = aux.Remove(aux.Length - 1);
    //        }


    //    }
    //     else
    //    {

    //        cabecalho = "Chamados Pendentes do cliente Selecionado";


    //        DataTable dtPendentes = Utilitarios.Pesquisar("SET DATEFORMAT DMY " +
    //        "SELECT " +
    //        "CASE " +
    //        "WHEN C.DEPURACAO = 'S' THEN 'DEPURAÇÃO' " +
    //        "WHEN C.PDR = 'S' THEN 'PDR' " +
    //        "WHEN C.ATENDIMENTO = 'S'  THEN 'ATENDIMENTO' " +
    //        "WHEN C.ERRO = 'S' THEN 'INCIDÊNCIA' " +
    //        "WHEN C.MELHORIA = 'S' THEN 'MELHORIA' " +
    //        "WHEN C.OPERACIONAL = 'S' THEN 'ERRO OPERACIONAL' " +
    //        " END AS NOME_CATEGORIA ,  " +
    //        "COUNT(*) AS QUANTIDADE_PENDENTE, " +
    //        "(select item from dbo.getvaloressplit2(dbo.converteMinutos2(SUM(isnull(HorasCodes,0)+isnull(HorasCqs,0))*60),':') where id=1) AS ESFORCO "+
    //        "FROM HELPDESK H " +
    //        "JOIN ASSESSORES A ON A.CODIGO = H.CODIGO_CLIENTE " +
    //        "JOIN CATEGORIA C ON C.CODIGO = H.CODIGO_CATEGORIA " +
    //        "WHERE H.TITULO IS NOT NULL " +
    //        "AND H.STATUS NOT IN(12, 2, 19, 20, 7, 13, 31) AND H.CODIGO_CLIENTE = "+ clientePendente +
    //        " GROUP BY " +
    //        "CASE " +
    //        "WHEN C.DEPURACAO = 'S' THEN 'DEPURAÇÃO' " +
    //        "WHEN C.PDR = 'S' THEN 'PDR' " +
    //        "WHEN C.ATENDIMENTO = 'S'  THEN 'ATENDIMENTO' " +
    //        "WHEN C.ERRO = 'S' THEN 'INCIDÊNCIA' " +
    //        "WHEN C.MELHORIA = 'S' THEN 'MELHORIA' " +
    //        "WHEN C.OPERACIONAL = 'S' THEN 'ERRO OPERACIONAL' " +
    //        " END  " +
    //        "ORDER BY COUNT(*) DESC");


    //        for (int i = 0; i < dtPendentes.Rows.Count; i++)
    //        {
    //            //aux += "[\"" + dtPendentes.Rows[i]["NOME_CATEGORIA"].ToString() + "\"," + dtPendentes.Rows[i]["QUANTIDADE_PENDENTE"].ToString() + ",\"" + cor.RetornaNomeCor() + "\"],";
    //            aux += "[\"" + dtPendentes.Rows[i]["NOME_CATEGORIA"].ToString() + "\"," + dtPendentes.Rows[i]["QUANTIDADE_PENDENTE"].ToString() + "," + dtPendentes.Rows[i]["ESFORCO"].ToString() + "],";
    //        }

    //        if (aux != "")
    //        {
    //            aux = aux.Remove(aux.Length - 1);
    //        }
    //    }



    //if(clientePendente=="")
    //    {
    //        javascript =
    //        "google.charts.load(\"current\", { packages:['corechart']});" +
    //        "google.charts.setOnLoadCallback(drawChart);" +
    //        "function drawChart() {" +
    //        "var data = google.visualization.arrayToDataTable([[\"Element\", \"Pendentes\", { role:\"style\" } ]," + aux + "]);" +
    //        "var view = new google.visualization.DataView(data);" +
    //        "view.setColumns([0, 1," +
    //        "{ calc: \"stringify\"," +
    //        "sourceColumn: 1," +
    //        "type: \"string\"," +
    //        "role: \"annotation\" }," +
    //        "2]);" +
    //        "var options = {" +
    //        "title: \"" + cabecalho + "\"," +
    //        "width: 800," +
    //        "height: 500," +
    //        "hAxis: {slantedText: true,textStyle : {fontSize:7.5}}," +
    //        "bar: {groupWidth: \"95% \"}," +
    //        "legend: { position: \"none\"},};" +
    //        "var chart = new google.visualization.ColumnChart(document.getElementById(\"columnchart_values\")); " +
    //        "chart.draw(view, options);}";
    //    }
    //else
    //    {
    //        javascript =
    //    "google.charts.load(\"current\", { packages:['corechart']});" +
    //    "google.charts.setOnLoadCallback(drawChart);" +
    //    "function drawChart() {" +
    //    "var data = google.visualization.arrayToDataTable([[\"Categoria\", \"Quantidade de Pendentes\", \"Esforço Estimado em Horas\"]," + aux + "]);" +
    //    "var view = new google.visualization.DataView(data);" +
    //    "view.setColumns([0, 1,{ calc: \"stringify\",sourceColumn: 1, type: \"string\",role: \"annotation\" },2,{ calc: \"stringify\",sourceColumn: 2, type: \"string\",role: \"annotation\" }]);" +
    //    "var options = {" +
    //    "title: \"" + cabecalho + "\"," +
    //    "width: 800," +
    //    "height: 500," +
    //    "bar: {groupWidth: \"95% \"}," +
    //    "legend: { position: \"top\"},};" +
    //    "var chart = new google.visualization.ColumnChart(document.getElementById(\"columnchart_values\")); " +
    //    "chart.draw(view,options);}";




    //    }







    //    string aux2 = "";
    //    string cabecalho2 = "";
    //    string javascript2 = "";

    //    if (clienteConcluido == "")
    //    {

    //        cabecalho2 = "Top 10 Chamados Concluidos por cliente";


    //        DataTable dtConcluidos = Utilitarios.Pesquisar("SELECT TOP  10 A.nome_associado as NOME_CLIENTE,COUNT(*) AS QUANTIDADE_RESOLVIDO " +
    //          "FROM HELPDESK H " +
    //          "JOIN ASSESSORES A ON A.CODIGO = H.CODIGO_CLIENTE " +
    //          "CROSS APPLY(SELECT TOP 1 data FROM interacoes I WHERE I.codigo_helpdesk = H.codigo AND(comentario = 'Alterou Status para: Resolvido' OR status = 2) ORDER BY I.codigo DESC) AS DT_RESOLVIDO " +
    //          "WHERE  " +
    //          "H.STATUS = 2 " +
    //          "AND nome_associado NOT LIKE '%AGENDA%' AND A.ativo = 'S' " +
    //          "AND MONTH(DT_RESOLVIDO.data) =" + MesAnterior +
    //          "AND YEAR(DT_RESOLVIDO.data) =" + data.Year.ToString() +
    //          "GROUP BY A.nome_associado " +
    //          "ORDER BY QUANTIDADE_RESOLVIDO DESC");


    //        for (int i = 0; i < dtConcluidos.Rows.Count; i++)
    //        {
    //            aux2 += "[\"" + dtConcluidos.Rows[i]["NOME_CLIENTE"].ToString() + "\"," + dtConcluidos.Rows[i]["QUANTIDADE_RESOLVIDO"].ToString() + ",\"" + cor.RetornaNomeCor() + "\"],";
    //        }

    //        if (aux2 != "")
    //        {
    //            aux2 = aux2.Remove(aux2.Length - 1);
    //        }
    //    }
    //    else
    //    {
    //        cabecalho2 = "Chamados Concluidos do cliente Selecionado";

    //        DataTable dtConcluidos = Utilitarios.Pesquisar("SELECT " +
    //        "CASE " +
    //        "WHEN C.DEPURACAO = 'S' THEN 'DEPURAÇÃO' " +
    //        "WHEN C.PDR = 'S' THEN 'PDR' " +
    //        "WHEN C.ATENDIMENTO = 'S'  THEN 'ATENDIMENTO' " +
    //        "WHEN C.ERRO = 'S' THEN 'INCIDÊNCIA' " +
    //        "WHEN C.MELHORIA = 'S' THEN 'MELHORIA' " +
    //        "WHEN C.OPERACIONAL = 'S' THEN 'ERRO OPERACIONAL' " +
    //        " END AS NOME_CATEGORIA ,  " +
    //        "COUNT(*) AS QUANTIDADE_RESOLVIDO  ," +
    //        "(select item from dbo.getvaloressplit2(dbo.converteMinutos2(SUM(isnull(HorasCatRealizada, 0) + isnull(HorasCatRealizadaApoio, 0) + isnull(HorasCodesRealizada, 0) + isnull(HorasCopetRealizada, 0) + isnull(HorasCopetRealizadaApoio, 0) + isnull(HorasCqsRealizada, 0)) * 60), ':') where id = 1) AS ESFORCO_REALIZADO " +
    //        "FROM HELPDESK H  " +
    //        "JOIN ASSESSORES A ON A.CODIGO = H.CODIGO_CLIENTE  " +
    //        "JOIN CATEGORIA C ON C.CODIGO = H.CODIGO_CATEGORIA  " +
    //        "CROSS APPLY(SELECT TOP 1 DATA FROM INTERACOES I WHERE I.CODIGO_HELPDESK = H.CODIGO AND(COMENTARIO = 'ALTEROU STATUS PARA: RESOLVIDO' OR STATUS = 2) ORDER BY I.CODIGO DESC) AS DT_RESOLVIDO  " +
    //        "WHERE  " +
    //        "H.STATUS = 2  " +
    //        "AND A.ATIVO = 'S'  " +
    //        "AND MONTH(DT_RESOLVIDO.DATA) =" + MesAnterior +
    //        " AND YEAR(DT_RESOLVIDO.DATA) =" + data.Year.ToString() +
    //        " AND H.CODIGO_CLIENTE = " + clienteConcluido +
    //        " GROUP BY CASE " +
    //        "WHEN C.DEPURACAO = 'S' THEN 'DEPURAÇÃO' " +
    //        "WHEN C.PDR = 'S' THEN 'PDR' " +
    //         "WHEN C.ATENDIMENTO = 'S'  THEN 'ATENDIMENTO' " +
    //        "WHEN C.ERRO = 'S' THEN 'INCIDÊNCIA' " +
    //        "WHEN C.MELHORIA = 'S' THEN 'MELHORIA' " +
    //        "WHEN C.OPERACIONAL = 'S' THEN 'ERRO OPERACIONAL' " +
    //        " END " +
    //        "ORDER BY QUANTIDADE_RESOLVIDO DESC ") ;


    //        for (int i = 0; i < dtConcluidos.Rows.Count; i++)
    //        {
    //            //aux2 += "[\"" + dtConcluidos.Rows[i]["NOME_CATEGORIA"].ToString() + "\"," + dtConcluidos.Rows[i]["QUANTIDADE_RESOLVIDO"].ToString() + ",\"" + cor.RetornaNomeCor() + "\"],";
    //            aux2 += "[\"" + dtConcluidos.Rows[i]["NOME_CATEGORIA"].ToString() + "\"," + dtConcluidos.Rows[i]["QUANTIDADE_RESOLVIDO"].ToString() + "," + dtConcluidos.Rows[i]["ESFORCO_REALIZADO"].ToString() + "],";
    //        }

    //        if (aux2 != "")
    //        {
    //            aux2 = aux2.Remove(aux2.Length - 1);
    //        }



    //    }


    //    if(clienteConcluido=="")
    //    {
    //        javascript2 =
    //            "google.charts.setOnLoadCallback(drawChart2);" +
    //            "function drawChart2() {" +
    //            "var data2 = google.visualization.arrayToDataTable([[\"Element\", \"Concluidos\", { role:\"style\" } ]," + aux2 + "]);" +
    //            "var view2 = new google.visualization.DataView(data2);" +
    //            "view2.setColumns([0, 1," +
    //            "{ calc: \"stringify\"," +
    //            "sourceColumn: 1," +
    //            "type: \"string\"," +
    //            "role: \"annotation\" }," +
    //            "2]);" +
    //            "var options2 = {" +
    //             "title: \"" + cabecalho2 + "\"," +
    //            "width: 800," +
    //            "height: 500," +
    //            "hAxis: {slantedText: true,textStyle : {fontSize:7.5}}," +
    //            "bar: {groupWidth: \"95% \"}," +
    //            "legend: { position: \"none\"},};" +
    //            "var chart2 = new google.visualization.ColumnChart(document.getElementById(\"columnchart_values2\")); " +
    //            "chart2.draw(view2, options2);}";
    //    }
    //    else
    //    {
    //        javascript2 =
    //      "google.charts.setOnLoadCallback(drawChart2);" +
    //      "function drawChart2() {" +
    //      "var data2 = google.visualization.arrayToDataTable([[\"Categoria\", \"Quantidade de Concluidos\", \"Esforço Realizado em Horas\"]," + aux2 + "]);" +
    //      "var view2 = new google.visualization.DataView(data2);" +
    //      "view2.setColumns([0, 1,{ calc: \"stringify\",sourceColumn: 1, type: \"string\",role: \"annotation\" },2,{ calc: \"stringify\",sourceColumn: 2, type: \"string\",role: \"annotation\" }]);" +
    //      "var options2 = {" +
    //      "title: \"" + cabecalho2 + "\"," +
    //      "width: 800," +
    //      "height: 500," +
    //      "bar: {groupWidth: \"95% \"}," +
    //      "legend: { position: \"top\"},};" +
    //      "var chart2 = new google.visualization.ColumnChart(document.getElementById(\"columnchart_values2\")); " +
    //      "chart2.draw(view2,options2);}";
    //    }




    //    string Concatena;
    //    string cabecalho3 = "";
    //    if (clienteSistema !="")
    //    {
    //        Concatena = " AND A.CODIGO= " + clienteSistema + " ";
    //        cabecalho3 = "Pesquisa e Satisfação do Sistema do Cliente Selecionado:";
    //    }
    //    else
    //    {
    //        Concatena = "";
    //        cabecalho3 = "Resultado Geral da Pesquisa de Satisfação do Sistema:";
    //    }

    //    DataTable PesquisaSistema = Utilitarios.Pesquisar("SELECT P.NOME AS AVALIACAO, COUNT(*) AS QTD_AVALIACAO " +
    //       "FROM EXTRANET.PESQUISA_SATISFACAO_PAINEL E " +
    //       "JOIN ASSESSORES A ON A.CODIGO = E.CLIENTE_ID " +
    //       "JOIN EXTRANET.PESQUISA_SATISFACAO_PAINEL_RANKS P ON P.ID = E.AVALIACAO_SISTEMA " +
    //       "WHERE MONTH(E.DATA) =" + data.Month.ToString() + " AND YEAR(E.DATA) =" + data.Year.ToString() + Concatena +
    //    " GROUP BY P.NOME ORDER BY COUNT(*) DESC");




    //    string aux3 = "['NOME', 'TOTAL'],";

    //    for (int i = 0; i < PesquisaSistema.Rows.Count; i++)
    //    {
    //        //aux3 += "['" + PesquisaSistema.Rows[i]["AVALIACAO"].ToString() + "'," + PesquisaSistema.Rows[i]["QTD_AVALIACAO"].ToString() + "],";

    //        aux3 += "['" + PesquisaSistema.Rows[i]["AVALIACAO"].ToString() + " - " + PesquisaSistema.Rows[i]["QTD_AVALIACAO"].ToString() + "' ," + PesquisaSistema.Rows[i]["QTD_AVALIACAO"].ToString() + "],";
    //    }

    //    aux3 = aux3.Remove(aux3.Length - 1);

    //    string javascript3 =
    //                        "google.charts.setOnLoadCallback(drawChart3);" +
    //                        "function drawChart3(){" +
    //                        "var data3 = google.visualization.arrayToDataTable([" +
    //                        aux3 +
    //                        "]);" +
    //                        "var options3 = { title: '"+ cabecalho3+"',width: 800, height: 500, 'chartArea': {'width': '80%', 'height': '80%'}}; " +
    //                        "var chart3 = new google.visualization.PieChart(document.getElementById('piechart')); " +
    //                        "chart3.draw(data3, options3);" +
    //                        //"};$('body').removeClass('modal-open');$('.modal-backdrop').remove();$(\"#modalGrafico\").modal(\"show\");";
    //                        "};$('body').removeClass('modal-open');$('.modal-backdrop').remove();";





    //    string Concatena2;
    //    if (clienteAtendimento != "")
    //    {
    //        Concatena2 = " AND A.CODIGO= " + clienteAtendimento + " ";
    //    }
    //    else
    //    {
    //        Concatena2 = "";
    //    }




    //    DataTable PesquisaAtedimento = Utilitarios.Pesquisar("SELECT P.NOME AS AVALIACAO, COUNT(*) AS QTD_AVALIACAO " +
    //            "FROM EXTRANET.PESQUISA_SATISFACAO_PAINEL E " +
    //            "JOIN ASSESSORES A ON A.CODIGO = E.CLIENTE_ID " +
    //            "JOIN EXTRANET.PESQUISA_SATISFACAO_PAINEL_RANKS P ON P.ID = E.AVALIACAO_ATENDIMENTO " +
    //            "WHERE MONTH(E.DATA) =" + data.Month.ToString() + " AND YEAR(E.DATA) =" + data.Year.ToString() + Concatena2 +
    //            "GROUP BY P.NOME ORDER BY COUNT(*) DESC");

    //    string aux4 = "['NOME', 'TOTAL'],";

    //    for (int i = 0; i < PesquisaAtedimento.Rows.Count; i++)
    //    {
    //        //aux4 += "['" + PesquisaAtedimento.Rows[i]["AVALIACAO"].ToString() + "'," + PesquisaAtedimento.Rows[i]["QTD_AVALIACAO"].ToString() + "],";

    //        aux4 += "['" + PesquisaAtedimento.Rows[i]["AVALIACAO"].ToString() + " - " + PesquisaAtedimento.Rows[i]["QTD_AVALIACAO"].ToString() + "' ," + PesquisaAtedimento.Rows[i]["QTD_AVALIACAO"].ToString() + "],";

    //    }

    //    aux4 = aux4.Remove(aux4.Length - 1);

    //    string javascript4 =
    //                        "google.charts.setOnLoadCallback(drawChart4);" +
    //                        "function drawChart4(){" +
    //                        "var data4 = google.visualization.arrayToDataTable([" +
    //                        aux4 +
    //                        "]);" +
    //                        "var options4 = { title: 'Resultado da Pesquisa de Satisfação do Atendimento',width: 800, height: 500, 'chartArea': {'width': '80%', 'height': '80%'}}; " +
    //                        "var chart4 = new google.visualization.PieChart(document.getElementById('piechart2')); " +
    //                        "chart4.draw(data4, options4);" +

    //                        "};$('body').removeClass('modal-open');$('.modal-backdrop').remove();";





    //    // ScriptManager.RegisterStartupScript(this, GetType(), "", javascript4 , true);

    //    if (acao == true)
    //    {
    //        ScriptManager.RegisterStartupScript(this, GetType(), "", javascript + javascript2 + javascript3 + javascript4 + "$('body').removeClass('modal-open');$('.modal-backdrop').remove();$(\"#modalRelatorioEstrategico\").modal(\"show\");", true);

    //    }
    //    else
    //    {
    //        ScriptManager.RegisterStartupScript(this, GetType(), "", javascript + javascript2 + javascript3 + javascript4, true);
    //    }

    //}

    public class CorGraficos
    {
        private static Random random = new Random();

        public string RetornaNomeCor()
        {
            int r = int.Parse(random.Next(255).ToString());
            int g = int.Parse(random.Next(255).ToString());
            int b = int.Parse(random.Next(255).ToString());
            int a = int.Parse(random.Next(255).ToString());
            string colorName = Color.FromArgb(a, r, g, b).Name.ToUpper();
            string cor = string.Format("#{0}", colorName.Substring(0, colorName.Length >= 6 ? 6 : colorName.Length));
            return cor;
        }
    }

    protected void BotaoOcultoRelatorio_Click(object sender, EventArgs e)
    {
        CarregaClientes();

        DateTime data = DateTime.Now.AddMonths(-1);
        txtMes.SelectedValue = data.Month.ToString();

        //ExplodeGrafico(false, "", "", "", "");
        ExplodeGraficoConcluido(false, "");
        ExplodeGraficoPendente(false, "");
        ExplodeGraficoSistema(false, "");
        ExplodeGraficoAtendimento(false, "");
        ChamaRelatorio(false);
        ScriptManager.RegisterStartupScript(this, GetType(), "mdl", "$(\"#modalRelatorioEstrategico\").modal(\"show\");", true);
    }

    protected void CarregaClientes()
    {
        DataTable dt = Utilitarios.Pesquisar("SELECT E.NOME_FANTASIA,A.CODIGO FROM ASSESSORES A JOIN EXTRANET.CLIENTE E ON E.ID = A.LOGIN WHERE A.ATIVO = 'S' AND E.ATIVO = 'S' AND A.USUARIO_TIPO = '1' ORDER BY E.NOME_FANTASIA");
        DataTable dt2 = Utilitarios.Pesquisar("SELECT E.NOME_FANTASIA,A.CODIGO FROM ASSESSORES A JOIN EXTRANET.CLIENTE E ON E.ID = A.LOGIN WHERE A.ATIVO = 'S' AND E.ATIVO = 'S' AND A.USUARIO_TIPO = '1' AND A.CODIGO IN (SELECT CLIENTE_ID FROM EXTRANET.PESQUISA_SATISFACAO_PAINEL) ORDER BY E.NOME_FANTASIA");
        Utilitarios.AtualizaDropDown(txtcliente, dt, "NOME_FANTASIA", "CODIGO", "SELECIONE");
        Utilitarios.AtualizaDropDown(txtclienteConcluido, dt, "NOME_FANTASIA", "CODIGO", "SELECIONE");
        Utilitarios.AtualizaDropDown(txtclientePesqSistema, dt2, "NOME_FANTASIA", "CODIGO", "SELECIONE");
        Utilitarios.AtualizaDropDown(txtclientePesqAtendimento, dt2, "NOME_FANTASIA", "CODIGO", "SELECIONE");
    }

    protected void txtMes_SelectedIndexChanged(object sender, EventArgs e)
    {
        //ExplodeGrafico(true, txtcliente.SelectedValue.ToString(), txtclienteConcluido.SelectedValue.ToString(), txtclientePesqSistema.SelectedValue.ToString(), txtclientePesqAtendimento.SelectedValue.ToString());
        ExplodeGraficoConcluido(true, txtclienteConcluido.SelectedValue.ToString());
        ExplodeGraficoPendente(true, txtcliente.SelectedValue.ToString());
        ExplodeGraficoSistema(true, txtclientePesqSistema.SelectedValue.ToString());
        ExplodeGraficoAtendimento(true, txtclientePesqAtendimento.SelectedValue.ToString());

    }

    protected void btnOcultoModuloUtilizados_Click(object sender, EventArgs e)
    {
        DataTable dt = Utilitarios.Pesquisar("SELECT E.NOME_FANTASIA,E.CLIENTE_ID FROM ASSESSORES A JOIN EXTRANET.CLIENTE E ON E.ID = A.LOGIN WHERE A.ATIVO = 'S' AND E.ATIVO = 'S' AND A.USUARIO_TIPO = '1' AND A.NOME_ASSOCIADO NOT LIKE '%AGENDA%' ORDER BY E.NOME_FANTASIA");
        Utilitarios.AtualizaDropDown(dropModulosUtilizados, dt, "NOME_FANTASIA", "CLIENTE_ID", "SELECIONE");
        ScriptManager.RegisterStartupScript(this, GetType(), "mdl", "$(\"#modalModulosUtilizados\").modal(\"show\");", true);
    }

    protected void btnGerarModulosUtilizados_Click(object sender, EventArgs e)
    {
        if (dropModulosUtilizados.SelectedValue != string.Empty)
        {

            string where = " 1=1 ";

            if (chkSisprev.Checked && ChkIntegra.Checked == false)
            {
                where += " AND TIPO= 1 ";
            }
            else if (chkSisprev.Checked == false && ChkIntegra.Checked)
            {
                where += " AND TIPO= 2 ";
            }

            DataTable dt = Utilitarios.Pesquisar("SELECT MODULO,CONTRATADO,UTILIZADO FROM DBO.RETONRA_MODULOS_UTILIZADOS(" + dropModulosUtilizados.SelectedValue.ToString() + ") WHERE" + where);

            Utilitarios.geraExcel(dt, "Modulos", dt.TableName);
            ScriptManager.RegisterStartupScript(this, GetType(), "mdl", "$(\"#modalModulosUtilizados\").modal(\"show\");", true);

        }
        else
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "", "alert('Selecione um Cliente!');", true);
            ScriptManager.RegisterStartupScript(this, GetType(), "mdl", "$(\"#modalModulosUtilizados\").modal(\"show\");", true);
        }
    }

    void ConsultaCrpAnoCorrente()
    {
        if (Utilitarios.Exec_StringSql_Return("SELECT COUNT(*) FROM EXTRANET.COLABORADOR_CRP WHERE COLABORADOR_ID = " + DadosUsuarioLogado.COLABORADOR_ID + " AND ANO = " + DateTime.Now.Year + " AND DADOS IS NOT NULL") != "0")
        {
            divCrp.Visible = true;
            lblAnoCrp.Text = DateTime.Now.Year.ToString();
        }
    }

    protected void btnBaixarCrp_Click(object sender, EventArgs e)
    {
        var dt = Utilitarios.Pesquisar("SELECT DADOS FROM EXTRANET.COLABORADOR_CRP WHERE COLABORADOR_ID = " + DadosUsuarioLogado.COLABORADOR_ID + " AND ANO = " + DateTime.Now.Year);

        if (dt.Rows.Count == 0)
        {
            ScriptManager.RegisterClientScriptBlock(this, GetType(), "msg", "<script>alert('Comprovante de Rendimentos não Encontrado!');</script>", false);
        }

        Response.ContentType = "application/force-download";
        Response.AddHeader("content-disposition", "attachment; filename=Comprovante de Rendimentos Pagos - " + DateTime.Now.Year + ".pdf");
        Response.AddHeader("content-type", "application/pdf");
        Response.AddHeader("content-length", ((byte[])dt.Rows[0]["DADOS"]).Length.ToString());
        Response.BinaryWrite((byte[])dt.Rows[0]["DADOS"]);
        Response.End();
    }

    // Gamification Conhecendo os Colegas - 189177
    void ConhecendoOsColegas()
    {
        if (Session["GAMIFICATION_COMPLETED"] != null)
        {
            return;
        }

        Page.Form.DefaultButton = btnGamification.UniqueID;

        bool jaParticipou = Utilitarios.Exec_StringSql_Return("SELECT COUNT(*) FROM EXTRANET.GamificationConhecendoColegas" +
                " WHERE CAST(data AS DATE) = CAST(GETDATE() AS DATE)" +
                " AND colaborador_inseriu = " + DadosUsuarioLogado.COLABORADOR_ID) != "0";

        if (Session["VISTO_GAMIFICATION"] != null)
        {
            //Recarregou a página, marca como tentativa incorreta
            Session["GAMIFICATION_COMPLETED"] = true;

            if (!jaParticipou)
            {
                Utilitarios.Exec_StringSql("INSERT INTO EXTRANET.GamificationConhecendoColegas (data, colaborador_inseriu, colaborador_apareceu, acertou) " +
                    "VALUES (getdate(), " + DadosUsuarioLogado.COLABORADOR_ID + " , " + Session["GAMIFICATION_COLABORADOR_ID"] + ", 'N')");

                ConhecendoOsColegasOpcaoIncorreta();
            }
            return;
        }

        if (jaParticipou)
        {
            Session["VISTO_GAMIFICATION"] = true;
            return;
        }

        //pega colaborador aleatório
        var sql = @"SELECT TOP 1 EC.COLABORADOR_ID, EC.NOME, EC.BIOGRAFIA, EC.FOTO, S.NOME AS SETOR, CURR.SEXO, C.NOME AS CARGO FROM EXTRANET.COLABORADOR EC
            JOIN EXTRANET.SETOR S ON EC.SETOR_ID = S.SETOR_ID AND S.SETOR_ID <> " + DadosUsuarioLogado.SETOR_ID.ToString() + @"
            JOIN EXTRANET.CURRICULO CURR ON EC.COLABORADOR_ID = CURR.COLABORADOR_ID
            JOIN EXTRANET.CARGO C ON C.CARGO_ID = EC.CARGO_ID
            WHERE EC.ATIVO='S' 
            AND EC.COLABORADOR_ID NOT IN (0, " + DadosUsuarioLogado.COLABORADOR_ID + @")
            AND EC.NOME NOT LIKE 'FILA %' 
            AND COALESCE(EC.FOTO,'') <> ''
            AND CURR.SEXO IS NOT NULL
            ORDER BY NEWID()";

        var dt = Utilitarios.Pesquisar(sql);
        if (dt == null || dt.Rows.Count < 1)
        {
            return;
        }

        string htmlFoto = "<div class='img-circle' " +
                "style='background-image: url(\"https://agendaextranet.com.br/novaextranet/Library/Colaborador_Foto/" + dt.Rows[0]["FOTO"] + "\"); " +
                    "width: 145px; " +
                    "height: 145px; " +
                    "background-size: cover; " +
                    "background-position: top; " +
                    "background-position: center; " +
                    "border: 3px solid #CCC; " +
                    "background-repeat: no-repeat; " +
                    "margin: 0 auto;'>" +
            "</div>";

        ltGamificationFoto.Text = htmlFoto;

        lblGamificationSetor.Text = dt.Rows[0]["SETOR"].ToString();

        string biografia = dt.Rows[0]["BIOGRAFIA"].ToString();
        if (string.IsNullOrEmpty(biografia))
            biografia = "Biografia ainda não preenchida";

        lblBiografia.Text = biografia;

        string nomeColaborador = dt.Rows[0]["NOME"].ToString().ToUpper();

        if (nomeColaborador.StartsWith("ANA ") || nomeColaborador.StartsWith("JOÃO "))
            nomeColaborador = string.Join(" ", nomeColaborador.Split(' ').Take(2));
        else
            nomeColaborador = nomeColaborador.Split(' ').First();

        string sqlNomes = "SELECT * FROM (SELECT '" + nomeColaborador + "' AS NOME UNION SELECT * FROM (SELECT TOP 2 SUBSTRING(UPPER(EC.NOME), 1, CHARINDEX(' ', UPPER(EC.NOME), 1)) AS NOME FROM EXTRANET.COLABORADOR EC " +
           "JOIN EXTRANET.CURRICULO CURRICULO ON EC.COLABORADOR_ID = CURRICULO.COLABORADOR_ID " +
           "WHERE 1=1 " +
           "AND EC.ATIVO='S' " +
           "AND EC.COLABORADOR_ID NOT IN(0) " +
           "AND EC.NOME NOT LIKE 'FILA %' " +
           "AND CURRICULO.SEXO = '" + dt.Rows[0]["SEXO"].ToString() + "' AND EC.COLABORADOR_ID <>" + dt.Rows[0]["COLABORADOR_ID"].ToString() + " " +
           "AND EC.NOME NOT LIKE '" + nomeColaborador + "%' ORDER BY NEWID()) AS T) AS T1 ORDER BY NEWID()";

        DataTable opcoes = Utilitarios.Pesquisar(sqlNomes);

        rdblGamificationOpcoes.DataValueField = "NOME";
        rdblGamificationOpcoes.DataTextField = "NOME";
        rdblGamificationOpcoes.DataSource = opcoes;
        rdblGamificationOpcoes.DataBind();

        Session["GAMIFICATION_NOME"] = dt.Rows[0]["NOME"].ToString();
        Session["GAMIFICATION_NOME_ABREV"] = nomeColaborador;
        Session["GAMIFICATION_SETOR"] = dt.Rows[0]["SETOR"].ToString();
        Session["GAMIFICATION_COLABORADOR_ID"] = dt.Rows[0]["COLABORADOR_ID"].ToString();
        Session["GAMIFICATION_CARGO"] = dt.Rows[0]["CARGO"].ToString();
        Session["GAMIFICATION_FOTO"] = htmlFoto;
        Session["GAMIFICATION_BIOGRAFIA"] = biografia;
        Session["VISTO_GAMIFICATION"] = true;

        if (Session["GAMIFICATION_START_DATE"] == null)
            Session["GAMIFICATION_START_DATE"] = DateTime.Now;

        ScriptManager.RegisterStartupScript(this, GetType(), "modal", "mostraGamification();", true);
    }

    //Função que gerencia exibição dos modais da extranet
    void GerenciarModais()
    {
        MensagemMotivacao();

        if ((PrimeiroDiaUtilMes() || DateTime.Now.Date == Convert.ToDateTime("03/03/2022").Date
             || DateTime.Now.Date == Convert.ToDateTime("04/05/2023").Date) && Session["VISTO_PONTUACAO_GAMIFICATION"] == null)
        {
            PontuacaoConhecendoOsColegas(false);
        }
        // Primeiro dia do Ano
        else if (DateTime.Now.Date == Convert.ToDateTime("02/01/2024").Date && Session["VISTO_PONTUACAO_GAMIFICATION_GERAL"] == null)
        {
            PontuacaoConhecendoOsColegas(true);
        }
        else
        {
            ConhecendoOsColegas();
        }
    }

    bool PrimeiroDiaUtilMes()
    {
        var data = DateTime.Now;
        var primeiroDiaUtilMes = new DateTime(data.Year, data.Month, 1);

        bool diaUtil = false;

        while (!diaUtil)
        {
            if (primeiroDiaUtilMes.DayOfWeek == DayOfWeek.Saturday || primeiroDiaUtilMes.DayOfWeek == DayOfWeek.Sunday)
            {
                primeiroDiaUtilMes = primeiroDiaUtilMes.AddDays(1);
            }
            else
            {
                break;
            }
        }

        return data.Date == primeiroDiaUtilMes.Date;
    }

    void PontuacaoConhecendoOsColegas(bool geral)
    {
        string sql = "";

        if (geral)
        {
            Session["VISTO_PONTUACAO_GAMIFICATION_GERAL"] = true;
            sql = "EXEC GamificationConhecendoColegasGERAL " + Convert.ToString(DateTime.Now.Year - 1);
            ltTopoColegas.Text = "<p style='color:#0000FF;font-size:30px !important;'><b>Ranking Anual " + Convert.ToString(DateTime.Now.Year - 1) + "</b></p>";
            divModalTotal.Attributes.Add("style", "background-color:#e0e0e0");
        }
        else
        {
            Session["VISTO_PONTUACAO_GAMIFICATION"] = true;

            sql = @"EXEC GamificationConhecendoColegasMES";

            ltTopoColegas.Text = "<p>Ranking</p>";
        }

        var dt = Utilitarios.Pesquisar(sql);

        for (int i = 0; i < dt.Rows.Count; i++)
        {
            if (i > 9 && Convert.ToInt32(dt.Rows[i]["COLABORADOR_ID"]) != DadosUsuarioLogado.COLABORADOR_ID)
                dt.Rows[i].Delete();
        }

        dt.AcceptChanges();

        rptPontuacao.DataSource = dt;
        rptPontuacao.DataBind();

        if (hddAvaliacaoId.Value == "")
            ScriptManager.RegisterStartupScript(this, GetType(), "gmf_pontuacao", "$('#modal-pontuacao-gamification').modal('show'); ", true);
    }

    protected void btnGamification_Click(object sender, EventArgs e)
    {
        if (Session["GAMIFICATION_COMPLETED"] != null)
            return;

        if (Session["GAMIFICATION_COLABORADOR_ID"] == null || Session["GAMIFICATION_NOME"] == null)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "modal", "alert('Não foi possível salvar sua tentativa, tente novamente mais tarde!');", true);
            return;
        }

        string nomeSelecionou = rdblGamificationOpcoes.SelectedValue;

        if (string.IsNullOrWhiteSpace(nomeSelecionou))
        {
            nomeSelecionou = "VAZIO";
            //Utilitarios.Alerta(this.Page, "Atenção", "É necessário escolher uma opção!");
            //ScriptManager.RegisterStartupScript(this, GetType(), "gmf", "$('#modal-gamification').modal('show'); alert('');", true);
            //return;
        }

        Session["GAMIFICATION_COMPLETED"] = true;

        bool acertou = Session["GAMIFICATION_NOME_ABREV"].ToString() == nomeSelecionou;

        bool inseriu = Utilitarios.Exec_StringSql_Return("SELECT COUNT(*) FROM EXTRANET.GamificationConhecendoColegas" +
                " WHERE CAST(data AS DATE) = CAST(GETDATE() AS DATE)" +
                " AND colaborador_inseriu = " + DadosUsuarioLogado.COLABORADOR_ID) != "0";

        if (!inseriu)
        {
            string sqlCmd = "INSERT INTO EXTRANET.GamificationConhecendoColegas (data, data_iniciou, colaborador_inseriu, colaborador_apareceu, acertou)" +
                " VALUES (@DATA, @DATA_INICIOU, @COLABORADOR_INSERIU, @COLABORADOR_APARECEU, @ACERTOU)";

            using (var conn = Utilitarios.GetOpenConnection())
            using (var cmd = new SqlCommand(sqlCmd, conn))
            {
                cmd.Parameters.Add("@DATA", SqlDbType.DateTime).Value = DateTime.Now;
                cmd.Parameters.Add("@DATA_INICIOU", SqlDbType.DateTime).Value = (DateTime)Session["GAMIFICATION_START_DATE"];
                cmd.Parameters.Add("@COLABORADOR_INSERIU", SqlDbType.Int).Value = DadosUsuarioLogado.COLABORADOR_ID;
                cmd.Parameters.Add("@COLABORADOR_APARECEU", SqlDbType.Int).Value = Convert.ToInt32(Session["GAMIFICATION_COLABORADOR_ID"]);
                cmd.Parameters.Add("@ACERTOU", SqlDbType.VarChar).Value = acertou ? "S" : "N";

                cmd.ExecuteNonQuery();
            }
        }

        if (acertou)
        {
            btnFecharModalGamification.Visible = true;
            divGamificationOpcoes.Visible = false;
            btnEnviarGamification.Visible = false;
            divBiografia.Visible = true;
            lblGamificationNome.Text = Session["GAMIFICATION_NOME"].ToString();
            lblCargo.Text = Session["GAMIFICATION_CARGO"].ToString();

            ScriptManager.RegisterStartupScript(this, GetType(), "gmf", "$('#modal-gamification').modal('show');", true);
            Utilitarios.Alerta(this.Page, "Muito bem!", "Você acertou o nome do colega", "success");
        }
        else
        {
            ConhecendoOsColegasOpcaoIncorreta();
        }
    }

    void ConhecendoOsColegasOpcaoIncorreta()
    {
        btnFecharModalGamification.Visible = true;
        divGamificationOpcoes.Visible = false;
        btnEnviarGamification.Visible = false;
        divBiografia.Visible = true;

        lblGamificationNome.Text = Session["GAMIFICATION_NOME"].ToString();
        ltGamificationFoto.Text = Session["GAMIFICATION_FOTO"].ToString();
        lblGamificationSetor.Text = Session["GAMIFICATION_SETOR"].ToString();
        lblBiografia.Text = Session["GAMIFICATION_BIOGRAFIA"].ToString();
        lblCargo.Text = Session["GAMIFICATION_CARGO"].ToString();

        ScriptManager.RegisterStartupScript(this, GetType(), "gmf", "$('#modal-gamification').modal('show');", true);
        Utilitarios.Alerta(this.Page, "Atenção", "Resposta incorreta, busque conhecer o colega!");
    }

    protected void btnGamificationExpirou_Click(object sender, EventArgs e)
    {
        if (Session["GAMIFICATION_COMPLETED"] != null)
            return;

        if (Session["GAMIFICATION_COLABORADOR_ID"] == null || Session["GAMIFICATION_NOME"] == null)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "modal", "alert('Não foi possível salvar sua tentativa, tente novamente mais tarde!');", true);
            return;
        }

        Utilitarios.Exec_StringSql("INSERT INTO EXTRANET.GamificationConhecendoColegas (data, colaborador_inseriu, colaborador_apareceu, acertou) " +
            "VALUES (getdate(), " + DadosUsuarioLogado.COLABORADOR_ID + " , " + Session["GAMIFICATION_COLABORADOR_ID"] + ", 'N')");

        Session["GAMIFICATION_COMPLETED"] = true;

        btnFecharModalGamification.Visible = true;
        divGamificationOpcoes.Visible = false;
        btnEnviarGamification.Visible = false;
        divBiografia.Visible = true;
        lblGamificationNome.Text = Session["GAMIFICATION_NOME"].ToString();

        ScriptManager.RegisterStartupScript(this, GetType(), "gmf", "$('#modal-gamification').modal('show');", true);
        Utilitarios.Alerta(this.Page, "Atenção", "Prazo expirado, busque conhecer o colega!");
    }

    protected void lnkFecharPontuacao_Click(object sender, EventArgs e)
    {
        GerenciarModais();
    }

    void Termo()
    {
        if (Utilitarios.Pesquisar("select * from termo_aceite where colaborador_id=" + DadosUsuarioLogado.COLABORADOR_ID).Rows.Count == 0 && Session["TERMO_VISTO"] == null)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "modaltermo", "$('#modalTermo').modal('show', { backdrop: 'static', keyboard: false });;", true);
            Session["TERMO_VISTO"] = "S";
        }
    }

    protected void btnConfirmarTermo_Click(object sender, EventArgs e)
    {
        string senhaDigitada = Utilitarios.Criptografar(txtSenhaTermo.Text).ToLower();
        string senhaCadastrada = Utilitarios.Exec_StringSql_Return("select senhatrm from assessores where codigo=" + DadosUsuarioLogado.CODIGO).ToLower();

        if (chkTermo.Checked == false)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "modaltermo1", "$('#modalTermo').modal('show', { backdrop: 'static', keyboard: false });alert('Favor marque o check de aceitação do termo!');", true);
        }
        else if (senhaDigitada != senhaCadastrada)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "modaltermo2", "$('#modalTermo').modal('show', { backdrop: 'static', keyboard: false });alert('Senha inválida!');", true);
        }
        else
        {
            Utilitarios.Exec_StringSql("insert into termo_aceite (colaborador_id,nome,setor,data_hora) values (" + DadosUsuarioLogado.COLABORADOR_ID + ", '" + DadosUsuarioLogado.NOME_COLABORADOR +
                "', '" + DadosUsuarioLogado.SETOR + "', getdate())");

            ScriptManager.RegisterStartupScript(this, GetType(), "aceitetermo", "alert('Aceite do Termo de Compromisso registrado com sucesso, obrigado!');", true);
        }
    }

    protected void btnFecharModalGamification_Click(object sender, EventArgs e)
    {
        Termo();
    }

    protected void lnkTermo_Click(object sender, EventArgs e)
    {
        if (Utilitarios.Pesquisar("select * from termo_aceite where colaborador_id=" + DadosUsuarioLogado.COLABORADOR_ID).Rows.Count == 0)
        {
            divConfTermo.Visible = true;
            divFechaTermo.Visible = false;
        }
        else
        {
            divConfTermo.Visible = false;
            divFechaTermo.Visible = true;
        }
        ScriptManager.RegisterStartupScript(this, GetType(), "modaltermovis", "$('#modalTermo').modal('show', { backdrop: 'static', keyboard: false });", true);



    }

    protected void lnkAvaliacoes_Click(object sender, EventArgs e)
    {
        ConsultaAvaliacao("false");
    }


    private void ConsultaAvaliacao(string inicio)
    {
        DataTable dt = Utilitarios.Pesquisar(@"SELECT TOP 1 A.NOME AS TITULO, A.DESCRICAO,  P.* 
        FROM EXTRANET.AVALIACAO_PERGUNTA P JOIN EXTRANET.AVALIACAO A ON A.AVALIACAO_ID = P.AVALIACAO_ID
        WHERE CAST(GETDATE() AS DATE) BETWEEN A.DATA_INI AND A.DATA_FIM AND A.ATIVO = 1
             AND P.AVALIACAO_PERGUNTA_ID NOT IN(SELECT AVALIACAO_PERGUNTA_ID FROM EXTRANET.AVALIACAO_RESPOSTA R WHERE R.COLABORADOR_ID = " + DadosUsuarioLogado.COLABORADOR_ID + @")
         ORDER BY A.AVALIACAO_ID, P.NOME");

        if (dt.Rows.Count > 0)
        {
            hddAvaliacaoId.Value = dt.Rows[0]["AVALIACAO_ID"].ToString();
            ltTituloAvaliacao.Text = dt.Rows[0]["TITULO"].ToString();
            ltSubTituloAvaliacao.Text = dt.Rows[0]["DESCRICAO"].ToString();

            ltPergunta.Text = dt.Rows[0]["NOME"].ToString();
            hddAvaliacaoPerguntaId.Value = dt.Rows[0]["AVALIACAO_PERGUNTA_ID"].ToString();

            string wheretipo = "WHERE AVALIACAO_TIPO_ID < 5";

            if (dt.Rows[0]["TIPO"].ToString() == "2")
                wheretipo = "WHERE AVALIACAO_TIPO_ID  > 4";

            DataTable dtT = Utilitarios.Pesquisar("SELECT AVALIACAO_TIPO_ID, NOME + '<br><img src=''Library/Images/Icon'+ replace(nome,'Ó','O') +'.png''>' AS NOME FROM EXTRANET.AVALIACAO_TIPO " + wheretipo + " ORDER BY 1");
            rdblTipos.DataSource = dtT;
            rdblTipos.DataValueField = "AVALIACAO_TIPO_ID";
            rdblTipos.DataTextField = "NOME";
            rdblTipos.DataBind();

            ScriptManager.RegisterStartupScript(this, GetType(), "modaltermovis", "$('#modalAvaliacoes').modal('show', { backdrop: 'static', keyboard: false });", true);
        }
        else
        {
            if (inicio == "false")
            {
                Utilitarios.Alerta(this.Page, "Atenção", "Obrigado, você concluiu seu processo de avaliação!");
                ScriptManager.RegisterStartupScript(this, GetType(), "modaltermovis", "$('#modalAvaliacoes').modal('hide', { backdrop: 'static', keyboard: false });", true);
            }
        }

        Session["AVALIACAO_VISTO"] = "S";

    }

    protected void btnAvaliar_Click(object sender, EventArgs e)
    {
        if (rdblTipos.SelectedValue == "")
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "modaltermovis", "$('#modalAvaliacoes').modal('show', { backdrop: 'static', keyboard: false });", true);
            Utilitarios.Alerta(this.Page, "Atenção", "Selecione um tipo para avaliar!");
            return;
        }

        int existe = Utilitarios.Pesquisar("SELECT * FROM EXTRANET.AVALIACAO_RESPOSTA WHERE AVALIACAO_PERGUNTA_ID=" + hddAvaliacaoPerguntaId.Value +
            " AND COLABORADOR_ID=" + DadosUsuarioLogado.COLABORADOR_ID).Rows.Count;

        if (existe == 0)
        {
            Utilitarios.Exec_StringSql("INSERT INTO EXTRANET.AVALIACAO_RESPOSTA (AVALIACAO_PERGUNTA_ID, AVALIACAO_TIPO_ID, COLABORADOR_ID) VALUES (" + hddAvaliacaoPerguntaId.Value +
                "," + rdblTipos.SelectedValue + ", " + DadosUsuarioLogado.COLABORADOR_ID + ")");
        }

        lnkAvaliacoes_Click(null, null);
    }
}

