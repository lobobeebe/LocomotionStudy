using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// PullTheWorldLocomotion moves the play area in the opposite direction of lateral movement of the tracked controllers.
/// </summary>
public class PullTheWorldLocomotion : BaseLocomotion
{
    public float SpeedScale = 1.5f;

    // The current movement velocity of the player. If not active, it will be set to Vector.zero.
    protected Vector3 mCurrentSpeed = Vector3.zero;
    
    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        // Initialize sum of tracked object movement
        mCurrentSpeed = SpeedScale * (mLeftController.GetDeltaMovement() + mRightController.GetDeltaMovement());
            
        // Move the Play Area
        if (mCurrentSpeed != Vector3.zero)
        {
            MovePlayArea(mCurrentSpeed);
        }
    }
    
    protected virtual void MovePlayArea(Vector3 movementVelocity)
    {
        Vector3 movement = movementVelocity;
        Vector3 finalPosition = new Vector3(movement.x + transform.position.x, transform.position.y, movement.z + transform.position.z);
        transform.position = finalPosition;
    }
}