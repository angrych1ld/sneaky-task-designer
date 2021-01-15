using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlaceableToolbar : MonoBehaviour
{
    public static event Action<Placeable> OnNewPlaceableClick;

    // List of possible placeable prefabs
    [SerializeField] private List<Placeable> placeablePrefs = new List<Placeable>();

    // Camera, used for generating textures ar runtime
    [SerializeField] private Camera utilCam = null;

    // Prefab of the toolbar panel entry
    [SerializeField] private Transform toolbarEntryPref = null;

    // Parent for placeble entries
    [SerializeField] private RectTransform entryParent = null;

    private void Start()
    {
        foreach (Placeable placeable in placeablePrefs)
        {
            Transform toolbarEntry = Instantiate(toolbarEntryPref, entryParent);
            toolbarEntry.GetComponentInChildren<Image>().sprite = createPrefabSprite(placeable);
            toolbarEntry.GetComponentInChildren<Button>().onClick.AddListener(() => OnEntryClick(placeable));
        }
    }

    private void OnEntryClick(Placeable target)
    {
        OnNewPlaceableClick?.Invoke(target);
    }

    private Sprite createPrefabSprite(Placeable prefab)
    {
        Vector3 renderSpot = new Vector3(0, -5000f, 0);
        Vector2 textureSize = new Vector2(256, 256);

        // Set camera and texture settings
        RenderTexture renTex = new RenderTexture((int)textureSize.x, (int)textureSize.y, 16);
        utilCam.targetTexture = renTex;
        utilCam.transform.position = renderSpot - utilCam.transform.forward * 10f + Vector3.up * prefab.height * 0.5f;

        // Instantiate the target prefab
        Placeable prefInstance = Instantiate(prefab, renderSpot, Quaternion.identity);

        // Render the image and turn into texture
        utilCam.Render();
        RenderTexture.active = renTex;
        Texture2D result = new Texture2D(renTex.width, renTex.height);
        result.filterMode = FilterMode.Bilinear;
        result.ReadPixels(new Rect(0, 0, renTex.width, renTex.height), 0, 0);
        result.Apply();
        RenderTexture.active = null;

        // Clean up
        prefInstance.gameObject.SetActive(false);
        Destroy(renTex);
        Destroy(prefInstance.gameObject);

        return Sprite.Create(result, new Rect(Vector2.zero, textureSize), Vector2.one * 0.5f);
    }
}
