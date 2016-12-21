using Craig.TotalMiner.API;

public class TMPluginProvider : ITMPluginProvider
{
    public ITMPlugIn GetPlugin()
    {
        return new VehiclesMod.VehiclesMod();
    }

    public ITMPlugInBlocks GetPluginBlocks()
    {
        return new VehiclesMod.VehiclesBlocks();
    }
}
