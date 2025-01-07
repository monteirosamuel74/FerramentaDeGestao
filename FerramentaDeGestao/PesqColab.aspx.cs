using FerramentaDeGestao.WebService;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FerramentaDeGestao
{
    public partial class PesqColab : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            txtPesqColab.Focus();
            if (!IsPostBack)
            {

            }
        }

        protected void btnPesqColab_Click(object sender, EventArgs e)
        {
            string nome = txtPesqColab.Text.ToUpper();
            //ParticipanteService service = new ParticipanteService();
            var participantes = Pesquisar(nome);
            if (participantes.Rows.Count > 0) 
            {
                grdResultados.DataSource = participantes;
                grdResultados.DataBind();
            }
            else
                ScriptManager.RegisterClientScriptBlock(
                    this,
                    GetType(),
                    "MSG",
                    "<script>alert('Não foram encontrados registros para a pesquisa realizada.');</script>",
                    false);
        }

        protected void grdResultados_ItemCreated(object sender, DataGridItemEventArgs e)
        {

        }

        protected void grdResultados_ItemDataBound(object sender, DataGridItemEventArgs e)
        {

        }

        protected void grdResultados_ItemCommand(object source, DataGridCommandEventArgs e)
        {

        }
        public static DataTable Pesquisar(string nome)
        {
            DataTable dt = new DataTable();
            string connectionString = WebConfigurationManager.ConnectionStrings["SISPREVConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string busca = "SELECT COLABORADOR_ID, Nome, Email FROM COLABORADOR2 WHERE Nome LIKE '"
                    + nome + "%'";
                using (SqlCommand cmd = new SqlCommand(busca, conn))
                {
                    SqlDataAdapter exec = new SqlDataAdapter();
                    exec.SelectCommand = cmd;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText += "SET DATEFORMAT DMY; ";
                    try
                    {
                        exec.Fill(dt);
                    }
                    finally
                    {
                        conn.Close();
                        conn.Dispose();
                        exec.Dispose();
                        cmd.Dispose();
                    }
                    return dt;
                }
            }
        }

        protected void btnInserir_Click(object sender, EventArgs e)
        {
            List<string> participantesSelecionados = new List<string>();
            foreach (DataGridItem item in grdResultados.Items)
            {
                CheckBox checkBox = (CheckBox)item.FindControl("chkSelecionar");
                if(checkBox != null && checkBox.Checked)
                {
                    string colabiradoId = item.Cells[1].Text;
                    string nome = item.Cells[3].Text;
                    string email = item.Cells[4].Text;
                    AdicionarParticipanteEscolhido(colabiradoId, nome, email);
                    participantesSelecionados.Add(nome);
                }
            }
        }

        private void AdicionarParticipanteEscolhido(string colaboradorId, string nome, string email)
        {
            List<string> participantesSelecionados = (List<string>)ViewState["Participantes"] ?? new List<string>();
            participantesSelecionados.Add(nome);
            ViewState["Participantes"] = participantesSelecionados;

            Session["Participantes"] = participantesSelecionados;
            ScriptManager.RegisterStartupScript(this, GetType(), "AtualizarPaginaPrincipal", "window.opener.atualizarParticipantes();", true);
        }
    }
}