using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;

namespace Modules.Code.Interfaces.Modules
{
    /// <summary>
    /// The interface all Block Entity Modules should inherit to work
    /// </summary>
    public interface IBlockEntityBehaviorModule
    {
        public static abstract bool IsApplicableTo(BlockEntity blockEntity);

        public static abstract void RandomizeAttributes(ITreeAttribute attributes);
    }
}