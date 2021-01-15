using UnityEngine;

// The actually perspective of a mirror isn't actually as simple as this, needs improvement
public class Mirror : MonoBehaviour
{
    private Camera cam;

    private void Awake()
    {
        cam = GetComponentInChildren<Camera>();
        cam.targetTexture = new RenderTexture(256, 256, 16);

        GetComponent<Renderer>().material.mainTexture = cam.targetTexture;
    }
}
