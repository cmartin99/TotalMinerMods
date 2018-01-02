using StudioForge.TotalMiner.API;

namespace PongArcade
{
    public class TMPluginProvider : ITMPluginProvider
    {
        public ITMPlugin GetPlugin()
        {
            return new PongArcade.PongMod();
        }

        public ITMPluginBlocks GetPluginBlocks()
        {
            return null;
        }

        public ITMPluginArcade GetPluginArcade()
        {
            return new PongArcade.PongPlugin();
        }

        public ITMPluginGUI GetPluginGUI()
        {
            return null;
        }
    }
}