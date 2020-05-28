using StudioForge.BlockWorld;
using StudioForge.TotalMiner;
using StudioForge.TotalMiner.API;
using ArcadeMachines.TotalPong;
using ArcadeMachines.TotalDefender;
using ArcadeMachines.TotalRobo;

namespace ArcadeMachines
{
    class ArcadeMachinesPlugin : ITMPluginArcade
    {
        #region ITMPluginArcade

        ArcadeMachine ITMPluginArcade.GetArcadeMachine(int gameID, ITMGame game, ITMMap map, ITMPlayer player, GlobalPoint3D p, BlockFace face)
        {
            switch (gameID)
            {
                case 0:
                    return new TotalPongGame(game, map, player, p, face);
                case 1:
                    return new TotalDefenderGame(game, map, player, p, face);
                case 2:
                    return new TotalRoboGame(game, map, player, p, face);
            }
            return null;
        }

        IArcadeMachineRenderer ITMPluginArcade.GetArcadeMachineRenderer(int gameID)
        {
            IArcadeMachineRenderer renderer = null;

            switch (gameID)
            {
                case 0:
                    renderer = new TotalPongRenderer();
                    break;
                case 1:
                    renderer = new TotalDefenderRenderer();
                    break;
                case 2:
                    renderer = new TotalRoboRenderer();
                    break;
            }

            if (renderer != null)                
                renderer.LoadContent(null);

            return renderer;
        }

        string ITMPluginArcade.GetArcadeMachineName(int gameID)
        {
            switch (gameID)
            {
                case 0:
                    return "Total Pong";
                case 1:
                    return "Total Defender";
                case 2:
                    return "Total Robo";
                default:
                    return "Error";
            }
        }

        #endregion
    }
}
