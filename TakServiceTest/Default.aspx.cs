using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TakServiceTest
{
    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        protected void go_Click(object sender, EventArgs e)
        {
            CloudTakMoveService.TakMoveServiceClient client = new CloudTakMoveService.TakMoveServiceClient();
            move.Text = client.GetMove(ptn.Text);
            client.Close();
            //move.Text = "just do something!";
        }
    }
}