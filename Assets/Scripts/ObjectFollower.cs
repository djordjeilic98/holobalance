using HoloKit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFollower : MonoBehaviour
{
    public GameObject objectToFollow;
    public GameObject objectRotationToFollow;
    public Vector3 offset;
    public Vector3 rotation;

    // Start is called before the first frame update
    void Start()
    {
        if (!objectToFollow)
        {
            objectToFollow = HoloKitCamera.Instance.gameObject.transform.parent.gameObject;
            objectRotationToFollow = HoloKitCamera.Instance.gameObject;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 forward = Vector3.Normalize(new Vector3(objectToFollow.transform.forward.x, 0, objectToFollow.transform.forward.z));
        transform.position = objectToFollow.transform.position + Vector3.down * offset.y + forward * offset.x;
        transform.rotation = Quaternion.Euler(rotation.x, rotation.y, -objectRotationToFollow.transform.rotation.eulerAngles.y);
    }
}
