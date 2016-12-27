using System;
using Craig.Engine;
using Craig.BlockWorld;
using Craig.TotalMiner;
using Craig.TotalMiner.Blocks;
using Craig.TotalMiner.API;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics.PackedVector;

namespace VehiclesMod
{
    static class Blocks
    {
        public const Block TrainTrackStraight = Craig.TotalMiner.Block.Retro;
        public const Block TrainTrackCorner = Craig.TotalMiner.Block.CherryMetal;
    }

    class VehiclesBlocks : ITMPluginBlocks
    {
        ITMGame game;

        public void InitializeGame(ITMGame game)
        {
            this.game = game;
        }

        public DataBlock NewDataBlock(GlobalPoint3D p, Block blockID, short playerID)
        {
            return null;
        }

        public bool IsCustomMesh(byte blockID)
        {
            switch ((Block)blockID)
            {
                case Blocks.TrainTrackStraight:
                case Blocks.TrainTrackCorner:
                    return true;

                default:
                    return false;
            }
        }

        public BoundingBox GetBlockBox(GlobalPoint3D p, Block blockID)
        {
            switch ((Block)blockID)
            {
                case Blocks.TrainTrackStraight:
                case Blocks.TrainTrackCorner:
                    var box = GetBlockBoxCore(game.Map, p);
                    box.Max.Y = box.Min.Y + 0.2f;
                    return box;

                default:
                    return game.GetBlockBox(p, blockID);
            }
        }

        BoundingBox GetBlockBoxCore(ITMMap map, GlobalPoint3D p)
        {
            float tilesize = map.TileSize;
            var box = new BoundingBox();
            box.Min.X = p.X * tilesize;
            box.Min.Y = p.Y * tilesize;
            box.Min.Z = p.Z * tilesize;
            box.Max.X = box.Min.X + tilesize;
            box.Max.Y = box.Min.Y + tilesize;
            box.Max.Z = box.Min.Z + tilesize;
            return box;
        }

        public byte GetAuxForPlacement(Vector3 viewDirection, GlobalPoint3D swingTarget, BlockFace swingFace, int facePos, Block blockID)
        {
            switch (blockID)
            {
                case Blocks.TrainTrackStraight:
                    return (byte)(Math.Abs(viewDirection.X) > Math.Abs(viewDirection.Z) ? 0 : 1);

                case Blocks.TrainTrackCorner:
                    return (byte)(facePos & 0x3);

                default:
                    return 0;
            }
        }

        public byte GetMeshBlockID(byte blockID)
        {
            return blockID;
        }

        public void BuildCustomMesh(ITMMeshBuilder meshBuilder, ITMMap map, GlobalPoint3D p, byte blockID)
        {
            switch ((Block)blockID)
            {
                case Blocks.TrainTrackStraight:
                    BuildMeshTrainTrackStraight(meshBuilder, map, p, blockID);
                    break;

                case Blocks.TrainTrackCorner:
                    BuildMeshTrainTrackCorner(meshBuilder, map, p, blockID);
                    break;
            }
        }

