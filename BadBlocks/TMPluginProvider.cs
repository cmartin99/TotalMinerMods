using StudioForge.TotalMiner.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadBlocks
{
    class TMPluginProvider : ITMPluginProvider
    {
        public ITMPlugin GetPlugin()
        {
            return new BadBlocks();
        }

        public ITMPluginArcade GetPluginArcade()
        {
            return null;
        }

        public ITMPluginBlocks GetPluginBlocks()
        {
            return null;
        }

        public ITMPluginGUI GetPluginGUI()
        {
            return null;
        }
    }
}
