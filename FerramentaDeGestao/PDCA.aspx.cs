using FerramentaDeGestao.WebService;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Optimization;
using System.Web.UI;
using System.Web.UI.WebControls;
using static FerramentaDeGestao.WebService.ParticipanteService;

namespace FerramentaDeGestao
{
    public partial class PDCA : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindPDCARepeater();
                if (Session["Participantes"] != null)
                {
                    List<string> participantes = (List<string>)Session["Participantes"];
                    rptParticipantes.DataSource = participantes;
                    rptParticipantes.DataBind();

                    Session.Remove("Participantes");
                }
            }
        }

        private void BindPDCARepeater()
        {
            DataTable dt = GetPDCADetails();

            rptPDCA.DataSource = dt;
            rptPDCA.DataBind();
        }

        private DataTable GetPDCADetails()
        {
            DataTable dt = new DataTable();
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SISPREVConnectionString"].ConnectionString;

            using (SqlConnection conn  = new SqlConnection(connectionString))
            {
                string query = 
                    "SELECT Plano, PRAZO_PLANO, Desempenhar, PRAZO_DESEMPENHAR, Checar, PRAZO_CHECAR, Acao, PRAZO_ACAO, Participantes FROM PDCA;";
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                da.Fill(dt);
            }

            if (dt.Rows.Count == 0)
            {
                dt.Rows.Add("Definir metas e objetivos", "01/01/2025", "Implementar ações", "08/01/2025", "Avaliar resultados", "15/01/2025", "Corrigir desvios", "22/01/2025", "Equipe A");
                //dt.Rows.Add("Planejamento estratégico", "Execução de tarefas", "Monitoramento de desempenho", "Ajustes necessários", "Equipe B");
            }

            return dt;
        }

        protected void btnAdicionar_Click(object sender, EventArgs e)
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SISPREVConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SET DATEFORMAT DMY; INSERT INTO PDCA (Plano, PRAZO_PLANO, Desempenhar, PRAZO_DESEMPENHAR, Checar, PRAZO_CHECAR, Acao, PRAZO_ACAO, Participantes)" +
                    "VALUES (@Plano, @PRAZO_PLANO, @Desempenhar, @PRAZO_DESEMPENHAR, @Checar, @PRAZO_CHECAR, @Acao, @PRAZO_ACAO, @Participantes);";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Plano", txtPlano.Value);
                    cmd.Parameters.AddWithValue("@PRAZO_PLANO", dataPlano.Value);
                    cmd.Parameters.AddWithValue("@Desempenhar", txtDesempenhar.Value);
                    cmd.Parameters.AddWithValue("@PRAZO_DESEMPENHAR", dataDesempenhar.Value);
                    cmd.Parameters.AddWithValue("@Checar", txtChecar.Value);
                    cmd.Parameters.AddWithValue("@PRAZO_CHECAR", dataChecar.Value);
                    cmd.Parameters.AddWithValue("@Acao", txtAcao.Value);
                    cmd.Parameters.AddWithValue("@PRAZO_ACAO", dataAcao.Value);
                    cmd.Parameters.AddWithValue("@Participantes", rptParticipantes.ToString());

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            
            BindPDCARepeater();

            ClearForm();
        }

        private void ClearForm()
        {
            txtPlano.Value = string.Empty;
            txtDesempenhar.Value = string.Empty;
            txtChecar.Value = string.Empty;
            txtAcao.Value = string.Empty;
        }

        protected void gdrResultados_ItemCreated(object sender, DataGridItemEventArgs e)
        {

        }

        protected void gdrResultados_ItemDataBound(object sender, DataGridItemEventArgs e)
        {

        }

        protected void gdrResultados_ItemCommand(object source, DataGridCommandEventArgs e)
        {

        }

        protected void btnConsultar_Click(object sender, EventArgs e)
        {
            
        }
    }
}