using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sokoban
{
    public class Sticky : Block
    {
        public Block[] stickingBlocks = new Block[4];

        public override void UpdateData()
        {
            base.UpdateData();
            Sense();
        }

        protected override void MoveStickyNClingy(Direction moveDir)
        {
            var blocks = GridData.Instance.GetSurroundingBlocks(OriginalGridPos, this);
            var oppositeDir = GetOppositeDir(moveDir);
            var clingy = GridData.Instance.GetClingy(OriginalGridPos, oppositeDir);

            MoveSurroundingBlocks(blocks, moveDir);
            MoveClingy(clingy, moveDir);
        }

        private void MoveSurroundingBlocks(Block[] blocks, Direction moveDir)
        {
            foreach (var block in blocks)
            {
                if (block is not Clingy)
                {
                    //Debug.Log(block.gameObject.name);
                    block.Move(moveDir);
                }
            }
        }

        private void MoveClingy(Clingy clingy, Direction moveDir)
        {
            clingy?.Move(moveDir);
        }

        private void Sense()
        {
            stickingBlocks[(int)Direction.right] = GridData.Instance.GetBlockData(GridPos + Vector2Int.right).Block;
            stickingBlocks[(int)Direction.left] = GridData.Instance.GetBlockData(GridPos + Vector2Int.left).Block;
            stickingBlocks[(int)Direction.up] = GridData.Instance.GetBlockData(GridPos + Vector2Int.up).Block;
            stickingBlocks[(int)Direction.down] = GridData.Instance.GetBlockData(GridPos + Vector2Int.down).Block;
        }

        private bool IsStickingOn(Block block)
        {
            foreach (var stickingBlock in stickingBlocks)
            {
                if (stickingBlock == block)
                {
                    return true;
                }
            }
            return false;
        }
    }
}