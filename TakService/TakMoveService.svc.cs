using System;
using System.ServiceModel.Activation;
using TakEngine;

namespace TakService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "TakMoveService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select TakMoveService.svc or TakMoveService.svc.cs at the Solution Explorer and start debugging.
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class TakMoveService : ITakMoveService
    {

        GameState _game;
        TakAI_V3 _ai;

        public string GetMove(string ptn = null, string code = null, int aiLevel = 3, int flatScore = 9000, bool tps = false)
        {
            string input;
            if(ptn == null)
            {
                if(code == null) { return "must set code or ptn"; };
                input = code;
            }
            else if(code == null) { input = ptn; }
            else { return "can't set both code and ptn"; };

            try {
                InitGame(input, aiLevel, flatScore, tps);
                var next_move = _ai.FindGoodMove(_game);
                return next_move.Notate();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string[][] GetAllMoves(string code, int aiLevel = 3, int flatScore = 9000, bool tps = false)
        {
            try
            {
                InitGame(code, aiLevel, flatScore, tps);
                //throw new Exception("no support for All Moves, yet");
                return _ai.ReportAllMoves(_game).ToStringArray();
            }
            catch (Exception ex)
            {
                string[][] ret = new string[1][];
                ret[0] = new string[1];
                ret[0][0] = ex.Message;
                return ret;
            }
        }

        void InitGame(string code, int aiLevel, int flatScore, bool tps)
        {
            if (code == null) { throw new Exception("must supply code"); };
            // deal with odd IIS behavior
            if (aiLevel == 0) { aiLevel = 3; };
            if (flatScore == 0) { flatScore = 9000; };

            if (tps)
            {
                _game = GameState.LoadFromTPS(code);

            }
            else //turn-by-turn ptn
            {
                _game = GameState.LoadFromPTN(code);
            }
            _ai = new TakAI_V3(_game.Size, flatScore);
            _ai.MaxDepth = aiLevel;
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
