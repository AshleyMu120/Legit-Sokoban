using System.Collections.Generic;
using UnityEngine;

namespace Sokoban
{
    public enum Direction { right = 0, left = 1, up = 2, down = 3, None = 4 }

    public class Block : MonoBehaviour
    {
        public Vector2Int GridPos => _gridObject.gridPosition;
        protected Vector2Int OriginalGridPos;
        protected Vector2 Dimensions => GridMaker.reference.dimensions;
        protected BetterGridObject _gridObject;
        public Vector2Int TargetMovement => _targetMovement;
        private Vector2Int _targetMovement, _targetGridPos;

        protected virtual void Awake()
        {
            _gridObject = GetComponent<BetterGridObject>();
        }

        protected virtual void Update() { }

        private void LateUpdate()
        {
            UpdateData();
            OriginalGridPos = GridPos;
        }

        public virtual void UpdateData() { }

        public bool Move(Direction moveDir)
        {
            if (GridData.Instance.GetBlockData(OriginalGridPos).MovedThisFrame)
            {
                return true;
            }

            if (GridData.Instance.GetBlockData(GridPos).AssessdThisFrame)
            {
                return false;
            }

            DetermineMovement(moveDir);

            bool result = AssessMovement(moveDir);

            if (result)
            {
                MoveStickyNClingy(moveDir);
            }

            return result;
        }

        protected virtual void MoveStickyNClingy(Direction moveDir)
        {
            var stickys = GridData.Instance.GetSurroundingStickys(OriginalGridPos, this);
            var oppositeDir = GetOppositeDir(moveDir);
            var clingy = GridData.Instance.GetClingy(OriginalGridPos, oppositeDir);

            foreach (var sticky in stickys)
            {
                Debug.Log(sticky.gameObject.name);
                sticky.Move(moveDir);
            }

            clingy?.Move(moveDir);
        }

        private void DetermineMovement(Direction moveDir)
        {
            _targetMovement = moveDir switch
            {
                Direction.right => Vector2Int.right,
                Direction.left => Vector2Int.left,
                Direction.up => Vector2Int.up,
                Direction.down => Vector2Int.down,
                _ => Vector2Int.zero
            };

            _targetGridPos = GridPos + _targetMovement;
        }

        protected virtual bool AssessMovement(Direction moveDir)
        {
            GridData.Instance.UpdateBlockData(GridPos, new BlockData(this, false, true));

            Block interactingBlock = GridData.Instance.GetBlockData(_targetGridPos).Block;
            if (interactingBlock != null)
            {
                if (interactingBlock is not Clingy clingy || GridData.Instance.GetBlockData(clingy).MovedThisFrame)
                {
                    if (interactingBlock.Move(moveDir))
                    {
                        Move(_targetGridPos);
                        return true;
                    }
                }
            }
            else if (IsWithinBounds(_targetGridPos))
            {
                Move(_targetGridPos);
                return true;
            }

            return false;
        }

        private bool IsWithinBounds(Vector2Int position)
        {
            return position.x <= Dimensions.x && position.x > 0 && position.y <= Dimensions.y && position.y > 0;
        }

        protected void Move(Vector2Int targetGridPos)
        {
            GridData.Instance.UpdateBlockData(GridPos, new BlockData(this, true, true));
            UndoManager.Instance.ExecuteCommand(new MoveCommand(_gridObject, targetGridPos, OriginalGridPos));
        }

        protected Direction GetOppositeDir(Direction direction)
        {
            return direction switch
            {
                Direction.right => Direction.left,
                Direction.left => Direction.right,
                Direction.up => Direction.down,
                Direction.down => Direction.up,
                _ => Direction.None
            };
        }
    }
}