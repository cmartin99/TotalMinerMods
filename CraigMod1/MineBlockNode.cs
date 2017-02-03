using System.Collections.Generic;
using System.IO;
using StudioForge.Engine.Core;
using StudioForge.Engine.GUI;
using StudioForge.TotalMiner;
using StudioForge.TotalMiner.AI;

namespace CraigMod1
{
    [BehaviourTreeNodeAttribute("MineBlock", BehaviourTreeNodeType.Action)]
    public class MineBlockNode : BehaviourTreeNode
    {
        [PropertyEditorField(PropertyEditorFieldAttribute.FlagTypes.IsCSV)]
        public List<Block> BlockIDs = new List<Block>();

        public MineBlockNode()
        {
        }

        public MineBlockNode(INPCBehaviour npc)
            : base(npc)
        {
        }

        protected override void UpdateCore(ITMBehaviourExecutionEngine engine)
        {
            Status = BehaviourTreeNodeStatus.Failure;
            if (npc == null) return;

            if (BlockIDs.Count > 0 && npc.SwingTargetIsValid && BlockIDs.Contains(npc.Map.GetBlockID(npc.SwingTarget)))
            {
                npc.StandStill();
                npc.SwingHand(InventoryHand.Right);
                Status = BehaviourTreeNodeStatus.Success;
            }
        }

        #region Serialization

        protected override void ReadStateCore(BinaryReader reader, int version)
        {
            base.ReadStateCore(reader, version);
            BlockIDs.Clear();
            if (version < 285)
                BlockIDs.Add((Block)reader.ReadByte());
            else
                ReadBlockList(reader, BlockIDs);

        }

        protected override void WriteStateCore(BinaryWriter writer)
        {
            base.WriteStateCore(writer);
            WriteBlockList(writer, BlockIDs);
        }

        #endregion
    }
}
