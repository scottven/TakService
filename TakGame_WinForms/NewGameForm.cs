using System.Linq;
using System.Windows.Forms;

namespace TakGame_WinForms
{
    public partial class NewGameForm : Form
    {
        public NewGameForm(int defaultSize)
        {
            InitializeComponent();

            listSize.SelectedItem = listSize
                .Items
                .Cast<string>()
                .FirstOrDefault(x => x.StartsWith(defaultSize.ToString()));
        }

        public int SelectedSize { get { return int.Parse(listSize.SelectedItem.ToString().Substring(0, 1)); } }
    }
}
