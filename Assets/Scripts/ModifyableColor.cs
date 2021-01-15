using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ModifyableColor
{
    // Title of the property
    public string title;

    // Renderers, that display this color
    public List<Renderer> colorRenderers = new List<Renderer>();

    // Currently set color
    public Color current;
}
