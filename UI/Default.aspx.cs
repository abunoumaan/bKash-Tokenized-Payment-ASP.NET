using System;

namespace bKashPayment
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Load any default data if needed
            }
        }
    }
}
