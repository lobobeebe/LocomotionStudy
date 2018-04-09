using UnityEngine;
using System.Collections;

public class RotateObjectAroundAPoint : MonoBehaviour
{
    public GameObject goToRotate;
    private Transform mTrans;
    public GameObject target;
    private Vector3 targetPosition;
    public float speed;
    private RotateObjectAroundAPoint inst;
    void Start()
    {
        inst = this;
        inst.mTrans = goToRotate.transform;
        inst.targetPosition = target.transform.position;
        inst.mTrans.LookAt(target.transform);
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            speed++;
           //inst.mTrans.RotateAround(targetPosition, Vector3.up, speed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            speed--;
            //inst.mTrans.RotateAround(targetPosition, Vector3.up, speed * Time.deltaTime);
        }

        inst.mTrans.RotateAround(targetPosition, Vector3.up, speed * Time.deltaTime);
    }

    void OnGUI()
    {
        GUI.Label(new Rect(Screen.width - 300 >> 1, Screen.height - 100, 300, 100),
                  "Use the arrow keys to move the camera");
    }
}
