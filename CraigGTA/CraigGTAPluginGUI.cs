using StudioForge.BlockWorld;
using StudioForge.TotalMiner;
using StudioForge.TotalMiner.API;
using VehiclesMod.Screens;

namespace VehiclesMod
{
    class CraigGTAPluginGUI : ITMPluginGUI
    {
        #region ITMPlugin

        bool ITMPluginGUI.HasItemCustomSetupScreen(Item itemID)
        {
            switch (itemID)
            {
                case Item.Rasta:
                    return true;
                default:
                    return false;
            }
        }

        NewGuiMenu ITMPluginGUI.GetItemCustomSetupScreen(INewGuiMenuScreen screen, ITMGame game, ITMPlayer player, GlobalPoint3D p, Item itemID)
        {
            switch (itemID)
            {
                case Item.Rasta:
                    return new VehicleSetupScreen(screen, game, player, p);

                default:
                    return null;
            }
        }

        #endregion
    }
}
