using System.Collections.Generic;
using UnityEngine;

public class Placeable : MonoBehaviour
{
    public enum PlacementType { Ground = 0, Wall = 1 }

    // Type of placement this object is used with
    public PlacementType placementType = PlacementType.Ground;

    // The height of the bounding box around this placeable
    public float height = 1f;

    // List of things that can have their color modified
    public List<ModifyableColor> modifyableColors = new List<ModifyableColor>();

    // Is the object placed on right wall
    [HideInInspector] public bool isRightWall = true;

    private void Awake()
    {
        // We want to create new shared colors for all the modifyable renderers,
        // so they can be modified individually
        foreach (ModifyableColor col in modifyableColors)
        {
            // NOTE: this creates more materials than needed,
            // for production usage, this should be made in a way where for ex. all chair legs use the same material
            foreach (Renderer ren in col.colorRenderers)
            {
                ren.sharedMaterial = new Material(ren.sharedMaterial);
                col.current = ren.sharedMaterial.color;
            }
        }
    }
    public Vector3 GetDefaultPosition()
    {
        if (placementType == PlacementType.Wall)
        {
            return new Vector3(5, 5, 0);
        }
        else
        {
            return Vector3.zero;
        }
    }
}
