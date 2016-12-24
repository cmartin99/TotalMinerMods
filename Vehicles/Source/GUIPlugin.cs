using Craig.BlockWorld;
using Craig.TotalMiner;
using Craig.TotalMiner.API;
using VehiclesMod.Screens;

namespace VehiclesMod
{
    class GUIPlugin : ITMPlugInGUI
    {
        #region ITMPlugIn

        NewGuiMenu ITMPlugInGUI.GetItemCustomSetupScreen(ITMGame game, ITMPlayer player, GlobalPoint3D p, Item itemID)
        {
            switch (itemID)
            {
                case Item.Rasta:
                    return new VehicleSetupScreen(game, player, p);

                default:
                    return null;
            }
        }

        #endregion
    }
}
