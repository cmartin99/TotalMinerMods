using Craig.BlockWorld;
using Craig.TotalMiner;
using Craig.TotalMiner.API;

namespace ArcadeGames
{
    class ArcadePlugin : ITMPlugInArcade
    {
        #region ITMPlugInArcade

        ArcadeMachine ITMPlugInArcade.GetArcadeMachine(int gameID, ITMGame game, ITMMap map, ITMPlayer player, GlobalPoint3D p, BlockFace face)
        {
            return new PongGame(game, map, player, p, face);
        }

        IArcadeMachineRenderer ITMPlugInArcade.GetArcadeMachineRenderer(int gameID)
        {
            var renderer = new PongRenderer();
            renderer.LoadContent(null);
            return renderer;
        }

        string ITMPlugInArcade.GetArcadeMachineName(int gameID)
        {
            return "Total Pong";
        }

        #endregion
    }
}
