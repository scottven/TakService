using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using TakEngine;

namespace TakService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "TakMoveService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select TakMoveService.svc or TakMoveService.svc.cs at the Solution Explorer and start debugging.
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class TakMoveService : ITakMoveService
    {
        
        public string GetMove(string ptn, int aiLevel = 3, int flatScore = 9000, bool tps = false)
        {
            try {
                GameState _game;
                TakAI _ai;
                TakAI.Evaluator _evaluator;
                if (tps)
                {
                    _game = GameState.LoadTPS(ptn);
                    _ai = new TakAI(_game.Size, flatScore);
                    _evaluator = new TakAI.Evaluator(_game.Size);
                }
                else //turn-by-turn ptn
                {
                    var database = TakEngine.Notation.TakPGN.LoadFromString(ptn);
                    TakEngine.Notation.GameRecord _gameRecord = new TakEngine.Notation.GameRecord();
                    _gameRecord = database.Games[0];
                    _game = GameState.NewGame(_gameRecord.BoardSize);
                    _ai = new TakAI(_game.Size, flatScore);
                    _evaluator = new TakAI.Evaluator(_game.Size);
                    foreach (var notation in _gameRecord.MoveNotations)
                    {
                        List<IMove> _tempMoveList = new List<IMove>();
                        TakAI.EnumerateMoves(_tempMoveList, _game, _ai.NormalPositions);
                        var move = notation.MatchLegalMove(_tempMoveList);
                        if (null == move)
                            return string.Format("Illegal move: {0}", notation.Text);
                        move.MakeMove(_game);
                        _game.Ply++;
                    }
                }
                _ai.MaxDepth = aiLevel;
                var next_move = _ai.FindGoodMove(_game);
                return next_move.Notate();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /*public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return composite;
        }*/
    }
}
