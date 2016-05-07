using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace TakGame_WinForms
{
    public partial class HistoryForm : Form
    {
        bool _loaded = false;
        public HistoryForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            grid.DataSource = _turnData;
            _loaded = true;
            UpdateGridSelection();
            base.OnLoad(e);
        }

        int selectedCellRow = 0;
        int selectedCellColumn = 0;

        bool _updatingTurn = false;
        private void grid_CellStateChanged(object sender, DataGridViewCellStateChangedEventArgs e)
        {
            if (e.Cell == null || e.StateChanged != DataGridViewElementStates.Selected)
                return;

            //if Cell that changed state is to be selected you don't need to process
            //as event caused by 'unselectable' will select it again
            if (e.Cell.RowIndex == selectedCellRow && e.Cell.ColumnIndex == selectedCellColumn)
                return;

            //this condition is necessary if you want to reset your DataGridView
            if (!e.Cell.Selected)
                return;

            if (e.Cell.ColumnIndex == 0 || e.Cell.Value == null)
            {
                e.Cell.Selected = false;
                if (selectedCellColumn > 0)
                    grid.Rows[selectedCellRow].Cells[selectedCellColumn].Selected = true;
            }
            else
            {
                selectedCellRow = e.Cell.RowIndex;
                selectedCellColumn = e.Cell.ColumnIndex;
                if (!_updatingTurn)
                {
                    _updatingTurn = true;
                    this.SelectedPly = selectedCellRow * 2 + (selectedCellColumn - 1);
                    _updatingTurn = false;
                }
            }
        }

        int _selectedPly = -1;
        public int SelectedPly
        {
            get { return _selectedPly; }
            set
            {
                if (value != _selectedPly)
                {
                    _selectedPly = value;
                    if (!_updatingTurn)
                        UpdateGridSelection();
                    SelectedPlyChanged(this, EventArgs.Empty);
                }
            }
        }

        private void UpdateGridSelection()
        {
            if (_loaded && _turnData.Count > 0)
            {
                _updatingTurn = true;
                int row = _selectedPly / 2;
                int player = _selectedPly & 1;
                grid.Rows[row].Cells[player + 1].Selected = true;
                _updatingTurn = false;
            }
        }

        public event EventHandler SelectedPlyChanged = delegate { };
        public void Clear()
        {
            selectedCellColumn = 0;
            selectedCellRow = 0;
            _turnData.Clear();
        }

        public void AddPly(string notation)
        {
            TurnData data;
            bool addedp2 = false;
            if (_turnData.Count == 0 || _turnData[_turnData.Count - 1].P2Notation != null)
            {
                data = new TurnData();
                data.MoveNumber = _turnData.Count + 1;
                data.P1Notation = notation;
                _turnData.Add(data);
            }
            else
            {
                data = _turnData[_turnData.Count - 1];
                data.P2Notation = notation;
                addedp2 = true;
            }
            this.SelectedPly = _turnData.Count * 2 - (addedp2 ? 1 : 2);
        }

        public void RemoveLastPly()
        {
            var last = _turnData[_turnData.Count - 1];
            bool removedp2 = false;
            if (last.P2Notation == null)
                _turnData.RemoveAt(_turnData.Count - 1);
            else
            {
                last.P2Notation = null;
                removedp2 = true;
            }
            this.SelectedPly = _turnData.Count * 2 - (removedp2 ? 2 : 1);
        }

        BindingList<TurnData> _turnData = new BindingList<TurnData>();
        class TurnData : INotifyPropertyChanged
        {
            private int _moveNumber;
            public int MoveNumber
            {
                get { return _moveNumber; }
                set
                {
                    if (value != _moveNumber)
                    {
                        _moveNumber = value;
                        OnPropertyChanged("MoveNumber");
                    }
                }
            }

            private string _p1Notation;
            public string P1Notation
            {
                get { return _p1Notation; }
                set
                {
                    if (value != _p1Notation)
                    {
                        _p1Notation = value;
                        OnPropertyChanged("P1Notation");
                    }
                }
            }
            
            private string _p2Notation;
            public string P2Notation
            {
                get { return _p2Notation; }
                set
                {
                    if (value != _p2Notation)
                    {
                        _p2Notation = value;
                        OnPropertyChanged("P2Notation");
                    }
                }
            }

            void OnPropertyChanged(string name)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }

            public event PropertyChangedEventHandler PropertyChanged = delegate { };
        }
    }
}