        void BuildMeshTrainTrackStraight(ITMMeshBuilder meshBuilder, ITMMap map, GlobalPoint3D p, byte blockID)
        {
            var tilesize = map.TileSize;
            var tc1 = meshBuilder.TexCoords1[blockID];
            var tc2 = meshBuilder.TexCoords2[blockID];
            var tc3 = meshBuilder.TexCoords3[blockID];
            var tc4 = meshBuilder.TexCoords4[blockID];
            meshBuilder.RotateTexCoords(ref p, (byte)BlockFace.Left, ref tc1, ref tc2, ref tc3, ref tc4);

            var data = new AVParams();
            data.Point = p;
            data.BlockID = blockID;
            data.IsCorner = true;
            data.Face = (int)BlockFace.Up;
            data.Pos1 = map.GetPosition(p);
            data.Pos1.Y -= tilesize;
            data.Pos2.X = data.Pos1.X + tilesize;
            data.Pos2.Z = data.Pos1.Z + tilesize;
            data.Pos2.Y = data.Pos1.Y;
            var y1 = data.Pos1.Y;
            var y2 = data.Pos2.Y;
            var y3 = data.Pos2.Y;
            var y4 = data.Pos1.Y;

            #region Raise End
            Block b;
            var aux = map.GetAuxData(p) & 0x01;
            if (aux == 0)
            {
                ++p.X;
                b = (Block)map.GetBlockID(p);
                if (b != Blocks.TrainTrackStraight && b != Blocks.TrainTrackCorner)
                {
                    ++p.Y;
                    b = (Block)map.GetBlockID(p);
                    if (b == Blocks.TrainTrackStraight || b == Blocks.TrainTrackCorner) { y2 += tilesize; y3 += tilesize; }
                }
                else
                {
                    p.X -= 2;
                    b = (Block)map.GetBlockID(p);
                    if (b != Blocks.TrainTrackStraight && b != Blocks.TrainTrackCorner)
                    {
                        ++p.Y;
                        b = (Block)map.GetBlockID(p);
                        if (b == Blocks.TrainTrackStraight || b == Blocks.TrainTrackCorner) { y1 += tilesize; y4 += tilesize; }
                    }
                }
            }
            else
            {
                ++p.Z;
                b = (Block)map.GetBlockID(p);
                if (b != Blocks.TrainTrackStraight && b != Blocks.TrainTrackCorner)
                {
                    ++p.Y;
                    b = (Block)map.GetBlockID(p);
                    if (b == Blocks.TrainTrackStraight || b == Blocks.TrainTrackCorner) { y3 += tilesize; y4 += tilesize; }
                }
                else
                {
                    p.Z -= 2;
                    b = (Block)map.GetBlockID(p);
                    if (b != Blocks.TrainTrackStraight && b != Blocks.TrainTrackCorner)
                    {
                        ++p.Y;
                        b = (Block)map.GetBlockID(p);
                        if (b == Blocks.TrainTrackStraight || b == Blocks.TrainTrackCorner) { y1 += tilesize; y2 += tilesize; }
                    }
                }
            }
            #endregion

            data.X = data.Pos1.X;
            data.Y = y1 + 0.1f;
            data.Z = data.Pos1.Z;
            data.TC = new NormalizedShort2(tc3.X, tc3.Y);
            meshBuilder.AddVertex(ref data);
            data.X = data.Pos2.X;
            data.Y = y2 + 0.1f;
            data.TC = new NormalizedShort2(tc1.X, tc1.Y);
            meshBuilder.AddVertex(ref data);
            data.Z = data.Pos2.Z;
            data.Y = y3 + 0.1f;
            data.TC = new NormalizedShort2(tc2.X, tc2.Y);
            meshBuilder.AddVertex(ref data);
            data.X = data.Pos1.X;
            data.Y = y4 + 0.1f;
            data.TC = new NormalizedShort2(tc4.X, tc4.Y);
            meshBuilder.AddVertex(ref data);
        }

        void BuildMeshTrainTrackCorner(ITMMeshBuilder meshBuilder, ITMMap map, GlobalPoint3D p, byte blockID)
        {
            var tilesize = map.TileSize;
            var tc1 = meshBuilder.TexCoords1[blockID];
            var tc2 = meshBuilder.TexCoords2[blockID];
            var tc3 = meshBuilder.TexCoords3[blockID];
            var tc4 = meshBuilder.TexCoords4[blockID];
            meshBuilder.RotateTexCoords(ref p, (byte)BlockFace.Left, ref tc1, ref tc2, ref tc3, ref tc4);

            var data = new AVParams();
            data.Point = p;
            data.BlockID = blockID;
            data.IsCorner = true;
            data.Face = (int)BlockFace.Up;
            data.Pos1 = map.GetPosition(p);
            data.Pos1.Y -= tilesize;
            data.Pos2.X = data.Pos1.X + tilesize;
            data.Pos2.Z = data.Pos1.Z + tilesize;
            data.Pos2.Y = data.Pos1.Y;

            data.X = data.Pos1.X;
            data.Y = data.Pos1.Y + 0.1f;
            data.Z = data.Pos1.Z;
            data.TC = new NormalizedShort2(tc3.X, tc3.Y);
            meshBuilder.AddVertex(ref data);
            data.X = data.Pos2.X;
            data.TC = new NormalizedShort2(tc1.X, tc1.Y);
            meshBuilder.AddVertex(ref data);
            data.Z = data.Pos2.Z;
            data.TC = new NormalizedShort2(tc2.X, tc2.Y);
            meshBuilder.AddVertex(ref data);
            data.X = data.Pos1.X;
            data.TC = new NormalizedShort2(tc4.X, tc4.Y);
            meshBuilder.AddVertex(ref data);
        }
    }
}
