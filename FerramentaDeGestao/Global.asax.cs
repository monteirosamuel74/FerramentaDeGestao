using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Timers;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace FerramentaDeGestao
{
    public class Global : System.Web.HttpApplication
    {
        private static System.Timers.Timer _timer;
        protected void Application_Start(object sender, EventArgs e)
        {
            _timer = new System.Timers.Timer();
            _timer.Interval = 86400000;
            _timer.Elapsed += new ElapsedEventHandler(CheckPDCADeadLines);
            _timer.Start();
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }

        private void CheckPDCADeadLines(object sender, ElapsedEventArgs eea)
        {
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SISPREVConnectionString"].ConnectionString))
            {
                string sql = "SELECT Plano, PRAZO_PLANO, Desempenhar, PRAZO_DESEMPENHAR, Checar, PRAZO_CHECAR, Acao, PRAZO_ACAO, Participantes FROM PDCA";
                SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                foreach (DataRow row in dt.Rows)
                {
                    DateTime prazoPlano = Convert.ToDateTime(row["PRAZO_PLANO"]);
                    DateTime prazoDesempenhar = Convert.ToDateTime(row["PRAZO_DESEMPENHAR"]);
                    DateTime prazoChecar = Convert.ToDateTime(row["PRAZO_CHECAR"]);
                    DateTime prazoAcao = Convert.ToDateTime(row["PRAZO_ACAO"]);
                    string[] participantes = row["Participantes"].ToString().Split(',');

                    CheckAndNotify(row["Plano"].ToString(), prazoPlano, "Plano", participantes);
                    CheckAndNotify(row["Desempenhar"].ToString(), prazoDesempenhar, "Desempenhar", participantes);
                    CheckAndNotify(row["Checar"].ToString(), prazoChecar, "Checar", participantes);
                    CheckAndNotify(row["Acao"].ToString(), prazoAcao, "Acao", participantes);
                }
            }
        }

        private void CheckAndNotify(string tarefa, DateTime prazo, string tipo, string[] participantes)
        {
            DateTime agora = DateTime.Now;
            if ((prazo - agora).Days == 2)
            {
                foreach (string participanteId in participantes)
                {
                    string participanteEmail = GetColaboradorEmailById(participanteId);
                    if (!string.IsNullOrEmpty(participanteEmail))
                    {
                        string subject = $"Lembrete de Prazo - {tipo}";
                        string body = $"A tarefa '{tarefa}' do tipo '{tipo}' está prevista para vencer em 2 dias. Prazo: {prazo:dd/MM/yyyy}";
                        SendEmailNotification(participanteEmail, subject, body);
                    }
                }
            }
            else if ((prazo-agora).Days == 0)
            {
                foreach (string participanteId in participantes)
                {
                    string participanteEmail = GetColaboradorEmailById(participanteId.Trim());
                    if (!string.IsNullOrEmpty(participanteEmail))
                    {
                        string subject = $"Prazo Vencido - {tipo}";
                        string body = $"A tarefa '{tarefa}' do tipo '{tipo}' venceu. Prazo: {prazo:dd/MM/yyyy} ";
                        SendEmailNotification(participanteEmail, subject, body);
                    }
                }

            }
        }

        private string GetColaboradorEmailById(string colaboradorId)
        {
            string email = string.Empty;
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SISPREVConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = "SELECT Email FROM Colaboradores WHERE colaborador_id = @ColaboradorId;";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@ColaboradorId", colaboradorId);
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