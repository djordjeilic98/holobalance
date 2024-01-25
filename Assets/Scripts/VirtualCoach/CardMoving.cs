using UnityEngine;
using System.Collections;

public class CardMoving : MonoBehaviour
{
    GameObject mainCamera = null;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = GameObject.Find("MainCamera");
    }

    // Update is called once per frame
    void Update()
    {
        float eulerY = mainCamera.transform.rotation.eulerAngles.y;
        float posX = 0.0f;
        if (eulerY > 90)
            posX = 360f - eulerY;
        else
            posX = eulerY * -1.0f;

        this.transform.position = new Vector3(posX * 0.05f, mainCamera.transform.position.y, mainCamera.transform.position.z + 1.75f);
    }
}
