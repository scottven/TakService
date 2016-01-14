using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TakEngine;

namespace TakGame_WinForms
{
    public partial class MainForm : Form
    {
        TakEngine.Notation.GameRecord _gameRecord = new TakEngine.Notation.GameRecord();
        string _fileName = null;
        const int DefaultGameSize = 5;
        BoardView _boardView;
        GameState _game;
        TakAI.Evaluator _evaluator;
        TakAI _ai;
        List<PieceToolInfo> _tools = new List<PieceToolInfo>();
        int?[] _aiLevels = new int?[] { null, null };
        System.Threading.ManualResetEvent _notThinking = new System.Threading.ManualResetEvent(true);
        HistoryForm _historyForm = null;
        
        // cache of IMove objects so that we don't have to regeneate the entire game history when navigating backwards
        Dictionary<TakEngine.Notation.MoveNotation, IMove> _movesOfNotation = new Dictionary<TakEngine.Notation.MoveNotation, IMove>();

        public MainForm()
        {
            InitializeComponent();
            listAiLevel.SelectedIndex = 0;
            _tools.Add(new PieceToolInfo(btnFlat0, Piece.MakePieceID(Piece.Stone_Flat, 0)));
            _tools.Add(new PieceToolInfo(btnFlat1, Piece.MakePieceID(Piece.Stone_Flat, 1)));
            _tools.Add(new PieceToolInfo(btnCap0, Piece.MakePieceID(Piece.Stone_Cap, 0)));
            _tools.Add(new PieceToolInfo(btnCap1, Piece.MakePieceID(Piece.Stone_Cap, 1)));
            _tools.Add(new PieceToolInfo(btnStand0, Piece.MakePieceID(Piece.Stone_Standing, 0)));
            _tools.Add(new PieceToolInfo(btnStand1, Piece.MakePieceID(Piece.Stone_Standing, 1)));
            this.KeyPreview = true;

            foreach (var tool in _tools)
                tool.Btn.Click += toolBtn_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            _boardView = new BoardView();
            _boardView.Dock = DockStyle.Fill;
            this.Controls.Add(_boardView);
            _boardView.BringToFront();
            _boardView.MouseClick += boardView_MouseClick;
            _boardView.MouseOverSpotChanged += boardView_MouseOverSpotChanged;
            InitButtonImages();
            NewGame(DefaultGameSize);
        }

        IBoardInteraction _interaction;
        void CancelInteraction()
        {
            if (_interaction != null)
                _interaction.Cancel();
            _interaction = null;
        }

        void NewGame(int size)
        {
            _fileName = null;
            _gameRecord.Tags.Clear();
            _gameRecord.MoveNotations.Clear();
            _game = GameState.NewGame(size);
            _ai = new TakAI(_game.Size);
            _evaluator = new TakAI.Evaluator(_game.Size);
            _boardView.Game = _game;
            PrepareTurn();
        }

        void InitButtonImages()
        {
            foreach (var tool in _tools)
                tool.Btn.Image = IsoImages.GetPieceImage(tool.PieceID);
        }

        private void boardView_MouseOverSpotChanged(object sender, EventArgs e)
        {
            if (_interaction != null)
            {
                var mouseOver = _boardView.MouseOverSpot;
                _interaction.CancelPreview();
                if (mouseOver.HasValue)
                    _interaction.TryPreviewAt(mouseOver.Value);
            }
        }

        class PieceToolInfo
        {
            public Button Btn;
            public int PieceID;
            public PieceToolInfo(Button btn, int pieceID)
            {
                Btn = btn;
                PieceID = pieceID;
            }
        }

