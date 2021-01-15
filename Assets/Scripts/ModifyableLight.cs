using System.Linq;
using UnityEngine;

// This script is hacky, should set up a way to reach to modifyable color changes instead
public class ModifyableLight : MonoBehaviour
{
    // The light object of the light fixture
    private new Light light;

    // The light color modifier
    private ModifyableColor lightColorModifier;

    private void Awake()
    {
        light = GetComponentInChildren<Light>();
        lightColorModifier = GetComponent<Placeable>()?.modifyableColors.First(mc => mc.title.ToLower().Equals("light"));

        if (light == null || lightColorModifier == null)
        {
            Debug.LogError("Modifyable light failed initialization.");
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        light.color = lightColorModifier.current;
    }
}
