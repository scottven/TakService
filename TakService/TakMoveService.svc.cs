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
                if (tps)
                {
                    _game = GameState.LoadFromTPS(ptn);

                }
                else //turn-by-turn ptn
                {
                    _game = GameState.LoadFromPTN(ptn);
                }
                TakAI _ai = new TakAI(_game.Size, flatScore);
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
