using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class OverHeadTeleport : BaseLocomotion
{
    public GameObject SpyCamera;
    public GameObject SpyGlass;
    public GameObject Reticle;

    public Vector3 SpyGlassOffset;
    public Vector3 SpyGlassRotationOffset;

    private GameObject mSpyCamera;
    private GameObject mSpyGlass;
    private GameObject mReticle;

    private Vector3 mSelectedLocation;

    protected override void OnEnable()
    {
        base.OnEnable();

        mLeftController.Activated += OnTeleport;

        mRightController.Activated += OnActivate;
        mRightController.Deactivated += OnDeactivate;

        mSpyCamera = Instantiate(SpyCamera);
        mSpyGlass = Instantiate(SpyGlass);
        mReticle = Instantiate(Reticle);

        mSpyCamera.SetActive(false);
        mSpyGlass.SetActive(false);
        mReticle.SetActive(false);

        mSelectedLocation = Vector3.zero;
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        mSpyCamera = null;
        mSpyGlass = null;
    }

    public void OnTeleport()
    {
        if (mSelectedLocation != Vector3.zero)
        {
            mPlayArea.transform.position = mSelectedLocation;

            // Fake the Right Controller being released
            mRightController.EngageButtonReleased(null, new ControllerInteractionEventArgs());

            // Reset the selected location
            mSelectedLocation = Vector3.zero;

            // Turn off the SpyCamera/Glass
            mSpyCamera.SetActive(false);
            mSpyGlass.SetActive(false);
        }
    }

    public void OnActivate()
    {
        mSpyCamera.SetActive(true);
        mSpyGlass.SetActive(true);

        mReticle.SetActive(true);
    }

    public void OnDeactivate()
    {
        mReticle.SetActive(false);
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (mRightController.mIsActive)
        {
            Ray ray = new Ray(mRightController.CurrentPosition, mRightController.Forward);
            Debug.DrawRay(mRightController.CurrentPosition, mRightController.Forward);

            RaycastHit hit;
            if (!Physics.Raycast(ray, out hit))
            {
                return;
            }

            mSelectedLocation = hit.point;

            if (mReticle)
            {
                mReticle.transform.position = mSelectedLocation;
            }
        }

        if (mSelectedLocation != Vector3.zero)
        {
            Transform headTransform = VRTK_DeviceFinder.HeadsetTransform();

            if (mSpyCamera)
            {
                mSpyCamera.transform.position = mSelectedLocation + Vector3.up * (headTransform.localPosition.y);
                mSpyCamera.transform.rotation = headTransform.rotation;
            }

            if (mSpyGlass)
            {
                mSpyGlass.transform.rotation = Quaternion.LookRotation(mSpyGlass.transform.position - headTransform.position, Vector3.up);
                mSpyGlass.transform.position = mLeftController.CurrentPosition + (Vector3.up * SpyGlassOffset.y) + (mSpyGlass.transform.forward * SpyGlassOffset.z);
                mSpyGlass.transform.Rotate(SpyGlassRotationOffset);
            }
        }

    }
}
