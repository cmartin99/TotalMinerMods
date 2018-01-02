using StudioForge.TotalMiner.API;

namespace TotalDefender
{
    public class TMPluginProvider : ITMPluginProvider
    {
        public ITMPlugin GetPlugin()
        {
            return new TotalDefender.TotalDefenderMod();
        }

        public ITMPluginBlocks GetPluginBlocks()
        {
            return null;
        }

        public ITMPluginArcade GetPluginArcade()
        {
            return new TotalDefender.TotalDefenderPlugin();
        }

        public ITMPluginGUI GetPluginGUI()
        {
            return null;
        }
    }
}