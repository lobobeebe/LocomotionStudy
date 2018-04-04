using UnityEngine;
using System.Collections.Generic;
using VRTK;

/// <summary>
/// PullTheWorldLocomotion moves the play area in the opposite direction of lateral movement of the tracked controllers.
/// </summary>
public class PullTheWorldLocomotion : MonoBehaviour
{
    [Header("Control Settings")]

    [Tooltip("Select which button to hold to engage PullTheWorld.")]
    public VRTK_ControllerEvents.ButtonAlias EngageButton = VRTK_ControllerEvents.ButtonAlias.TriggerPress;

    [Header("Speed Settings")]

    [Tooltip("Lower to decrease speed, raise to increase.")]
    public float SpeedScale = 1;

    [Header("Custom Settings")]
    [Tooltip("An optional Body Physics script to check for potential collisions in the moving direction. If any potential collision is found then the move will not take place. This can help reduce collision tunnelling.")]
    public VRTK_BodyPhysics BodyPhysics;

    protected Transform mPlayArea;

    protected ControllerHelper mLeftController;
    protected ControllerHelper mRightController;
    
    // The current movement velocity of the player. If not active, it will be set to Vector.zero.
    protected Vector3 mCurrentSpeed;

    protected virtual void Awake()
    {
        VRTK_SDKManager.instance.AddBehaviourToToggleOnLoadedSetupChange(this);
    }

    protected virtual void OnEnable()
    {
        mCurrentSpeed = Vector3.zero;

        BodyPhysics = (BodyPhysics != null ? BodyPhysics : GetComponentInChildren<VRTK_BodyPhysics>());

        mLeftController = new ControllerHelper(VRTK_DeviceFinder.GetControllerLeftHand());
        mRightController = new ControllerHelper(VRTK_DeviceFinder.GetControllerRightHand());

        mLeftController.Subscribe(EngageButton);
        mRightController.Subscribe(EngageButton);

        mPlayArea = VRTK_DeviceFinder.PlayAreaTransform();

        if (mPlayArea == null)
        {
            VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.SDK_OBJECT_NOT_FOUND, "PlayArea", "Boundaries SDK"));
        }
    }

    protected virtual void OnDisable()
    {
        mLeftController.Unsubscribe(EngageButton);
        mRightController.Unsubscribe(EngageButton);

        mLeftController = null;
        mRightController = null;
        mPlayArea = null;
    }

    protected virtual void OnDestroy()
    {
        VRTK_SDKManager.instance.RemoveBehaviourToToggleOnLoadedSetupChange(this);
    }

    protected virtual void FixedUpdate()
    {
        // Initialize sum of tracked object movement
        mCurrentSpeed = SpeedScale * (mLeftController.GetDeltaMovement() + mRightController.GetDeltaMovement());
            
        // Reset previous positions
        mLeftController.ResetPreviousPosition();
        mRightController.ResetPreviousPosition();

        // Move the Play Area
        MovePlayArea(mCurrentSpeed);
    }
    
    protected virtual void MovePlayArea(Vector3 movementVelocity)
    {
        Vector3 movement = movementVelocity * Time.fixedDeltaTime;
        Vector3 finalPosition = new Vector3(movement.x + mPlayArea.position.x, mPlayArea.position.y, movement.z + mPlayArea.position.z);
        if (mPlayArea != null && CanMove(BodyPhysics, mPlayArea.position, finalPosition))
        {
            mPlayArea.position = finalPosition;
        }
    }

    protected virtual bool CanMove(VRTK_BodyPhysics givenBodyPhysics, Vector3 currentPosition, Vector3 proposedPosition)
    {
        if (givenBodyPhysics == null)
        {
            return true;
        }

        Vector3 proposedDirection = (proposedPosition - currentPosition).normalized;
        float distance = Vector3.Distance(currentPosition, proposedPosition);
        return !givenBodyPhysics.SweepCollision(proposedDirection, distance);
    }

    public class ControllerHelper
    {
        private GameObject mControllerEventsObject;
        private GameObject mTrackedController;

        private Vector3 mLastPosition;

        public bool mIsSubscribed;
        public bool mIsActive;

        public ControllerHelper(GameObject controllerEventsObject)
        {
            mControllerEventsObject = controllerEventsObject;
            mTrackedController = VRTK_DeviceFinder.GetActualController(mControllerEventsObject);

            mLastPosition = mTrackedController.transform.position;

            mIsSubscribed = false;
        }

        public void EngageButtonPressed(object sender, ControllerInteractionEventArgs e)
        {
            mLastPosition = mTrackedController.transform.position;

            mIsActive = true;
        }

        public void EngageButtonReleased(object sender, ControllerInteractionEventArgs e)
        {
            mIsActive = false;
        }

        public Vector3 GetDeltaMovement(bool supressYAxis = true)
        {
            Vector3 deltaMovement = Vector3.zero;

            if (mIsActive)
            {
                deltaMovement = mTrackedController.transform.position - mLastPosition;

                if (supressYAxis)
                {
                    deltaMovement.y = 0;
                }
            }

            return deltaMovement;
        }

        public void ResetPreviousPosition()
        {
            mLastPosition = mTrackedController.transform.position;
        }

        public void Subscribe(VRTK_ControllerEvents.ButtonAlias engageButton)
        {
            if (!mIsSubscribed)
            {
                VRTK_ControllerEvents controllerEvent = mControllerEventsObject.GetComponent<VRTK_ControllerEvents>();

                if (controllerEvent)
                {
                    controllerEvent.SubscribeToButtonAliasEvent(engageButton, true, EngageButtonPressed);
                    controllerEvent.SubscribeToButtonAliasEvent(engageButton, false, EngageButtonReleased);
                    mIsSubscribed = true;
                }
            }
        }

        public void Unsubscribe(VRTK_ControllerEvents.ButtonAlias engageButton)
        {
            if (mIsSubscribed)
            {
                VRTK_ControllerEvents controllerEvent = mControllerEventsObject.GetComponent<VRTK_ControllerEvents>();

                if (controllerEvent)
                {
                    controllerEvent.UnsubscribeToButtonAliasEvent(engageButton, true, EngageButtonPressed);
                    controllerEvent.UnsubscribeToButtonAliasEvent(engageButton, false, EngageButtonReleased);
                    mIsSubscribed = false;
                }
            }
        }
    }
}