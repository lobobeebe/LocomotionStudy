using HTC.UnityPlugin.StereoRendering;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverHeadTeleport : BaseLocomotion
{
    public GameObject SpyGlass;
    public GameObject Reticle;

    public Vector3 SpyGlassOffset;
    public Vector3 SpyGlassRotationOffset;
    
    private GameObject mSpyGlass;
    private GameObject mReticle;
    private GameObject mSpyCameraLocation;
    
    protected override void OnEnable()
    {
        base.OnEnable();

        // Controller Subscriptions
        mLeftController.Activated += OnTeleport;

        mRightController.Activated += OnActivate;
        mRightController.Deactivated += OnDeactivate;

        // Spy Camera Location
        mSpyCameraLocation = new GameObject("Spy Camera Location");
        mSpyCameraLocation.transform.parent = null;

        // Spy Glass
        mSpyGlass = Instantiate(SpyGlass, mLeftController.mTrackedController.transform);
        mSpyGlass.transform.localPosition = SpyGlassOffset;
        mSpyGlass.transform.localRotation = Quaternion.Euler(SpyGlassRotationOffset);

        StereoRenderer renderer = mSpyGlass.GetComponent<StereoRenderer>();
        renderer.anchorTransform = mSpyCameraLocation.transform;

        mSpyGlass.SetActive(false);

        // Reticle
        mReticle = Instantiate(Reticle);
        mReticle.SetActive(false);
    }

    protected void OnDisable()
    {
        mSpyGlass = null;
        mReticle = null;
    }

    public void OnTeleport()
    {
        if (mReticle.activeInHierarchy)
        {
            transform.position = mReticle.transform.position;

            // Fake the Right Controller being released
            mRightController.EngageButtonReleased(); // ??

            // Turn off the SpyGlass
            mSpyGlass.SetActive(false);
            mReticle.SetActive(false);
        }
    }

    public void OnActivate()
    {
        mSpyGlass.SetActive(true);
        mReticle.SetActive(true);
    }

    public void OnDeactivate()
    {
    }

    // Update is called once per frame
    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (mRightController.mIsEngaged)
        {
            Ray ray = new Ray(mRightController.CurrentPosition, mRightController.Forward);
            Debug.DrawRay(mRightController.CurrentPosition, mRightController.Forward);

            RaycastHit hit;
            if (!Physics.Raycast(ray, out hit))
            {
                return;
            }

            if (mSpyCameraLocation)
            {
                mSpyCameraLocation.transform.position = hit.point;
                //mSpyCameraLocation.transform.rotation = mHeadCamera.transform.rotation;
            }

            if (mReticle)
            {
                mReticle.transform.position = hit.point;
            }
        }
    }
}
