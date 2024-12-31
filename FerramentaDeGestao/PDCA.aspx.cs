using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FerramentaDeGestao
{
    public partial class PDCA : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindPDCARepeater();
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
                string query = "SELECT Plano, Desempenhar, Checar, Acao, Participantes FROM PDCA;";
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                da.Fill(dt);
            }

            if (dt.Rows.Count == 0)
            {
                dt.Rows.Add("Definir metas e objetivos", "Implementar ações", "Avaliar resultados", "Corrigir desvios", "Equipe A");
                dt.Rows.Add("Planejamento estratégico", "Execução de tarefas", "Monitoramento de desempenho", "Ajustes necessários", "Equipe B");
            }

            return dt;
        }

        protected void btnAdicionar_Click(object sender, EventArgs e)
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SISPREVConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO PDCA (Plano, Desempenhar, Checar, Acao, Participantes)" +
                    "VALUE (@Plano, @Desempenhar, @Checar, @Acao, @Participantes);";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Plano", txtPlano.Value);
                    cmd.Parameters.AddWithValue("@Desempenhar", txtDesempenhar.Value);
                    cmd.Parameters.AddWithValue("@Checar", txtChecar.Value);
                    cmd.Parameters.AddWithValue("@Acao", txtAcao.Value);
                    cmd.Parameters.AddWithValue("@Participantes", txtParticipantes.Value);

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
            txtParticipantes.Value = string.Empty;
        }
    }
}