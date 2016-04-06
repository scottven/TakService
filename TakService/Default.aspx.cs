using System;
using System.Data;

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
            all_moves.DataSource = null;
            all_moves.DataBind();
            move.Text = service.GetMove(ptn.Text, null, Int32.Parse(aiLevel.Text), Int32.Parse(flatScore.Text), tps_true.Checked);
        }

        protected void all_Click(object sender, EventArgs e)
        {
            TakMoveService service = new TakMoveService();
            string[][] moves = service.GetAllMoves(ptn.Text, Int32.Parse(aiLevel.Text), Int32.Parse(flatScore.Text), tps_true.Checked);
            DataTable moves_data = new DataTable();
            moves_data.Columns.Add(new DataColumn("Move", typeof(string)));
            moves_data.Columns.Add(new DataColumn("Score", typeof(string)));
            for(int i = moves.Length - 1; i >= 0; i--)
            {
                moves_data.Rows.Add(moves[i]);
            }
            all_moves.DataSource = moves_data;
            all_moves.DataBind();
        }
    }
}