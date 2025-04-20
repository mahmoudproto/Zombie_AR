using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class PlaneDetectionHandler : MonoBehaviour
{
    [SerializeField] ARPlaneManager planeManager;
    [SerializeField] GameObject monsterPrefap;
    [SerializeField] GameObject planePrefap;
    bool planeCreated=false;
    ARPlane mainPlane;
    GameObject monsterObject ;
    GameObject planeObject;
    GameObject Container;

    Transform ARSessionOriginTransform;

    void Start()
    {
        if(planeManager == null)
            planeManager = FindObjectOfType<ARPlaneManager>();

        planeManager.planesChanged += OnPlanesChanged;
        Container = new GameObject("container");
    }

    //for testing you can safely remove it
    [ContextMenu("call OnPlanesChanged manually")]
    public void test()
    {
        CreatePlane(null);
    }

    private void OnPlanesChanged(ARPlanesChangedEventArgs args)
    {

        foreach (var plane in args.added)
        {
            if (planeCreated)
                return;
            CreatePlane(plane);
        }
        //foreach (var plane in args.updated)
        //{
        //    if (plane.trackableId.ToString() == mainPlane.trackableId.ToString())
        //    {
        //        planeObject.transform.position = plane.transform.position;
        //    }
        //}
    }

    void CreatePlane(ARPlane plane)
    {
        Transform planeTransform;
        if (plane == null)
        {
            plane = new ARPlane();
            planeTransform = this.transform;
        }
        else
            planeTransform = plane.transform;

        mainPlane = plane;
        VisualLogger.Log("Plane added: " + plane.trackableId);
        if (monsterPrefap != null)
        {
            Container.transform.position = planeTransform.position;
            planeObject = GameObject.Instantiate(planePrefap, planeTransform);
            monsterObject = GameObject.Instantiate(monsterPrefap, planeObject.transform);
            monsterObject.transform.localPosition = Vector3.zero;
            //planeObject.transform.SetParent(ARSessionOriginTransform);
        }
        planeCreated = true;
    }

    void OnDestroy()
    {
        if (planeManager != null)
            planeManager.planesChanged -= OnPlanesChanged;
    }
}