using StudioForge.TotalMiner.API;

namespace TotalRobo
{
    public class TMPluginProvider : ITMPluginProvider
    {
        public ITMPlugin GetPlugin()
        {
            return new TotalRoboMod();
        }

        public ITMPluginBlocks GetPluginBlocks()
        {
            return null;
        }

        public ITMPluginArcade GetPluginArcade()
        {
            return new TotalRoboPlugin();
        }

        public ITMPluginGUI GetPluginGUI()
        {
            return null;
        }

        public ITMPluginNet GetPluginNet()
        {
            return null;
        }
    }
}