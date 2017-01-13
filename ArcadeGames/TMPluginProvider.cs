using StudioForge.TotalMiner.API;

namespace ArcadeGames
{
    public class TMPluginProvider : ITMPluginProvider
    {
        public ITMPlugin GetPlugin()
        {
            return new ArcadeGames.ArcadeGamesMod();
        }

        public ITMPluginBlocks GetPluginBlocks()
        {
            return null;
        }

        public ITMPluginArcade GetPluginArcade()
        {
            return new ArcadeGames.ArcadePlugin();
        }

        public ITMPluginGUI GetPluginGUI()
        {
            return null;
        }
    }
}