using StudioForge.BlockWorld;
using StudioForge.TotalMiner;
using StudioForge.TotalMiner.API;

namespace TotalRobo
{
    class TotalRoboPlugin : ITMPluginArcade
    {
        #region ITMPluginArcade

        ArcadeMachine ITMPluginArcade.GetArcadeMachine(int gameID, ITMGame game, ITMMap map, ITMPlayer player, GlobalPoint3D p, BlockFace face)
        {
            return new TotalRoboGame(game, map, player, p, face);
        }

        IArcadeMachineRenderer ITMPluginArcade.GetArcadeMachineRenderer(int gameID)
        {
            var renderer = new TotalRoboRenderer();
            renderer.LoadContent(null);
            return renderer;
        }

        string ITMPluginArcade.GetArcadeMachineName(int gameID)
        {
            return "Total Robo";
        }

        #endregion
    }
}
