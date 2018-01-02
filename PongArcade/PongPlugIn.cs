using StudioForge.BlockWorld;
using StudioForge.TotalMiner;
using StudioForge.TotalMiner.API;

namespace PongArcade
{
    class PongPlugin : ITMPluginArcade
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
