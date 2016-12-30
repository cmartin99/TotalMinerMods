using Craig.TotalMiner.API;

public class TMPluginProvider : ITMPluginProvider
{
    public ITMPlugin GetPlugin()
    {
        return new CraigMod1.CraigMod1();
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
