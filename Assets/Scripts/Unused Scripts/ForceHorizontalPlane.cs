using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ForceHorizontalPlane : MonoBehaviour
{
    private ARPlaneManager planeManager;

    void Start()
    {
        planeManager = GetComponent<ARPlaneManager>();
        planeManager.planesChanged += OnPlanesChanged;
    }

    private void OnPlanesChanged(ARPlanesChangedEventArgs args)
    {
        VisualLogger.Log("rotate planes");
        foreach (var plane in args.added)
        {
            //Rotate the entire plane GameObject
            plane.transform.rotation = Quaternion.Euler(270, 0, 0);
        }
    }

    void OnDestroy()
    {
        if (planeManager != null)
            planeManager.planesChanged -= OnPlanesChanged;
    }
}