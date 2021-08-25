using StudioForge.TotalMiner.API;

namespace EntitiesMod
{
    public class TrafficModPluginProvider : ITMPluginProvider
    {
        public ITMPlugin GetPlugin() => new TrafficModPlugin();
        public ITMPluginBlocks GetPluginBlocks() => null;
        public ITMPluginArcade GetPluginArcade() => null;
        public ITMPluginGUI GetPluginGUI() => null;
        public ITMPluginNet GetPluginNet() => null;
    }
}