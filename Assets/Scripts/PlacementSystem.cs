using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlacementSystem : MonoBehaviour
{
    public static event Action<Placeable> SelectedPlaceableChanged;

    // Selected placeables
    private Placeable selectedPlaceable;

    // Offset from placeable position to the world point user clicked
    private Vector3 selectedPlaceableClickOffset;

    // Currently placed placeables
    private List<Placeable> currentPlaceables = new List<Placeable>();

    private void OnEnable()
    {
        PlaceableToolbar.OnNewPlaceableClick += NewPlaceableClick;
    }

    private void OnDisable()
    {
        PlaceableToolbar.OnNewPlaceableClick -= NewPlaceableClick;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            if (selectedPlaceable != null)
            {
                currentPlaceables.Remove(selectedPlaceable);
                Destroy(selectedPlaceable.gameObject);
                selectedPlaceable = null;
                SelectedPlaceableChanged?.Invoke(null);
            }
        }
    }

    public void WorldInteractionPointerDown(BaseEventData data)
    {
        PointerEventData pointerData = data as PointerEventData;

        // Check if we pressed on anything
        Placeable hitPlaceable = RaycastCurrentPlaceable(pointerData.position);
        if (hitPlaceable != selectedPlaceable)
        {
            selectedPlaceable = hitPlaceable;
            SelectedPlaceableChanged(hitPlaceable);
        }

        if (selectedPlaceable != null)
        {
            selectedPlaceableClickOffset = selectedPlaceable.transform.position - ScreenToWorld(pointerData.position, selectedPlaceable);
        }
    }

    public void WorldInteractionPointerDrag(BaseEventData data)
    {
        // If we have nothing selected, we have nothing to drag
        if (selectedPlaceable == null) return;

        PointerEventData pointerData = data as PointerEventData;
        Vector3 pointerWorldPoint = ScreenToWorld(pointerData.position, selectedPlaceable);


        // Check if we need to switch wall for prop
        if (selectedPlaceable.placementType == Placeable.PlacementType.Wall)
        {
            // Check if change is needed
            if ((pointerWorldPoint.x < 4.99f) == selectedPlaceable.isRightWall)
            {
                selectedPlaceable.isRightWall = !selectedPlaceable.isRightWall;

                selectedPlaceable.transform.localRotation = Quaternion.Euler(
                    selectedPlaceable.transform.localRotation.eulerAngles.x,
                    selectedPlaceable.isRightWall ? 0 : -90f,
                    selectedPlaceable.transform.localRotation.eulerAngles.z);

                // Need to flip X/Z on the offset as well
                selectedPlaceableClickOffset = new Vector3(selectedPlaceableClickOffset.z, selectedPlaceableClickOffset.y, selectedPlaceableClickOffset.x);
            }
        }

        // Lets move stuff with left button, rotate stuff with right button
        if (pointerData.button == PointerEventData.InputButton.Left)
        {
            selectedPlaceable.transform.position = pointerWorldPoint + selectedPlaceableClickOffset;
        }
        else if (pointerData.button == PointerEventData.InputButton.Right)
        {
            if (selectedPlaceable.placementType == Placeable.PlacementType.Ground)
            {
                selectedPlaceable.transform.localRotation = Quaternion.Euler(
                    selectedPlaceable.transform.localRotation.eulerAngles.x,
                    selectedPlaceable.transform.localRotation.eulerAngles.y + (pointerData.delta.x + pointerData.delta.y) * 0.5f,
                    selectedPlaceable.transform.localRotation.eulerAngles.z
                );
            }
        }
    }

    private void NewPlaceableClick(Placeable placeable)
    {
        Placeable newPlaceable = Instantiate(placeable);
        newPlaceable.name = placeable.name;
        currentPlaceables.Add(newPlaceable);
        newPlaceable.transform.position = placeable.GetDefaultPosition();

        selectedPlaceable = newPlaceable;
        SelectedPlaceableChanged?.Invoke(selectedPlaceable);
    }

    private Vector3 ScreenToWorld(Vector2 screenPos, Placeable accordingToPlaceable)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPos);

        // If this is a ground object, we check ground, otherwise both of side walls
        if (accordingToPlaceable.placementType == Placeable.PlacementType.Ground)
        {
            new Plane(Vector3.up, Vector3.zero).Raycast(ray, out float dist);
            Vector3 p = ray.GetPoint(dist);
            p.y = 0;
            return p;
        }
        else
        {
            // Check both side walls, return closest point
            new Plane(Vector3.back, Vector3.one * 5f).Raycast(ray, out float distZ);
            new Plane(Vector3.left, Vector3.one * 5f).Raycast(ray, out float distX);

            if (distX < distZ)
            {
                Vector3 p = ray.GetPoint(distX);
                p.x = 5f;
                return p;
            }
            else
            {
                Vector3 p = ray.GetPoint(distZ);
                p.z = 5f;
                return p;
            }
        }
    }

    private Placeable RaycastCurrentPlaceable(Vector2 screenPos)
    {
        RaycastHit[] hits = Physics.RaycastAll(Camera.main.ScreenPointToRay(screenPos));

        foreach (RaycastHit hit in hits)
        {
            Placeable p = hit.collider.GetComponentInParent<Placeable>();
            if (p != null)
            {
                return p;
            }
        }

        return null;
    }
}
