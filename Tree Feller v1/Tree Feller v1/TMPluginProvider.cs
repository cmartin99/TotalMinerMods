using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StudioForge.TotalMiner.API;

public class TMPluginProvider : ITMPluginProvider
{
    public ITMPluginGUI GetPluginGUI()
    {
        return null;
    }
    public ITMPluginBlocks GetPluginBlocks()
    {
        return null;
    }
    public ITMPluginArcade GetPluginArcade()
    {
        return null;
    }
    public ITMPlugin GetPlugin()
    {
        return new Tree_Feller_v1.Class1();
    }
}
