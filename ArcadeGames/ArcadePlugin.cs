using Craig.BlockWorld;
using Craig.TotalMiner;
using Craig.TotalMiner.API;

namespace ArcadeGames
{
    class ArcadePlugin : ITMPluginArcade
    {
        #region ITMPluginArcade

        ArcadeMachine ITMPluginArcade.GetArcadeMachine(int gameID, ITMGame game, ITMMap map, ITMPlayer player, GlobalPoint3D p, BlockFace face)
        {
            return new PongGame(game, map, player, p, face);
        }

        IArcadeMachineRenderer ITMPluginArcade.GetArcadeMachineRenderer(int gameID)
        {
            var renderer = new PongRenderer();
            renderer.LoadContent(null);
            return renderer;
        }

        string ITMPluginArcade.GetArcadeMachineName(int gameID)
        {
            return "Total Pong";
        }

        #endregion
    }
}
