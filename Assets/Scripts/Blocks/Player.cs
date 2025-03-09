using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sokoban
{
    public class Player : Block
    {
       
        protected override void Awake()
        {
            base.Awake();

            
        }
        protected override void Update()
        {
            base.Update();

            if (!_gridObject.IsMoving)
            {
                // Move Right
                if (Input.GetKeyDown(KeyCode.D))
                {
                    Move(Direction.right);
                }
                // Move Left
                if (Input.GetKeyDown(KeyCode.A))
                {
                    Move(Direction.left);
                }
                // Move Up
                if (Input.GetKeyDown(KeyCode.W))
                {
                    Move(Direction.up);
                }
                // Move Down
                if (Input.GetKeyDown(KeyCode.S))
                {
                    Move(Direction.down);
                }
            }
        }
        protected override void MoveStickyNClingy(Direction moveDir)
        {
            var stickys = GridData.Instance.GetSurroundingStickys(OriginalGridPos, this);
            var oppositeDir = GetOppositeDir(moveDir);
            var clingy = GridData.Instance.GetClingy(OriginalGridPos, oppositeDir);

            for (int i = 0; i < stickys.Length; i++)
            {
             //   Debug.Log(stickys[i].gameObject.name);
                stickys[i].Move(moveDir);
            }

            if (clingy != null)
            {
                clingy.Move(moveDir);
            }
        }
        
    }
}
