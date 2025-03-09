using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sokoban
{
    public class Smooth : Block
    {
        protected override void MoveStickyNClingy(Direction moveDir)
        {
            var stickys = GridData.Instance.GetSurroundingStickys(OriginalGridPos, this);

            for (int i = 0; i < stickys.Length; i++)
            {
              
                stickys[i].Move(moveDir);
            }
        }
    }
}
