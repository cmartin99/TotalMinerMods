using Craig.TotalMiner.API;

public class TMPluginProvider : ITMPluginProvider
{
    public ITMPlugin GetPlugin()
    {
        return new VehiclesMod.VehiclesMod();
    }

    public ITMPluginBlocks GetPluginBlocks()
    {
        return new VehiclesMod.VehiclesBlocks();
    }

    public ITMPluginArcade GetPluginArcade()
    {
        return null;
    }

    public ITMPluginGUI GetPluginGUI()
    {
        return new VehiclesMod.GUIPlugin();
    }
}
