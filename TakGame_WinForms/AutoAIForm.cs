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
    public partial class AutoAIForm : Form
    {
        BoardView _boardView;
        GameState _game;
        TakAI.Evaluator _evaluator;
        public AutoAIForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            _game = GameState.NewGame(5);
            _ai = new TakAI(_game.Size);
            _evaluator = new TakAI.Evaluator(_game.Size);
            _boardView = new BoardView();
            _boardView.Dock = DockStyle.Fill;
            _boardView.Game = _game;
            this.Controls.Add(_boardView);
            //_boardView.MouseOverSpotChanged += boardView_MouseOverSpotChanged;
        }

        private void boardView_MouseOverSpotChanged(object sender, EventArgs e)
        {
            _boardView.ClearHighlights();
            var mouseOver = _boardView.MouseOverSpot;
            if (mouseOver.HasValue)
                _boardView.SetHighlight(mouseOver.Value.Position, mouseOver.Value.StackPos);
        }

        TakAI _ai;
        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();

            bool gameOver;
            int eval;
            _evaluator.Evaluate(_game, out eval, out gameOver);
            if (gameOver)
                _game.Clear();
            _boardView.InvalidateRender();
            backgroundWorker.RunWorkerAsync(_game.DeepCopy());
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _boardView.Game = _game = (GameState)e.Result;
            timer1.Start();
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var game = (GameState)e.Argument;
            var move = _ai.FindGoodMove(game);
            move.MakeMove(game);
            game.Ply++;
            e.Result = game;
        }
    }
}
