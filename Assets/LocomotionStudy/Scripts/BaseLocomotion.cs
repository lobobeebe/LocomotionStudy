using UnityEngine;
using System.Collections;
using HTC.UnityPlugin.Vive;
using HTC.UnityPlugin.Pointer3D;
using Valve.VR.InteractionSystem;

public class BaseLocomotion : MonoBehaviour
{
    [Tooltip("Select which button to hold to engage PullTheWorld.")]
    public static ControllerButton EngageButton = ControllerButton.Trigger;
    
    public VivePoseTracker LeftHand;
    public VivePoseTracker RightHand;

    protected GameObject mHeadCamera;
    protected ControllerHelper mLeftController;
    protected ControllerHelper mRightController;

    protected virtual void Awake()
    {
        mLeftController = new ControllerHelper(LeftHand);
        mRightController = new ControllerHelper(RightHand);
    }

    protected virtual void OnEnable()
    {
        mHeadCamera = GameObject.FindGameObjectWithTag("MainCamera");
    }

    protected virtual void FixedUpdate()
    {
        mLeftController.FixedUpdate();
        mRightController.FixedUpdate();
    }

    public class ControllerHelper
    {
        public delegate void ActivationHandler();
        public ActivationHandler Activated = delegate { };
        public ActivationHandler Deactivated = delegate { };
        
        private float mTriggerActuation = .1f;
        
        public VivePoseTracker mTrackedController;
        
        private Vector3 mLastPosition;

        public bool mIsSubscribed;
        public bool mIsEngaged;

        public ControllerHelper(VivePoseTracker tracker)
        {
            mTrackedController = tracker;

            mIsEngaged = false;
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

        public void EngageButtonPressed()
        {
            mIsEngaged = true;
            Activated();
        }

        public void EngageButtonReleased()
        {
            mIsEngaged = false;
            Deactivated();
        }

        public Vector3 GetDeltaMovement(bool supressYAxis = true)
        {
            Vector3 deltaMovement = Vector3.zero;

            if (mTrackedController != null)
            {
                if (mIsEngaged)
                {
                    deltaMovement = mLastPosition - mTrackedController.transform.localPosition;

                    if (supressYAxis)
                    {
                        deltaMovement.y = 0;
                    }
                }

                mLastPosition = mTrackedController.transform.localPosition;
            }

            return deltaMovement;
        }

        public virtual void FixedUpdate()
        {
            if (mTrackedController && ViveInput.GetTriggerValue(mTrackedController.viveRole) > mTriggerActuation)
            {
                if (!mIsEngaged)
                {
                    EngageButtonPressed();
                }
            }
            else
            {
                if (mIsEngaged)
                {
                    EngageButtonReleased();
                }
            }
        }
    }
}
