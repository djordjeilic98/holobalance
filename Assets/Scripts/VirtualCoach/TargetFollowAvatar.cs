using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetFollowAvatar : MonoBehaviour
{
    public float offsetX = 0.0f;
    GameObject avatar = null;
    GameObject head = null;

    // Start is called before the first frame update
    void Start()
    {
        head = GameObject.Find("head");
    }

    // Update is called once per frame
    void Update()
    {
        if(avatar == null)
        {
            avatar = SceneControllerVirtualCoach.avatarCurrent;
            if(avatar != null)
            {
                transform.parent = avatar.transform;
            }
        }
        else
        {
            this.transform.position = new Vector3(avatar.transform.position.x + offsetX, head.transform.position.y, avatar.transform.position.z - 0.75f);
        }
    }
}
