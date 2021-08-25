using StudioForge.TotalMiner.API;

namespace EntitiesMod
{
    public class EntitiesModPluginProvider : ITMPluginProvider
    {
        public ITMPlugin GetPlugin() => new EntitiesModPlugin();
        public ITMPluginBlocks GetPluginBlocks() => null;
        public ITMPluginArcade GetPluginArcade() => null;
        public ITMPluginGUI GetPluginGUI() => null;
        public ITMPluginNet GetPluginNet() => null;
    }
}