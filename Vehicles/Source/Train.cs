
namespace VehiclesMod
{
    class Train : Vehicle
    {
        public Train(VehiclesMod mod)
            : base(mod)
        {
            groundBlock = Blocks.TrainTrackStraight;
            groundBlockOffsetY = 0;
            smokeDelay = 0.05f;
        }
    }
}
