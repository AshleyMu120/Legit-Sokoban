using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sokoban
{

[ExecuteInEditMode]
public class BetterGridObject : MonoBehaviour
{
    public Vector2Int gridPosition;
    private Vector2Int prevGridPosition;
    
    public bool IsMoving => _isMoving;
    public float Move1GridDuration = 0.1f;
    private Vector3 _targetPos;
    private bool _isMoving;
    private float _moveSpeed;
    private void Update()
    {
        if(_isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, _targetPos, _moveSpeed * Time.deltaTime);
            if((transform.position - _targetPos).magnitude < 0.1f)
            {
                transform.position = _targetPos;
                _isMoving = false;
            }
            else
            {
                return;
            }
        }

        //If our position hasn't been updated, don't 
        if (gridPosition == prevGridPosition)
            return;

        //Move to the new position
        UpdatePosition();

        //Keep track of our previous position
        prevGridPosition = gridPosition;
    }

    private void UpdatePosition()
    {
        float x = GridMaker.reference.TopLeft.x + GridMaker.reference.cellWidth * (gridPosition.x - 0.5f); 
        float y = GridMaker.reference.BottomRight.y + GridMaker.reference.cellWidth * (gridPosition.y - 0.5f);

        x = Mathf.Clamp(x, GridMaker.reference.TopLeft.x + 0.5f, GridMaker.reference.BottomRight.x - 0.5f);
        y = Mathf.Clamp(y, GridMaker.reference.BottomRight.y + 0.5f, GridMaker.reference.TopLeft.y - 0.5f);

        _targetPos = new Vector3(x, y); 
        _isMoving = true;
        _moveSpeed = (_targetPos - transform.position).magnitude / Move1GridDuration;
    }
}

}
