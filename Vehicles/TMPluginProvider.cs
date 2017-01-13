using StudioForge.TotalMiner.API;

namespace VehiclesMod
{
    public class MyPluginProvider : ITMPluginProvider
    {
        public ITMPlugin GetPlugin()
        {
            return new VehiclesMod();
        }

        public ITMPluginBlocks GetPluginBlocks()
        {
            return new VehiclesBlocks();
        }

        public ITMPluginArcade GetPluginArcade()
        {
            return null;
        }

        public ITMPluginGUI GetPluginGUI()
        {
            return new GUIPlugin();
        }
    }
}