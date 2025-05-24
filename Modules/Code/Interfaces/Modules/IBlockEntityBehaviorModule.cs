using InsanityLib.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;

namespace Modules.Code.Interfaces.Modules
{
    public interface IBlockEntityBehaviorModule
    {
        public static abstract bool IsApplicableTo(BlockEntity blockEntity);

        public static abstract void RandomizeAttributes(ITreeAttribute attributes);
    }
}
