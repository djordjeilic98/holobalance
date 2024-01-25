using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowController : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject objectToFollow;
    void Start()
    {
        if (!objectToFollow)
        {
            objectToFollow = Camera.main.gameObject;
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = objectToFollow.transform.position;
        transform.rotation = objectToFollow.transform.rotation;
    }
}
