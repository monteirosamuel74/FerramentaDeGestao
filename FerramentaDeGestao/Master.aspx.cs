using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

public partial class Master : System.Web.UI.Page
{
    private string _sql = string.Empty;
    private DataTable _dt = new DataTable();
    private readonly List<Pages> _paginas = new List<Pages>();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (Valida())
            {
                InsereBanco("Acesso Garantido");
                CarregaTela();
                ConsultaHistorico();
            }
            else
            {
                divParent.Visible = false;
                divAviso.Visible = true;
                LblAviso.Text = "Acessado por: <b>" + DadosUsuarioLogado.NOME_COLABORADOR + "</b> em " + DateTime.Now;

                //Enviar para o banco de dados quem tentou acessar
                InsereBanco("Tentou Acessar");
            }
        }
    }

    #region Validações de Acesso

    private bool Valida()
    {
        if (DadosUsuarioLogado.COLABORADOR_ID != 393 && DadosUsuarioLogado.COLABORADOR_ID != 18)
            return false;
        else return true;
    }

    private void InsereBanco(string tipo)
    {
        _sql = "INSERT INTO MASTER_ACESSO (COLABORADOR_ID, DATA, TIPO) VALUES (" + DadosUsuarioLogado.COLABORADOR_ID + ", GETDATE(), '" + tipo + "');";
        Utilitarios.Exec_StringSql(_sql);
    }

    #endregion

    #region Acesso as Páginas

    private void CarregaTela()
    {
        LblNome.Text = DadosUsuarioLogado.NOME_COLABORADOR;

        //Colaboradores
        _sql = "SELECT COLABORADOR_ID, UPPER(CONCAT(A.NOME, ' [', C.NOME, ']')) AS NOME FROM EXTRANET.COLABORADOR A " +
                "JOIN EXTRANET.SETOR B ON A.SETOR_ID = B.SETOR_ID " +
                "JOIN EXTRANET.CARGO C ON A.CARGO_ID = C.CARGO_ID " +
                "WHERE ATIVO = 'S' AND COLABORADOR_ID NOT IN(0, 836) ORDER BY A.NOME ASC";

        _dt = Utilitarios.Pesquisar(_sql);

        DropColaborador.DataSource = _dt;
        DropColaborador.DataValueField = "COLABORADOR_ID";
        DropColaborador.DataTextField = "NOME";
        DropColaborador.DataBind();
        DropColaborador.Items.Insert(0, new ListItem("Selecione"));
    }

    private void CarregarPagina()
    {
        ConsultaPaginas();

        //Prepara sessão de acesso
        Session["ACESSO_GARANTIDO"] = "$permite$" + DadosUsuarioLogado.COLABORADOR_ID;
        Session["ACESSO_MASTER"] = DropColaborador.SelectedValue;
        //Session["ACESSO_FUNC"] = DateTime.Now.Year.ToString();

        var page = string.Empty;

        foreach (var x in _paginas)
        {
            if (x.Opcao == Convert.ToInt32(DropPage.SelectedValue))
                page = x.Pagina;
        }

        if (!string.IsNullOrEmpty(page))
        {
            InsereBanco("Acessou registros de: " + DropColaborador.SelectedItem.Text);
            Response.Redirect(page);
        }
        else
            ScriptManager.RegisterStartupScript(this, GetType(), "", "alert('Erro interno de paginação');", true);
    }

    #endregion

    #region OnClick's

    protected void LnkConsultar_OnClick(object sender, EventArgs e)
    {
        CarregarPagina();
    }

    #endregion

    #region Histórico

    private void ConsultaHistorico()
    {
        _sql = "SELECT TOP 50 B.NOME, A.DATA, A.TIPO FROM MASTER_ACESSO A JOIN EXTRANET.COLABORADOR B ON A.COLABORADOR_ID = B.COLABORADOR_ID ORDER BY ID DESC";
        _dt = Utilitarios.Pesquisar(_sql);

        Session["consulta"] = _dt;
        CarregarGrid();
    }

    #endregion

    #region Classes e Construtores

    public class Pages
    {
        public int Opcao { get; set; }
        public string Pagina { get; set; }
    }

    private void ConsultaPaginas()
    {
        _paginas.Add(new Pages { Opcao = 0, Pagina = "WorkFlow.aspx" });
        _paginas.Add(new Pages { Opcao = 1, Pagina = "WorkFlowConsultas.aspx?tipo=1&nometipo=Enviadas" });
        _paginas.Add(new Pages { Opcao = 2, Pagina = "WorkFlowConsultas.aspx?tipo=0&nometipo=Recebidas" });
        _paginas.Add(new Pages { Opcao = 3, Pagina = "WorkFlowConsultas.aspx?tipo=3&nometipo=Arquivo" });
    }

    #endregion

    #region Paginacao

    private void CarregarGrid()
    {
        DataTable dt = (DataTable)Session["consulta"];

        pgsource.DataSource = dt.DefaultView;
        pgsource.AllowPaging = true;

        pgsource.PageSize = 15;
        pgsource.CurrentPageIndex = CurrentPage;
        ViewState["totpage"] = pgsource.PageCount;
        lnkPrevious.Enabled = !pgsource.IsFirstPage;
        lnkNext.Enabled = !pgsource.IsLastPage;
        lnkFirst.Enabled = !pgsource.IsFirstPage;
        lnkLast.Enabled = !pgsource.IsLastPage;
        RptAcessos.DataSource = pgsource;
        RptAcessos.DataBind();
        doPaging();

        if (dt.Rows.Count < 15)
            divPagina.Visible = false;
        else
            divPagina.Visible = true;
    }

    PagedDataSource pgsource = new PagedDataSource();

    int findex, lindex;

    private int CurrentPage
    {
        get
        {
            if (ViewState["CurrentPage"] == null)
            {
                return 0;
            }
            else
            {
                return ((int)ViewState["CurrentPage"]);
            }
        }
        set
        {
            ViewState["CurrentPage"] = value;
        }
    }

    private void doPaging()
    {
        DataTable dt = new DataTable();
        dt.Columns.Add("PageIndex");
        dt.Columns.Add("PageText");

        findex = CurrentPage - 5;

        if (CurrentPage > 4)
        {
            lindex = CurrentPage + 5;
        }
        else
        {
            lindex = 5;
        }

        if (lindex > Convert.ToInt32(ViewState["totpage"]))
        {
            lindex = Convert.ToInt32(ViewState["totpage"]);
            findex = lindex - 5;
        }

        if (findex < 0)
        {
            findex = 0;
        }

        for (int i = findex; i < lindex; i++)
        {
            DataRow dr = dt.NewRow();
            dr[0] = i;
            dr[1] = i;
            dt.Rows.Add(dr);
        }

        RepeaterPaging.DataSource = dt;
        RepeaterPaging.DataBind();
    }

    protected void lnkFirst_Click(object sender, EventArgs e)
    {
        CurrentPage = 0;
        CarregarGrid();
    }

    protected void lnkLast_Click(object sender, EventArgs e)
    {
        CurrentPage = (Convert.ToInt32(ViewState["totpage"]) - 1);
        CarregarGrid();
    }

    protected void lnkPrevious_Click(object sender, EventArgs e)
    {
        CurrentPage -= 1;
        CarregarGrid();
    }

    protected void lnkNext_Click(object sender, EventArgs e)
    {
        CurrentPage += 1;
        CarregarGrid();
    }

    protected void RepeaterPaging_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        if (e.CommandName.Equals("newpage"))
        {
            CurrentPage = Convert.ToInt32(e.CommandArgument.ToString());
            CarregarGrid();
        }
    }

    protected void RepeaterPaging_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        LinkButton lnkPage = (LinkButton)e.Item.FindControl("Pagingbtn");
        HtmlGenericControl li = (HtmlGenericControl)e.Item.FindControl("li");
        if (Convert.ToInt32(lnkPage.CommandArgument) == CurrentPage)
        {
            lnkPage.Enabled = false;
            li.Attributes.Remove("class");
            li.Attributes.Add("class", "active");
        }
    }

    #endregion
}