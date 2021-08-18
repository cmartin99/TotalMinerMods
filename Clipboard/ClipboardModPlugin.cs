using StudioForge.BlockWorld;
using StudioForge.TotalMiner;
using StudioForge.TotalMiner.API;
using Microsoft.Xna.Framework.Graphics;

namespace EntitiesMod
{
    class ClipboardModPlugin : ITMPlugin
    {
        BlockFace pasteFacing;
        GlobalPoint3D pastePoint;
        bool shouldPaste;
        ITMMap pasteMap;
        ITMGame game;

        #region ITMPlugin

        void ITMPlugin.UnloadMod()
        {
        }

        void ITMPlugin.WorldSaved(int version)
        {
        }

        void ITMPlugin.PlayerJoined(ITMPlayer player)
        {
        }

        void ITMPlugin.PlayerLeft(ITMPlayer player)
        {
        }

        void ITMPlugin.InitializeGame(ITMGame game)
        {
            this.game = game;
            game.World.ComponentPasted += OnComponentPasted;
            game.AddEventItemSwing(Item.DebugTool, Paste);
        }

        void ITMPlugin.Draw(ITMPlayer player, ITMPlayer virtualPlayer, Viewport vp)
        {
        }

        bool ITMPlugin.HandleInput(ITMPlayer player)
        {
            return false;
        }

        int frame;

        void ITMPlugin.Update(ITMPlayer player)
        {
            if (shouldPaste)
            {
                player.PasteCurrentClipboard(pastePoint, pasteFacing);
                shouldPaste = false;
            }

            ++frame;
        }

        void ITMPlugin.Update()
        {
        }

        void ITMPlugin.Callback(string data, GlobalPoint3D? p, ITMActor actor, ITMActor contextActor)
        {
            game.AddNotification($"I was called back with {data}");
        }

        void Paste(Item itemID, ITMHand hand)
        {
            pastePoint.X += pasteMap.MapSize.X;
            shouldPaste = true;
        }

        void OnComponentPasted(ITMPlayer player, ITMMap map, GlobalPoint3D p, BlockFace facing)
        {
            pasteMap = map;
            pastePoint = p;
            pasteFacing = facing;
        }

        #endregion

        public static string Path;

        public void Initialize(ITMPluginManager mgr, string path)
        {
            Path = path;
            var typeCounts = new EnumTypeOffsets();
            mgr.RegisterEnumCounts(typeCounts);
        }
    }
}
