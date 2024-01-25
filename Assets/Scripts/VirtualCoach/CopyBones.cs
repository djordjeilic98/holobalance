using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CopyBones : MonoBehaviour
{
    public GameObject sourceObject;

    void Start()
    {
        Copy();
    }

    void Copy()
    {
        if (sourceObject == null) return;

        var sourceRenderer = sourceObject.GetComponent<SkinnedMeshRenderer>();
        var targetRenderer = this.GetComponent<SkinnedMeshRenderer>();

        if (sourceRenderer == null || targetRenderer == null) return;

        targetRenderer.bones = sourceRenderer.bones.Where(b => targetRenderer.bones.Any(t => t.name == b.name)).ToArray();
    }
}
