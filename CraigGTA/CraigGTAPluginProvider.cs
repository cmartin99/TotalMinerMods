using StudioForge.TotalMiner.API;

namespace VehiclesMod
{
    public class CraigGTAPluginProvider : ITMPluginProvider
    {
        public ITMPlugin GetPlugin()
        {
            return new CraigGTAPlugin();
        }

        public ITMPluginBlocks GetPluginBlocks()
        {
            return new CraigGTAPluginBlocks();
        }

        public ITMPluginArcade GetPluginArcade()
        {
            return null;
        }

        public ITMPluginGUI GetPluginGUI()
        {
            return new CraigGTAPluginGUI();
        }

        public ITMPluginNet GetPluginNet()
        {
            return null;
        }
    }
}