using UnityEngine;
using System.Collections.Generic;
using VRTK;

/// <summary>
/// PullTheWorldLocomotion moves the play area in the opposite direction of lateral movement of the tracked controllers.
/// </summary>
public class PullTheWorldLocomotion : BaseLocomotion
{
    // The current movement velocity of the player. If not active, it will be set to Vector.zero.
    protected Vector3 mCurrentSpeed;

    protected override void OnEnable()
    {
        base.OnEnable();

        mCurrentSpeed = Vector3.zero;
    }

    protected virtual void FixedUpdate()
    {
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
        if (mPlayArea)
        {
            Vector3 movement = movementVelocity;
            Vector3 finalPosition = new Vector3(movement.x + mPlayArea.position.x, mPlayArea.position.y, movement.z + mPlayArea.position.z);
            if (mPlayArea != null && CanMove(BodyPhysics, mPlayArea.position, finalPosition))
            {
                mPlayArea.position = finalPosition;
            }
        }
    }
}