using System;

namespace TakEngine.Notation
{
    /// <summary>
    /// Builds a DatabaseRecord (and the data contained therein) as a PTN file is parsed
    /// </summary>
    class DatabaseBuilder : Generated.TakPGNBaseListener
    {
        public DatabaseRecord Database = new DatabaseRecord();

        public DatabaseBuilder()
        {
        }

        public GameRecord _currentGame;
        public override void EnterPgn_game(Generated.TakPGNParser.Pgn_gameContext context)
        {
            _currentGame = new GameRecord();
            Database.Games.Add(_currentGame);
            base.EnterPgn_game(context);
        }

        public override void ExitTag_pair(Generated.TakPGNParser.Tag_pairContext context)
        {
            _currentGame.Tags.Add(context.tag_name().SYMBOL().GetText(), context.tag_value().STRING().GetText().Trim('"'));
            base.ExitTag_pair(context);
        }

        int _inVariation = 0;
        public override void EnterRecursive_variation(Generated.TakPGNParser.Recursive_variationContext context)
        {
            _inVariation++;
            base.EnterRecursive_variation(context);
        }

        public override void ExitRecursive_variation(Generated.TakPGNParser.Recursive_variationContext context)
        {
            _inVariation--;
            base.ExitRecursive_variation(context);
        }

        public override void ExitSan_move(Generated.TakPGNParser.San_moveContext context)
        {
            var notationText = context.SYMBOL().GetText();
            MoveNotation notation;
            if (!MoveNotation.TryParse(notationText, out notation))
                throw new ApplicationException("Unrecognized move notation: " + notationText);
            _currentGame.MoveNotations.Add(notation);
            base.ExitSan_move(context);
        }

        public override void ExitGame_termination(Generated.TakPGNParser.Game_terminationContext context)
        {
            if (context.DRAWN_GAME() != null)
                _currentGame.ResultCode = context.DRAWN_GAME().GetText();
            if (context.P1_WINS_FLATS() != null)
                _currentGame.ResultCode = context.P1_WINS_FLATS().GetText();
            if (context.P2_WINS_FLATS() != null)
                _currentGame.ResultCode = context.P2_WINS_FLATS().GetText();
            if (context.P1_WINS_ROAD() != null)
                _currentGame.ResultCode = context.P1_WINS_ROAD().GetText();
            if (context.P2_WINS_ROAD() != null)
                _currentGame.ResultCode = context.P2_WINS_ROAD().GetText();
            if (context.ASTERISK() != null)
                _currentGame.ResultCode = context.ASTERISK().GetText();

            base.ExitGame_termination(context);
        }
    }
}
