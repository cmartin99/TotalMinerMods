using Craig.TotalMiner.API;

public class TMPluginProvider : ITMPluginProvider
{
    public ITMPlugin GetPlugin()
    {
        return new Lockpick.Lockpick();
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
}