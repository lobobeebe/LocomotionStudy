using System;
using UnityEngine;
using VRTK;

namespace UnityStandardAssets.SceneUtils
{
    public class AvatarMovement : BaseLocomotion
    {
        public GameObject Reticle;
        public Characters.ThirdPerson.AICharacterControl Character;
        
        protected override void OnEnable()
        {
            base.OnEnable();

            mRightController.Activated += OnActivate;
            mRightController.Deactivated += OnDeactivate;
        }

        public void OnActivate()
        {
            Transform headTransform = VRTK_DeviceFinder.HeadsetTransform();
            Character.transform.position = headTransform.position;
            Character.transform.rotation = headTransform.rotation;

            Character.gameObject.SetActive(true);
        }

        public void OnDeactivate()
        {
            Character.gameObject.SetActive(false);

            Vector3 finalPosition = Character.transform.position;
            
            if (mPlayArea != null)
            {
                mPlayArea.position = finalPosition;
                mPlayArea.rotation = Character.transform.rotation;
                mPlayArea.Rotate(Vector3.up, Vector3.Angle(mPlayArea.forward, VRTK_DeviceFinder.HeadsetTransform().forward));
            }
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

                if (Reticle)
                {
                    Reticle.transform.position = hit.point;

                    if (Character)
                    {
                        Character.SetTarget(Reticle.transform);
                    }
                }
            }
        }
    }
}
