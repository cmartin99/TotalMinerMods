using StudioForge.TotalMiner.API;

namespace EntitiesMod
{
    public class EntitiesModPluginProvider : ITMPluginProvider
    {
        public ITMPlugin GetPlugin()
        {
            return new EntitiesModPlugin();
        }

        public ITMPluginBlocks GetPluginBlocks()
        {
            return null;
        }

        public ITMPluginArcade GetPluginArcade()
        {
            return null;
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