using UnityEngine;
using UnityEngine.UI;

public class SelectedPlaceablePanel : MonoBehaviour
{
    [SerializeField] private Text label = null;
    [SerializeField] private CanvasGroup cGroup = null;
    [SerializeField] private ColorSelector selectorPrefab = null;
    [SerializeField] private RectTransform colorSelectorParent = null;

    private Placeable selectedPlaceable;

    private void Awake()
    {
        // Turn off panel at start
        SelectedPlaceableChanged(null);
    }

    private void OnEnable()
    {
        PlacementSystem.SelectedPlaceableChanged += SelectedPlaceableChanged;
    }

    private void OnDisable()
    {
        PlacementSystem.SelectedPlaceableChanged -= SelectedPlaceableChanged;
    }

    private void SelectedPlaceableChanged(Placeable placeable)
    {
        selectedPlaceable = placeable;

        // Enable/disable panel accordingly
        cGroup.alpha = placeable == null ? 0 : 1f;
        cGroup.blocksRaycasts = placeable != null;
        cGroup.interactable = placeable != null;

        // If we have something selected, populate panel
        if (selectedPlaceable != null)
        {
            label.text = "Selected: " + selectedPlaceable.name;

            // Destroy previous color selectors
            foreach (ColorSelector cs in colorSelectorParent.GetComponentsInChildren<ColorSelector>())
            {
                Destroy(cs.gameObject);
            }

            // Create new color selectors
            foreach (ModifyableColor modColor in placeable.modifyableColors)
            {
                ColorSelector selector = Instantiate(selectorPrefab, colorSelectorParent);
                selector.transform.localScale = Vector3.one;
                selector.SetTitle(modColor.title);
                selector.SetValueWithoutNotifty(modColor.current);

                selector.OnValueChanged += (c) => ColorSelectorValueChanged(modColor, c);
            }
        }
    }

    private void ColorSelectorValueChanged(ModifyableColor modColor, Color newColor)
    {
        modColor.current = newColor;
        foreach (Renderer ren in modColor.colorRenderers)
        {
            ren.sharedMaterial.color = newColor;
        }
    }
}
