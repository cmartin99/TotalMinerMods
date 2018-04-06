using StudioForge.TotalMiner.API;

namespace ArcadeMachines
{
    public class ArcadeMachinesPluginProvider : ITMPluginProvider
    {
        public ITMPlugin GetPlugin()
        {
            return new ArcadeMachinesModPlugin();
        }

        public ITMPluginBlocks GetPluginBlocks()
        {
            return null;
        }

        public ITMPluginArcade GetPluginArcade()
        {
            return new ArcadeMachinesPlugin();
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