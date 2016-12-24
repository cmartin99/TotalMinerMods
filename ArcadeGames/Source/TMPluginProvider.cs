using Craig.TotalMiner.API;

public class TMPluginProvider : ITMPluginProvider
{
    public ITMPlugIn GetPlugin()
    {
        return new ArcadeGames.ArcadeGamesMod();
    }

    public ITMPlugInBlocks GetPluginBlocks()
    {
        return null;
    }

    public ITMPlugInArcade GetPluginArcade()
    {
        return new ArcadeGames.ArcadePlugin();
    }

    public ITMPlugInGUI GetPluginGUI()
    {
        return null;
    }
}
