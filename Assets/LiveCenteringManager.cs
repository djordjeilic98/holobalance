using UnityEngine;

public class LiveCenteringManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SetTransform();
    }

    // Update is called once per frame
    void Update()
    {
        SetTransform();
    }

    protected void SetTransform()
    {
        if(CenteringReferenceObject.Instance)
        {
            Transform image = CenteringReferenceObject.Instance.transform;
            transform.position = image.position;
            Vector3 rotation = image.rotation.eulerAngles;
            transform.rotation = Quaternion.Euler(new Vector3(0.0f, rotation.y, 0.0f));
        }
    }
}
