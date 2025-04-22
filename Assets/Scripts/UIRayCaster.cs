// Ignore Spelling: Interactable

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIRayCaster : MonoBehaviour
{
    [Header("UI References")]
    public Image uiImage;
    public Camera rayCastCamera;

    [Header("RayCast Settings")]
    public float maxDistance = 100f;
    public LayerMask rayCastLayers = ~0;

    [Header("Visual Feedback")]
    public bool showDebugRay = true;
    public Color debugRayColor = Color.green;
    public float debugRayDuration = 1f;

    private RectTransform uiRectTransform;
    private Canvas parentCanvas;

    void Start()
    {
        InitializeComponents();
    }

    private void InitializeComponents()
    {
        if (uiImage == null) uiImage = GetComponent<Image>();
        if (rayCastCamera == null) rayCastCamera = Camera.main;

        uiRectTransform = uiImage.GetComponent<RectTransform>();
        parentCanvas = GetComponentInParent<Canvas>();
    }

    //gets called from the fire button
    public void FireRayCast()
    {
        RayCastFromUIImage();
    }

    private RaycastHit RayCastFromUIImage()
    {
        Ray ray = GetRayFromUIImage();
        RaycastHit hitInfo;
        bool hit = Physics.Raycast(ray, out hitInfo, maxDistance, rayCastLayers.value);

        if (showDebugRay)
        {
            Debug.DrawRay(ray.origin, ray.direction * (hit ? hitInfo.distance : maxDistance),
                         debugRayColor, debugRayDuration);
        }

        if (hit)
        {
            HandleRaycastHit(hitInfo);
        }

        return hitInfo;
    }

    private void HandleRaycastHit(RaycastHit hitInfo)
    {
        Debug.Log($"Hit object: {hitInfo.collider.gameObject.name} at {hitInfo.point}");

        IInteractable interactable = hitInfo.collider.GetComponentInParent<IInteractable>();
        if (interactable != null)
        {
            interactable.OnInteract();
        }
        else
            Debug.Log("no interactable object hit , is IInteractable set on the right object hierarchy");
    }

    private Ray GetRayFromUIImage()
    {
        Vector2 screenPoint;

        if (parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            // For Screen Space - Overlay, we can use the UI element's screen position directly
            screenPoint = uiRectTransform.position;
        }
        else
        {
            // For other modes (Screen Space - Camera or World Space)
            screenPoint = RectTransformUtility.WorldToScreenPoint(
                parentCanvas.worldCamera,
                uiRectTransform.position
            );
        }

        return rayCastCamera.ScreenPointToRay(screenPoint);
    }

    public interface IInteractable
    {
        void OnInteract();
    }
}