        void UpdateTools()
        {
            foreach (var tool in _tools)
            {
                string text = string.Empty;
                int player = Piece.GetPlayerID(tool.PieceID);
                int stone = Piece.GetStone(tool.PieceID);
                bool allowed = true;
                switch (stone)
                {
                    case Piece.Stone_Flat:
                        text = "Flat x " + _game.StonesRemaining[player].ToString();
                        break;
                    case Piece.Stone_Standing:
                        allowed = _game.Ply > 1;
                        break;
                    case Piece.Stone_Cap:
                        text = "Cap x " + _game.CapRemaining[player].ToString();
                        allowed = _game.CapRemaining[player] > 0 && _game.Ply > 1;
                        break;
                }
                tool.Btn.Text = text;
                tool.Btn.Enabled = allowed;
            }
            Panel[] playerControls = new Panel[] { player1Controls, player2Controls };
            int nextPlayer = _game.Ply & 1;
            if (_game.Ply < 2)
                nextPlayer ^= 1;
            playerControls[nextPlayer].Enabled = true;
            playerControls[nextPlayer ^ 1].Enabled = false;
        }

        PieceToolInfo GetTool(Button btn)
        {
            return _tools.First(x => x.Btn == btn);
        }

        private void toolBtn_Click(object sender, EventArgs e)
        {
            var tool = GetTool(sender as Button);
            SetActiveTool(tool);
        }

        PieceToolInfo _activeTool = null;
        void SetActiveTool(PieceToolInfo tool)
        {
            if (_activeTool != null)
            {
                _activeTool.Btn.BackColor = Color.Transparent;
                _activeTool = null;
            }

            CancelInteraction();

            _activeTool = tool;
            if (_activeTool != null)
            {
                _activeTool.Btn.BackColor = Color.LightCyan;
                _interaction = new InteractiveMove_PlaceFromReserve(_game, _boardView, _activeTool.PieceID);
            }
            else
                _interaction = new InteractiveMove_PickupAndPlace(_game, _boardView);
        }

        bool _aiLevelUpdating = true;

        void PrepareTurn()
        {
            _interaction = null;
            CancelTool();
            int playerID = _game.Ply & 1;
            lblAiLevel.Text = string.Format("Player {0} AI:", 1 + playerID);
            _aiLevelUpdating = true;
            if (!_aiLevels[playerID].HasValue)
                listAiLevel.SelectedIndex = 0;
            else
                listAiLevel.SelectedItem = listAiLevel.Items.Cast<string>().FirstOrDefault(x => x.EndsWith(_aiLevels[playerID].ToString()));
            _aiLevelUpdating = false;
            UpdateTools();

            int eval;
            bool gameOver;
            _evaluator.Evaluate(_game, out eval, out gameOver);
            if (gameOver)
            {
                SetResultCode(eval);
                _interaction = null;
                if (eval == 0)
                    lblStatus.Text = "Game over, it's a tie!";
                else
                    lblStatus.Text = string.Format("Game over, player {0} wins!", eval > 0 ? 1 : 2);
                tablePanel.Enabled = false;
                miMoveEnter.Enabled = false;
            }
            else
            {
                lblStatus.Text = string.Format("Player {0}'s move (turn {1})",
                    1 + (_game.Ply & 1),
                    _game.Ply + 1);
                tablePanel.Enabled = true;
                miMoveEnter.Enabled = true;
            }
            UpdateToolbar();
            CheckStartAI();
        }

        private void SetResultCode(int eval)
        {
            if (eval == 0)
            {
                _gameRecord.Result = _gameRecord.ResultCode = "1/2-1/2";
            }
            else
            {
                string condition = "F";
                if (Math.Abs(eval) > TakAI.Evaluator.FlatWinEval)
                    condition = "R";
                if (eval > 0)
                    _gameRecord.Result = _gameRecord.ResultCode = string.Format("{0}-0", condition);
                else
                    _gameRecord.Result = _gameRecord.ResultCode = string.Format("0-{0}", condition);
            }
        }

        void MoveTick()
        {
            if (_interaction.HasPreview)
                _interaction.AcceptPreview();
            if (_interaction.Completed)
            {
                var move = _interaction.GetMove();
                AddMoveToGameRecord(move);
                _game.Ply++;
                PrepareTurn();
            }
            _boardView.InvalidateMouseOverSpot();
            boardView_MouseOverSpotChanged(this, EventArgs.Empty);
        }

