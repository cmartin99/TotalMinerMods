using StudioForge.BlockWorld;
using StudioForge.TotalMiner;
using StudioForge.TotalMiner.API;

namespace TotalDefenderArcade
{
    class TotalDefenderPlugin : ITMPluginArcade
    {
        #region ITMPluginArcade

        ArcadeMachine ITMPluginArcade.GetArcadeMachine(int gameID, ITMGame game, ITMMap map, ITMPlayer player, GlobalPoint3D p, BlockFace face)
        {
            return new TotalDefenderGame(game, map, player, p, face);
        }

        IArcadeMachineRenderer ITMPluginArcade.GetArcadeMachineRenderer(int gameID)
        {
            var renderer = new TotalDefenderRenderer();
            renderer.LoadContent(null);
            return renderer;
        }

        string ITMPluginArcade.GetArcadeMachineName(int gameID)
        {
            return "Total Defender";
        }

        #endregion
    }
}
