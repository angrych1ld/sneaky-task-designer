using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ColorSelector : MonoBehaviour
{
    public event Action<Color> OnValueChanged;

    [SerializeField] private Text label = null;
    [SerializeField] private RectTransform colorButtonParent = null;
    [SerializeField] private List<Color> availableColors = new List<Color>();

    private List<Button> colorButtons = new List<Button>();

    private void Awake()
    {
        // For each color, set up a button
        foreach (Color color in availableColors)
        {
            Button button = new GameObject("Button", typeof(RectTransform), typeof(Button), typeof(Image)).GetComponent<Button>();
            button.transform.SetParent(colorButtonParent);
            button.transform.localScale = Vector3.one;

            button.GetComponent<Image>().color = color;
            button.targetGraphic = button.GetComponent<Image>();

            button.colors = ColorBlock.defaultColorBlock;

            colorButtons.Add(button);

            int i = colorButtons.Count - 1;

            button.onClick.AddListener(() => OnButtonClick(i));
        }
    }

    public void SetTitle(string title)
    {
        label.text = title;
    }

    public void SetValueWithoutNotifty(Color color)
    {
        int index = availableColors.IndexOf(color);
        if (index == -1) index = 0;

        foreach (Button button in colorButtons) button.interactable = true;
        colorButtons[index].interactable = false;
    }

    public void OnButtonClick(int i)
    {
        Color col = availableColors[i];
        SetValueWithoutNotifty(col);
        OnValueChanged?.Invoke(col);
    }
}
