using System;
using System.Collections.Generic;
using System.Windows.Forms;
using TakEngine;
using TakEngine.Notation;

namespace TakGame_WinForms
{
    public partial class EnterMoveForm : Form
    {
        GameState _game;

        public EnterMoveForm(GameState game)
        {
            InitializeComponent();
            _game = game;
        }

        public IMove ValidatedMove { get; private set; }
        public MoveNotation ValidatedNotation { get; private set; }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (!ValidateNotation())
            {
                this.DialogResult = System.Windows.Forms.DialogResult.None;
                txtNotation.SelectAll();
                BeginInvoke(new Action(FocusNotation));
            }
        }

        void FocusNotation()
        {
            txtNotation.Focus();
        }

        bool ValidateNotation()
        {
            ValidatedMove = null;
            ValidatedNotation = null;
            MoveNotation notated;
            if (!MoveNotation.TryParse(txtNotation.Text, out notated))
            {
                lblError.Text = "Unrecognized notation";
                lblError.Visible = true;
                return false;
            }
            var ai = new TakAI(_game.Size);
            var moves = new List<IMove>();
            TakAI.EnumerateMoves(moves, _game, ai.NormalPositions);
            ValidatedMove = notated.MatchLegalMove(moves);
            if (ValidatedMove == null)
            {
                lblError.Text = "Illegal move";
                lblError.Visible = true;
                return false;
            }
            ValidatedNotation = notated;
            lblError.Visible = false;
            return true;
        }
    }
}
