using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace McJagg.map
{
    public class FlashingBlock
    {
        public readonly Block block;
        public int flashState0to16;
        public readonly int flashToBlockType = -1;
        public readonly int flashOnTime0to16 = 10;
        /*public FlashingBlock(Block block, int flashOnTime0to16)
        {
            this.block = block;
            this.flashState0to16 = 0;
            this.flashOnTime0to16 = flashOnTime0to16;
        }*/
        public FlashingBlock(Block block, int flashToBlockType, int flashOnTime0to16)
        {
            this.block = block;
            this.flashState0to16 = 0;
            this.flashToBlockType = flashToBlockType;
            this.flashOnTime0to16 = flashOnTime0to16;
        }
    }
}
