using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
    }
}