        void AddMoveToGameRecord(IMove move)
        {
            TakEngine.Notation.MoveNotation notated;
            if (!TakEngine.Notation.MoveNotation.TryParse(move.Notate(), out notated))
                throw new ApplicationException("Critical movement error"); // this shouldn't ever happen
            _gameRecord.MoveNotations.Add(notated);
            _navigating = true;
            if (_historyForm != null)
                _historyForm.AddPly(notated.Text);
            _navigating = false;
            _movesOfNotation[notated] = move;
        }

        private void boardView_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                CancelTool();
            else if (e.Button == MouseButtons.Left && _interaction != null)
                MoveTick();
        }

        private void CancelTool()
        {
            SetActiveTool(null);
            _boardView.CarryClear();
            _boardView.ClearHighlights();
        }

        #region Keys
        const char EscapeKey = '\x1b';
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if (e.KeyChar == EscapeKey)
            {
                CancelTool();
                e.Handled = true;
            }
            else if (e.KeyChar == 'e')
            {
                e.Handled = true;
                BeginInvoke(new Action(MoveEnterNotation));
            }
            base.OnKeyPress(e);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            int keyByte = ((int)keyData & 0xFF);
            bool handled = false;
            if ((keyData & Keys.KeyCode) == Keys.Left)
            {
                BeginInvoke(new Action(MovePrev));
                handled = true;
            }
            else if ((keyData & Keys.KeyCode) == Keys.Right)
            {
                BeginInvoke(new Action(MoveNext));
                handled = true;
            }
            if (handled)
                return true;
            else
                return base.ProcessCmdKey(ref msg, keyData);
        }

        #endregion

        private void btnNew_Click(object sender, EventArgs e)
        {
            using (var dlg = new NewGameForm(_game.Size))
            {
                if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                {
                    NewGame(dlg.SelectedSize);
                    var recommendedControlSize = _boardView.RecommendedSize;

                    var delta = new Size(_boardView.Width - recommendedControlSize.Width, _boardView.Height - recommendedControlSize.Height);
                    if (delta.Width < 0)
                        this.Width -= delta.Width;
                    if (delta.Height < 0)
                        this.Height -= delta.Height;
                }
            }
        }

        void UpdateToolbar()
        {
            btnUndo.Enabled = (_gameRecord.MoveNotations.Count > 0);
        }

        private void btnUndo_Click(object sender, EventArgs e)
        {
            CancelInteraction();
            int undoCount = 1;
            if (_aiLevels[1 ^ (_game.Ply & 1)].HasValue)
                undoCount = 2;
            undoCount = Math.Min(undoCount, _gameRecord.MoveNotations.Count);
            for (int i = 0; i < undoCount; i++)
            {
                var lastNotation = _gameRecord.MoveNotations.RemoveLast();
                var move = _movesOfNotation[lastNotation];
                if (move == null)
                    throw new ApplicationException("Critical move error"); // this shouldn't ever happen
                move.TakeBackMove(_game);
                _game.Ply--;
                _movesOfNotation.Remove(lastNotation);
                _navigating = true;
                if (_historyForm != null)
                    _historyForm.RemoveLastPly();
                _navigating = false;
            }

            _boardView.InvalidateRender();
            PrepareTurn();
        }

        bool _aiSlowWarning = false;
        private void listAiLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_aiLevelUpdating)
                return;
            string s = listAiLevel.SelectedItem.ToString();
            int level;
            if (int.TryParse(s.Substring(s.Length - 1), out level))
            {
                if (_game.Size > 5 && !_aiSlowWarning)
                {
                    MessageBox.Show(this,
                        "The selected game size is larger than 5x5.\nAt larger board sizes, the AI is slow and probably not challenging.\nYou have been warned.",
                        "AI performance warning",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    _aiSlowWarning = true;
                }

                _aiLevels[_game.Ply & 1] = level;
                CheckStartAI();
            }
            else
                _aiLevels[_game.Ply & 1] = null;
        }

        bool _aiRunning = false;
        void CheckStartAI()
        {
            if (!_aiLevels[_game.Ply & 1].HasValue)
                return;

            int eval;
            bool gameOver;
            _evaluator.Evaluate(_game, out eval, out gameOver);
            if (gameOver)
            {
                SetResultCode(eval);
                return;
            }

            CancelInteraction();

            int playerID = _game.Ply & 1;
            lblStatus.Text = string.Format("Player {0} AI thinking...", playerID + 1);
            btnCancel.Visible = true;
            btnCancel.Enabled = true;
            btnNew.Enabled = false;
            listAiLevel.Enabled = false;
            btnUndo.Enabled = false;
            tablePanel.Enabled = false;
            miMoveEnter.Enabled = false;
            _notThinking.Reset();
            _aiRunning = true;
            if (_historyForm != null)
                _historyForm.Enabled = false;
            System.Threading.ThreadPool.QueueUserWorkItem(AI_BackgroundThread);
        }


        void AI_BackgroundThread(object unused)
        {
            var gameCopy = _game.DeepCopy();
            _ai.MaxDepth = _aiLevels[_game.Ply & 1].Value;
            var move = _ai.FindGoodMove(gameCopy);
            BeginInvoke(new Action<IMove>(AI_Completed), move);
            _notThinking.Set();
        }

        void AI_Completed(IMove move)
        {
            _aiRunning = false;
            if (_historyForm != null)
                _historyForm.Enabled = true;
            btnCancel.Visible = false;
            btnNew.Enabled = true;
            listAiLevel.Enabled = true;
            if (_ai.Canceled)
            {
                _aiLevelUpdating = true;
                _aiLevels[_game.Ply & 1] = null;
                listAiLevel.SelectedIndex = 0;
                _aiLevelUpdating = false;
            }
            else
            {
                move.MakeMove(_game);
                AddMoveToGameRecord(move);
                _boardView.InvalidateRender();
                _game.Ply++;
            }
            PrepareTurn();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _ai.Cancel();
            btnCancel.Enabled = false;
            lblStatus.Text = "Canceling...";
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            _ai.Cancel();
            _notThinking.WaitOne();
            base.OnClosing(e);
        }

        private void btnRotate_Click(object sender, EventArgs e)
        {
            _boardView.RotateView();
        }

        List<IMove> _tempMoveList = new List<IMove>();
        private void miFileOpen_Click(object sender, EventArgs e)
        {
            if (dlgOpen.ShowDialog(this) != DialogResult.OK)
                return;
            try
            {
                var database = TakEngine.Notation.TakPGN.LoadFromFile(dlgOpen.FileName);
                if (database.Games.Count != 1)
                    throw new ApplicationException("File must contain exactly 1 game");
                _gameRecord = database.Games[0];
                _game = GameState.NewGame(_gameRecord.BoardSize);
                _ai = new TakAI(_game.Size);
                _evaluator = new TakAI.Evaluator(_game.Size);
                _boardView.Game = _game;

                foreach (var notation in _gameRecord.MoveNotations)
                {
                    _tempMoveList.Clear();
                    TakAI.EnumerateMoves(_tempMoveList, _game, _ai.NormalPositions);
                    var move = notation.MatchLegalMove(_tempMoveList);
                    if (null == move)
                        throw new ApplicationException("Illegal move: " + notation.Text);
                    move.MakeMove(_game);
                    _movesOfNotation[notation] = move;
                    _navigating = true;
                    if (_historyForm != null)
                        _historyForm.AddPly(notation.Text);
                    _navigating = false;
                    _game.Ply++;
                }
                _fileName = dlgOpen.FileName;
                PrepareTurn();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Failed to open file", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        bool PromptSaveFile()
        {
            if (dlgSave.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                _fileName = dlgSave.FileName;
                return true;
            }
            else
                return false;
        }

        private void miFileSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_fileName))
                PromptSaveFile();
            if (!string.IsNullOrEmpty(_fileName))
                Save();
        }

        private void miFileSaveAs_Click(object sender, EventArgs e)
        {

            if (PromptSaveFile())
                Save();
        }

        void Save()
        {
            // ensure board size tag is set correctly
            _gameRecord.BoardSize = _game.Size;

            // finish saving
            var database = new TakEngine.Notation.DatabaseRecord();
            database.Games.Add(_gameRecord);
            database.Save(_fileName);
        }

        private void miMoveEnter_Click(object sender, EventArgs e)
        {
            NewMethod();
        }

        private void NewMethod()
        {
            MoveEnterNotation();
        }

        private void MoveEnterNotation()
        {
            using (var dlg = new EnterMoveForm(_game))
            {
                if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                {
                    var move = dlg.ValidatedMove;
                    move.MakeMove(_game);
                    _movesOfNotation[dlg.ValidatedNotation] = move;
                    _boardView.InvalidateRender();
                    _game.Ply++;
                    PrepareTurn();
                }
            }
        }

        private void historyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_historyForm != null)
            {
                _historyForm.Activate();
                return;
            }

            _historyForm = new HistoryForm();
            _historyForm.FormClosed += history_Closed;
            _historyForm.Top = this.Top;
            _historyForm.Left = this.Right;
            _historyForm.SelectedPlyChanged += historyForm_SelectedPlyChanged;
            var screenBounds = Screen.GetBounds(this._boardView);
            if (_historyForm.Right >= screenBounds.Right)
                _historyForm.Left = this.Left - _historyForm.Width;
            if (_historyForm.Left < 0)
                _historyForm.Left = 0;            
            _historyForm.Owner = this;

            foreach (var move in _gameRecord.MoveNotations)
                _historyForm.AddPly(move.Text);

            _historyForm.Show();
        }

        void historyForm_SelectedPlyChanged(object sender, EventArgs e)
        {
            if (!_navigating)
                Navigate(_historyForm.SelectedPly);
        }

        private void history_Closed(object sender, FormClosedEventArgs e)
        {
            _historyForm.FormClosed -= history_Closed;
            _historyForm.Dispose();
            _historyForm = null;
        }

        private void miMovePrev_Click(object sender, EventArgs e)
        {
            MovePrev();
        }

        private void miMoveNext_Click(object sender, EventArgs e)
        {
            MoveNext();
        }

        void MovePrev()
        {
            Navigate(_game.Ply - 2);
        }

        void MoveNext()
        {
            Navigate(_game.Ply);
        }

        bool _navigating = false;
        void Navigate(int ply)
        {
            if (_aiRunning)
                return;
            if (ply < 0 || ply >= _gameRecord.MoveNotations.Count)
                return;
            _navigating = true;
            CancelTool();
            _interaction = null;
            _game.Clear();
            for (int i = 0; i <= ply; i++)
            {
                var notation = _gameRecord.MoveNotations[i];
                IMove move;
                if (!_movesOfNotation.TryGetValue(notation, out move))
                    throw new ApplicationException("Critical movement error");
                move.MakeMove(_game);
                _game.Ply++;
            }
            _boardView.InvalidateRender();
            if (_historyForm != null)
                _historyForm.SelectedPly = ply;
            _navigating = false;

            if (_game.Ply < _gameRecord.MoveNotations.Count)
            {
                lblStatus.Text = string.Format("Viewing turn {0}.{1}", ply / 2 + 1, 1 + (ply & 1));
                listAiLevel.Enabled = false;
                btnUndo.Enabled = false;
                tablePanel.Enabled = false;
                miMoveEnter.Enabled = false;
            }
            else
            {
                listAiLevel.Enabled = true;
                PrepareTurn();
            }
        }
    }
}
