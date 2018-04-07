using UnityEngine;
using System.Collections;
using VRTK;

public class BaseLocomotion : MonoBehaviour
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
    
    protected virtual void Awake()
    {
        VRTK_SDKManager.instance.AddBehaviourToToggleOnLoadedSetupChange(this);
        UnityEngine.XR.InputTracking.disablePositionalTracking = true;

        mLeftController = new ControllerHelper();
        mRightController = new ControllerHelper();
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

    protected virtual void OnDestroy()
    {
        VRTK_SDKManager.instance.RemoveBehaviourToToggleOnLoadedSetupChange(this);
    }

    protected virtual void OnDisable()
    {
        if (mLeftController != null)
        {
            mLeftController.Unsubscribe(EngageButton);
        }

        if (mRightController != null)
        {
            mRightController.Unsubscribe(EngageButton);
        }

        mLeftController = null;
        mRightController = null;
        mPlayArea = null;
    }

    protected virtual void OnEnable()
    {
        BodyPhysics = (BodyPhysics != null ? BodyPhysics : GetComponentInChildren<VRTK_BodyPhysics>());

        mLeftController.UpdateController(VRTK_DeviceFinder.GetControllerLeftHand());
        mRightController.UpdateController(VRTK_DeviceFinder.GetControllerRightHand());

        mLeftController.Subscribe(EngageButton);
        mRightController.Subscribe(EngageButton);

        mPlayArea = VRTK_DeviceFinder.PlayAreaTransform();

        if (mPlayArea == null)
        {
            VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.SDK_OBJECT_NOT_FOUND, "PlayArea", "Boundaries SDK"));
        }
    }

    public class ControllerHelper
    {
        public delegate void ActivationHandler();
        public ActivationHandler Activated = delegate { };
        public ActivationHandler Deactivated = delegate { };

        private GameObject mControllerEventsObject;
        private GameObject mTrackedController;
        
        private Vector3 mLastPosition;

        public bool mIsSubscribed;
        public bool mIsActive;

        public ControllerHelper()
        {
            mIsActive = false;
            mIsSubscribed = false;
        }

        public Vector3 CurrentPosition
        {
            get
            {
                if (mTrackedController != null)
                {
                    return mTrackedController.transform.position;
                }

                return Vector3.zero;
            }
        }

        public Vector3 Forward
        {
            get
            {
                if (mTrackedController != null)
                {
                    return mTrackedController.transform.forward;
                }

                return Vector3.zero;
            }
        }

        public void UpdateController(GameObject controllerEventsObject)
        {
            mControllerEventsObject = controllerEventsObject;
            mTrackedController = VRTK_DeviceFinder.GetActualController(mControllerEventsObject);

            if (mTrackedController)
            {
                mLastPosition = mTrackedController.transform.position;
            }
        }

        public void EngageButtonPressed(object sender, ControllerInteractionEventArgs e)
        {
            mIsActive = true;
            Activated();
        }

        public void EngageButtonReleased(object sender, ControllerInteractionEventArgs e)
        {
            mIsActive = false;
            Deactivated();
        }

        public Vector3 GetDeltaMovement(bool supressYAxis = true)
        {
            Vector3 deltaMovement = Vector3.zero;

            if (mTrackedController != null)
            {
                Vector3 currentPostion = mTrackedController.transform.localPosition;

                if (mIsActive)
                {
                    deltaMovement = mLastPosition - currentPostion;

                    if (supressYAxis)
                    {
                        deltaMovement.y = 0;
                    }
                }

                mLastPosition = currentPostion;
            }

            return deltaMovement;
        }

        public void Subscribe(VRTK_ControllerEvents.ButtonAlias engageButton)
        {
            if (!mIsSubscribed && mControllerEventsObject != null)
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
            if (mIsSubscribed && mControllerEventsObject != null)
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
