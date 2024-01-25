using UnityEngine;
using System.Collections;

public class TargetFollowCamera : MonoBehaviour
{
    public float offsetX = 0.0f;
    GameObject mainCamera = null;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = GameObject.Find("MainCamera");
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = new Vector3(mainCamera.transform.position.x + offsetX, 0.0f, mainCamera.transform.position.z + 0.75f);
    }
}
