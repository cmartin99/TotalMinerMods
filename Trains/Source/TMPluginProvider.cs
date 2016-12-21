using Craig.TotalMiner.API;

public class TMPluginProvider : ITMPluginProvider
{
    public ITMPlugIn GetPlugin()
    {
        return new TrainsMod.TrainsMod();
    }

    public ITMPlugInBlocks GetPluginBlocks()
    {
        return new TrainsMod.TrainsBlocks();
    }
}
