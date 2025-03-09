

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sokoban
{
    [System.Serializable]
    public struct BlockData
    {
        public BlockData(Block block)
        {
            Block = block;
            MovedThisFrame = false;
            AssessdThisFrame = false;
        }
        public BlockData(Block block, bool movedThisFrame, bool assessedThisFrame)
        {
            Block = block;
            MovedThisFrame = movedThisFrame;
            AssessdThisFrame = assessedThisFrame;
        }
        public Block Block;
        public bool MovedThisFrame;
        public bool AssessdThisFrame;
    }

    public class GridData : SceneSingleton<GridData>
    {
        public bool Auto = true;
        private GridMaker _gridMaker;
        public Dictionary<Vector2Int, BlockData> AllBlocksOnGridDict;
        private List<Sticky> _stickys = new List<Sticky>();

        private void OnEnable()
        {
            Debug.Log("shitttttttt");
            _gridMaker = FindObjectOfType<GridMaker>();
            SetupData();
        }

        private void Start()
        {
        }

        private void LateUpdate()
        {
            if (Auto)
            {
                GatherData();
            }
        }

        private void SetupData()
        {
            AllBlocksOnGridDict = new Dictionary<Vector2Int, BlockData>();
            var dimensions = _gridMaker.dimensions;
            for (int x = 1; x <= dimensions.x; x++)
            {
                for (int y = 1; y <= dimensions.y; y++)
                {
                    AllBlocksOnGridDict.Add(new Vector2Int(x, y), new BlockData(null, false, false));
                }
            }
        }

        private void GatherData()
        {
            var keys = new List<Vector2Int>(AllBlocksOnGridDict.Keys);
            foreach (Vector2Int key in keys)
            {
                AllBlocksOnGridDict[key] = new BlockData(null, false, false);
            }

            Block[] blocks = GameObject.FindObjectsOfType<Block>();
            for (int i = 0; i < blocks.Length; i++)
            {
                if (AllBlocksOnGridDict.ContainsKey(blocks[i].GridPos))
                {
                    AllBlocksOnGridDict[blocks[i].GridPos] = new BlockData(blocks[i]);
                }
                else
                {
                    Debug.LogWarning($"{blocks[i].gameObject.name} is out of grid");
                }
            }

            _stickys.Clear();
            foreach (var block in AllBlocksOnGridDict)
            {
                if (block.Value.Block is Sticky sticky)
                {
                    _stickys.Add(sticky);
                }
            }
        }

        public BlockData GetBlockData(Vector2Int gridPos)
        {
            if (AllBlocksOnGridDict.ContainsKey(gridPos))
            {
                return AllBlocksOnGridDict[gridPos];
            }
            return new BlockData(null, true, true);
        }

        public BlockData GetBlockData(Block block)
        {
            var keys = new List<Vector2Int>(AllBlocksOnGridDict.Keys);
            foreach (Vector2Int key in keys)
            {
                if (AllBlocksOnGridDict[key].Block == block)
                {
                    return AllBlocksOnGridDict[key];
                }
            }
            return new BlockData(null, true, true);
        }

        public void UpdateBlockData(Vector2Int gridPos, BlockData newBlockData)
        {
            if (AllBlocksOnGridDict.ContainsKey(gridPos))
            {
                AllBlocksOnGridDict[gridPos] = newBlockData;
            }
        }

        public Sticky[] GetSurroundingStickys(Vector2Int gridPos, Block block)
        {
            List<Sticky> result = new List<Sticky>();
            var detectedPos = new Vector2Int[4];
            detectedPos[0] = gridPos + Vector2Int.right;
            detectedPos[1] = gridPos + Vector2Int.left;
            detectedPos[2] = gridPos + Vector2Int.up;
            detectedPos[3] = gridPos + Vector2Int.down;

            for (int i = 0; i < detectedPos.Length; i++)
            {
                if (AllBlocksOnGridDict.ContainsKey(detectedPos[i]))
                {
                    if (AllBlocksOnGridDict[detectedPos[i]].Block != block && AllBlocksOnGridDict[detectedPos[i]].Block is Sticky sticky && !AllBlocksOnGridDict[detectedPos[i]].MovedThisFrame)
                    {
                        result.Add(sticky);
                    }
                }
            }
            return result.ToArray();
        }

        public Block[] GetSurroundingBlocks(Vector2Int gridPos, Block block)
        {
            List<Block> result = new List<Block>();
            var detectedPos = new Vector2Int[4];
            detectedPos[0] = gridPos + Vector2Int.right;
            detectedPos[1] = gridPos + Vector2Int.left;
            detectedPos[2] = gridPos + Vector2Int.up;
            detectedPos[3] = gridPos + Vector2Int.down;

            for (int i = 0; i < detectedPos.Length; i++)
            {
                if (AllBlocksOnGridDict.ContainsKey(detectedPos[i]))
                {
                    if (AllBlocksOnGridDict[detectedPos[i]].Block != null && AllBlocksOnGridDict[detectedPos[i]].Block != block && !AllBlocksOnGridDict[detectedPos[i]].MovedThisFrame)
                    {
                        result.Add(AllBlocksOnGridDict[detectedPos[i]].Block);
                    }
                }
            }
            return result.ToArray();
        }

        public Clingy GetClingy(Vector2Int gridPos, Direction oppositeDir)
        {
            Vector2Int detectPos = Vector2Int.zero;
            switch (oppositeDir)
            {
                case Direction.right:
                    detectPos = gridPos + Vector2Int.right;
                    break;
                case Direction.left:
                    detectPos = gridPos + Vector2Int.left;
                    break;
                case Direction.up:
                    detectPos = gridPos + Vector2Int.up;
                    break;
                case Direction.down:
                    detectPos = gridPos + Vector2Int.down;
                    break;
            }

            if (AllBlocksOnGridDict.ContainsKey(detectPos))
            {
                if (AllBlocksOnGridDict[detectPos].Block is Clingy clingy)
                {
                    return clingy;
                }
            }
            return null;
        }

        public Clingy[] GetSurroundingClingies(Vector2Int gridPos, Direction moveDir)
        {
            List<Clingy> result = new List<Clingy>();
            var detectedPos = new Vector2Int[4];
            detectedPos[0] = gridPos + Vector2Int.right;
            detectedPos[1] = gridPos + Vector2Int.left;
            detectedPos[2] = gridPos + Vector2Int.up;
            detectedPos[3] = gridPos + Vector2Int.down;

            for (int i = 0; i < detectedPos.Length; i++)
            {
                if (AllBlocksOnGridDict.ContainsKey(detectedPos[i]))
                {
                    if (AllBlocksOnGridDict[detectedPos[i]].Block is Clingy clingy && !AllBlocksOnGridDict[detectedPos[i]].MovedThisFrame)
                    {
                        result.Add(clingy);
                    }
                }
            }
            return result.ToArray();
        }
    }

    public class UndoManager : SceneSingleton<UndoManager>
    {
        private List<ICommand> _commandAtFrame = new List<ICommand>();
        private Stack<ICommand[]> commandsStack = new Stack<ICommand[]>();
        private Stack<ICommand[]> redoCommandsStack = new Stack<ICommand[]>();

        public void ExecuteCommand(ICommand command)
        {
            command.Execute();
            _commandAtFrame.Add(command);
        }

        public void Undo()
        {
            if (commandsStack.Count > 0)
            {
                ICommand[] lastCommands = commandsStack.Pop();
                for (int i = 0; i < lastCommands.Length; i++)
                {
                    lastCommands[i].Undo();
                }
                redoCommandsStack.Push(lastCommands);
            }
        }

        public void Redo()
        {
            if (redoCommandsStack.Count > 0)
            {
                ICommand[] lastCommands = redoCommandsStack.Pop();
                for (int i = 0; i < lastCommands.Length; i++)
                {
                    lastCommands[i].Execute();
                }
                commandsStack.Push(lastCommands);
            }
        }
    }

    public interface ICommand
    {
        void Execute();
        void Undo();
    }

    public class MoveCommand : ICommand
    {
        private BetterGridObject _gridObject;
        private Vector2Int _targetPos;
        private Vector2Int _originalPos;

        public MoveCommand(BetterGridObject gridObject, Vector2Int targetPos, Vector2Int originalPos)
        {
            _gridObject = gridObject;
            _targetPos = targetPos;
            _originalPos = originalPos;
        }

        public void Execute()
        {
            _gridObject.gridPosition = _targetPos;
        }

        public void Undo()
        {
            _gridObject.gridPosition = _originalPos;
        }
    }
}