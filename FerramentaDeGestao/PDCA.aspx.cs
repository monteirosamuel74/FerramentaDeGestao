using System;
using System.Collections.Generic;
using System.Data;
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
            DataTable dt = (DataTable)ViewState["CriarTabelaPDCA"] ?? GetPDCADetails();

            rptPDCA.DataSource = dt;
            rptPDCA.DataBind();
        }

        private DataTable GetPDCADetails()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Plano");
            dt.Columns.Add("Designacao");
            dt.Columns.Add("Checar");
            dt.Columns.Add("Acao");
            dt.Columns.Add("Participantes");

            dt.Rows.Add(dt);

            return dt;
        }

        protected void btnAdicionar_Click(object sender, EventArgs e)
        {
            DataTable dt = (DataTable)ViewState["CriarTabelaPDCA"] ?? GetPDCADetails();

            dt.Rows.Add(txtPlano.Value, txtDesignacao.Value, txtChecar.Value, txtAcao, txtParticipantes.Value);

            ViewState["CriarTabelaPDCA"] = dt;

            BindPDCARepeater();

            txtPlano.Value = string.Empty;
        }
    }
}