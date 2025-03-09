using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sokoban
{
    public class Wall : Block
    {
        protected override bool AssessMovement(Direction moveDir)
        {
            return false;
        }
    }
}
