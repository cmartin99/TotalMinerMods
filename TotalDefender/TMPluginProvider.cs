using StudioForge.TotalMiner.API;

namespace TotalDefenderArcade
{
    public class TMPluginProvider : ITMPluginProvider
    {
        public ITMPlugin GetPlugin()
        {
            return new TotalDefenderMod();
        }

        public ITMPluginBlocks GetPluginBlocks()
        {
            return null;
        }

        public ITMPluginArcade GetPluginArcade()
        {
            return new TotalDefenderPlugin();
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