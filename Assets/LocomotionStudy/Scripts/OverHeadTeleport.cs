using HTC.UnityPlugin.StereoRendering;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class OverHeadTeleport : BaseLocomotion
{
    public GameObject Surface;
    public GameObject Reticle;

    public Vector3 SpyGlassOffset;
    public Vector3 SpyGlassRotationOffset;
    
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
        StereoRenderer renderer = Surface.GetComponent<StereoRenderer>();
        renderer.anchorTransform = mSpyCameraLocation.transform;

        Surface.SetActive(false);

        // Reticle
        mReticle = Instantiate(Reticle);
        mReticle.SetActive(false);
    }

    public void OnTeleport()
    {
        if (mReticle && mReticle.activeInHierarchy)
        {
            transform.position = mSpyCameraLocation.transform.position;

            // Fake the Right Controller being released
            mRightController.EngageButtonReleased(); // ??

            // Turn off the SpyGlass
            Surface.SetActive(false);
            mReticle.SetActive(false);
        }
    }

    public void OnActivate()
    {
        Surface.SetActive(true);
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


            NavMeshHit navHit;
            bool open = NavMesh.SamplePosition(hit.point, out navHit, .1f, 1);

            if (!open)
            {
                return;
            }

            if (mSpyCameraLocation)
            {
                mSpyCameraLocation.transform.position = hit.point;
            }

            if (mReticle)
            {
                mReticle.transform.position = hit.point;
            }
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collision");
    }
}
