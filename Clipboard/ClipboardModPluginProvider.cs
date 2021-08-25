using StudioForge.TotalMiner.API;

namespace EntitiesMod
{
    public class ClipboardModPluginProvider : ITMPluginProvider
    {
        public ITMPlugin GetPlugin() => new ClipboardModPlugin();
        public ITMPluginBlocks GetPluginBlocks() => null;
        public ITMPluginArcade GetPluginArcade() => null;
        public ITMPluginGUI GetPluginGUI() => null;
        public ITMPluginNet GetPluginNet() => null;
    }
}