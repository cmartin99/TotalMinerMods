using StudioForge.TotalMiner.API;

namespace Tree_Feller_v1
{
    public class TMPluginProvider : ITMPluginProvider
    {
        public ITMPlugin GetPlugin()
        {
            return new TreeFellerMod();
        }
        public ITMPluginGUI GetPluginGUI()
        {
            return null;
        }
        public ITMPluginBlocks GetPluginBlocks()
        {
            return null;
        }
        public ITMPluginArcade GetPluginArcade()
        {
            return null;
        }
    }
}