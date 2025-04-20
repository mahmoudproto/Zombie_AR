using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARAnchorManager))]
[RequireComponent(typeof(ARRaycastManager))]  // Added this requirement
public class ARAnchorCreator : MonoBehaviour
{
    [SerializeField] private GameObject objectToPlace;
    private ARAnchorManager anchorManager;
    private ARRaycastManager raycastManager;  // Added this reference
    private List<ARAnchor> anchors = new List<ARAnchor>();

    void Start()
    {
        anchorManager = GetComponent<ARAnchorManager>();
        raycastManager = GetComponent<ARRaycastManager>();  // Initialize raycast manager

        if (raycastManager == null)
        {
            Debug.LogError("ARRaycastManager component missing! Add it to AR Session Origin.");
        }
        
    }
    //protected override void anchorManager.OnTrackablesChanged(
    //       List<ARAnchor> added,
    //       List<ARAnchor> updated,
    //       List<ARAnchor> removed)
    //{ }
        public void PlaceObjectAtHitTest(Vector2 screenPosition)
    {
        // Perform a hit test using ARRaycastManager instead
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        if (raycastManager.Raycast(screenPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = hits[0].pose;

            // Create anchor at hit position
            ARAnchor anchor = anchorManager.AddAnchor(hitPose);

            if (anchor != null)
            {
                // Instantiate your object as child of anchor
                Instantiate(objectToPlace, anchor.transform);
                anchors.Add(anchor);
            }
            else
            {
                Debug.LogWarning("Failed to create ARAnchor");
            }
        }
    }
}