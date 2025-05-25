using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;

namespace Modules.Code.Interfaces.Modules
{
    public interface IBlockEntityBehaviorModule
    {
        public static abstract bool IsApplicableTo(BlockEntity blockEntity);

        public static abstract void RandomizeAttributes(ITreeAttribute attributes); //TODO try to make virtual
    }
}