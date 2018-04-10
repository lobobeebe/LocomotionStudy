using System;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

public class AvatarMovement : BaseLocomotion
{
    public GameObject ReticlePrefab;
    public GameObject CharacterPrefab;

    private GameObject mReticle;
    private AICharacterControl mCharacter;
        
    protected override void OnEnable()
    {
        base.OnEnable();

        mRightController.Activated += OnActivate;
        mRightController.Deactivated += OnDeactivate;

        mReticle = Instantiate(ReticlePrefab);
        mReticle.SetActive(false);

        GameObject character = Instantiate(CharacterPrefab);
        mCharacter = character.GetComponent<AICharacterControl>();
        character.SetActive(false);
    }

    public void OnActivate()
    {
        mCharacter.transform.position = transform.position;
        mCharacter.transform.rotation = transform.rotation;

        mCharacter.gameObject.SetActive(true);
        mReticle.SetActive(true);
    }

    public void OnDeactivate()
    {
        mCharacter.gameObject.SetActive(false);
        mReticle.SetActive(false);

        Vector3 finalPosition = mCharacter.transform.position;

        transform.position = finalPosition;// + Vector3.up * transform.position.y;
        transform.rotation = mCharacter.transform.rotation;
        transform.Rotate(Vector3.up, Vector3.Angle(transform.forward, Camera.current.transform.forward));
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

            if (mReticle)
            {
                mReticle.transform.position = hit.point;

                if (mCharacter)
                {
                    mCharacter.SetTarget(mReticle.transform);
                }
            }
        }
    }
}
