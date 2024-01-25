using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkControl : MonoBehaviour
{
    public static BlinkControl Instance = null;

    protected int blendShapeIndex = 0;
    protected bool closeEyes = false;
    protected bool openEyes = false;

    SkinnedMeshRenderer skinnedMeshRenderer;

    // Start is called before the first frame update
    void Start()
    {
        skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        string avatar = PlayerPrefs.GetString("Avatar");
        if (avatar == "Male")
            blendShapeIndex = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex("Genesis8Male__eCTRLEyesClosed");
        else if(avatar == "Female")
            blendShapeIndex = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex("Genesis8Female__eCTRLEyesClosed");

        Instance = this;
    }

    public void OpenEyes()
    {
        openEyes = true;
    }

    public void CloseEyes()
    {
        closeEyes = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(closeEyes) Blink();
        if(openEyes) Blink(false);
    }

    void Blink(bool close = true)
    {
        float weight = skinnedMeshRenderer.GetBlendShapeWeight(blendShapeIndex);
        int factor = close == true ? 1 : -1;
        weight += factor * 10;
        ClampWeight(ref weight);
        skinnedMeshRenderer.SetBlendShapeWeight(blendShapeIndex, weight);

        if (weight == 0f || weight == 100f)
        {
            closeEyes = false;
            openEyes = false;
        }
    }

    void ClampWeight(ref float weight)
    {
        weight = weight > 100f ? 100f : weight;
        weight = weight < 0f ? 0f : weight;
    }
}
