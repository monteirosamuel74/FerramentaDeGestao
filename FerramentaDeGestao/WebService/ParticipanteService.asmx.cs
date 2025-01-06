using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data.SqlClient;
using System.Web.Configuration;

namespace FerramentaDeGestao.WebService
{
    /// <summary>
    /// Serviço para buscar o participante no BD
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // Para permitir que esse serviço da web seja chamado a partir do script, usando ASP.NET AJAX, remova os comentários da linha a seguir. 
    // [System.Web.Script.Services.ScriptService]
    public class ParticipanteService : System.Web.Services.WebService
    {

        [WebMethod]
        public List<Participante> BuscarParticipantes(string query)
        {
            List<Participante> participantes = new List<Participante>();

            string connectionString = WebConfigurationManager.ConnectionStrings["SISPREVConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT COLABORADOR_ID, Nome, Email FROM COLABORADOR2 WHERE Nome LIKE @query", conn))
                {
                    cmd.Parameters.AddWithValue("@query", "%" + query + "%");

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Participante participante = new Participante
                            {
                                COLABORADOR_ID = reader["COLABORADOR_ID"].ToString(),
                                Nome = reader["Nome"].ToString(),
                                Email = reader["Email"].ToString()
                            };
                            participantes.Add(participante);
                        }
                    }
                }
                conn.Close();
            }
            return participantes;
        }
        public class Participante
        {
            public string COLABORADOR_ID { get; set; }
            public string Nome { get; set; }
            public string Email { get; set; }
        }
    }
}
