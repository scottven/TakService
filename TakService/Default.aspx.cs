using System;

namespace TakService
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void go_Click(object sender, EventArgs e)
        {
            TakMoveService service = new TakMoveService();
            move.Text = service.GetMove(ptn.Text, null, Int32.Parse(aiLevel.Text), Int32.Parse(flatScore.Text), tps_true.Checked);
        }
    }
}