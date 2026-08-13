using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;

public class OffWayPoint : MonoBehaviour
{
    private void Awake()
    {
        Off();
    }
    private void Off()
    {
        MeshRenderer[] meshRenderers = gameObject.GetComponentsInChildren<MeshRenderer>();

        foreach (MeshRenderer meshRenderer in meshRenderers)
        {
            meshRenderer.enabled = false;
        }
    }
}
