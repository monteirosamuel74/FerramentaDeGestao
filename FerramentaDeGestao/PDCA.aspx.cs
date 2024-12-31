using GSF.Net.Smtp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Mail;
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
                string query = @"SELECT Plano, PRAZO_PLANO, Desempenhar, PRAZO_DESEMPENHAR, Checar, PRAZO_CHECAR, Acao, PRAZO_ACAO, Participantes FROM PDCA;";
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                da.Fill(dt);
            }

            if (dt.Rows.Count == 0)
            {
                dt.Rows.Add("Definir metas e objetivos", DateTime.Now.AddDays(7), "Implementar ações",DateTime.Now.AddDays(14) , "Avaliar resultados",DateTime.Now.AddDays(21) , "Corrigir desvios",DateTime.Now.AddDays(28) , "Equipe A");
                dt.Rows.Add("Planejamento estratégico", DateTime.Now.AddDays(7), "Execução de tarefas", DateTime.Now.AddDays(14), "Monitoramento de desempenho", DateTime.Now.AddDays(21), "Ajustes necessários", DateTime.Now.AddDays(28), "Equipe B");
            }

            return dt;
        }

        protected void btnAdicionar_Click(object sender, EventArgs e)
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SISPREVConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO PDCA (Plano, PRAZO_PLANO, Desempenhar, PRAZO_DESEMPENHAR, Checar, PRAZO_CHECAR, Acao, PRAZO_ACAO, Participantes)" +
                    "VALUE (@Plano, @PRAZO_PLANO, @Desempenhar, @PRAZO_DESEMPENHAR, @Checar, @PRAZO_CHECAR, @Acao, @PRAZO_ACAO, @Participantes);";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Plano", txtPlano.Value);
                    cmd.Parameters.AddWithValue("@PRAZO_PLANO", txtPrazoPlano.Value);
                    cmd.Parameters.AddWithValue("@Desempenhar", txtDesempenhar.Value);
                    cmd.Parameters.AddWithValue("@PRAZO_DESEMPENHAR", txtPrazoDesempenhar.Value);
                    cmd.Parameters.AddWithValue("@Checar", txtChecar.Value);
                    cmd.Parameters.AddWithValue("@PRAZO_CHECAR", txtPrazoChecar.Value);
                    cmd.Parameters.AddWithValue("@Acao", txtAcao.Value);
                    cmd.Parameters.AddWithValue("@PRAZO_ACAO", txtPrazoAcao.Value);
                    cmd.Parameters.AddWithValue("@Participantes", txtParticipantes.Value);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }

            string[] participantes = txtParticipantes.Value.Split(',');

            foreach (string participanteId in participantes)
            {
                string participanteEmail = GetColaboradorEmailById(participanteId.Trim());
                if (!string.IsNullOrEmpty(participanteEmail))
                {
                    string subject = "Novo PDCA Adicionado";
                    string body = $"Um novo PDCA foi adicionado: \n" +
                        $"Plano: {txtPlano.Value}\n" +
                        $"Desempenhar: {txtDesempenhar.Value}\n" +
                        $"Checar: {txtChecar.Value}\n" +
                        $"Ação: {txtAcao.Value}\n" +
                        $"Prazos correspondentes.";
                    SendEmailNotification(participanteEmail, subject, body);
                }
            }

            BindPDCARepeater();

            ClearForm();
        }

        private void ClearForm()
        {
            txtPlano.Value = string.Empty;
            txtPrazoPlano.Value = string.Empty;
            txtDesempenhar.Value = string.Empty;
            txtPrazoDesempenhar.Value = string.Empty;
            txtChecar.Value = string.Empty;
            txtPrazoChecar.Value = string.Empty;
            txtAcao.Value = string.Empty;
            txtPrazoAcao.Value = string.Empty;
            txtParticipantes.Value = string.Empty;
        }

        private string GetColaboradorEmailById(string colaboradorId)
        {
            string email = string.Empty;
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SISPREVConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = "SELECT VERIFICAREMAIL FROM COLABORADORES WHERE COLABORADOR_ID = @COLABORADORID;";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@COLABORADORID", colaboradorId);
                    conn.Open();
                    email = cmd.ExecuteScalar()?.ToString();
                    conn.Close();
                }
            }

            return email;
        }

        private void SendEmailNotification(string toEmail, string subject, string body)
        {
            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress("samuel.silva@agendaassessoria.com.br");
                mail.To.Add(toEmail);
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;

                using (SmtpClient smtp = new SmtpClient("smtp.agendaassessoria.com.br"))
                {
                    smtp.Credentials = new NetworkCredential("samuel.silva@agendaassessoria.com.br", "Leumas74$");
                    smtp.EnableSsl = true;
                    smtp.Send(mail);
                }

            }

        }
    }
}