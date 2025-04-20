using System.ComponentModel;
using UnityEngine;
using UnityEngine.XR.ARFoundation;


[RequireComponent(typeof(ARAnchorManager))]
public class AnchorHandler : MonoBehaviour
{
    [SerializeField] GameObject gameFieldPrefap;
    private ARAnchorManager _anchorManager;
    private bool _hasAnchor = false;
    Transform ancherTransform;

    private void Awake()
    {
        _anchorManager = GetComponent<ARAnchorManager>();
        // Subscribe to anchor events
        _anchorManager.anchorsChanged += OnAnchorsChanged;
    }

    private void OnAnchorsChanged(ARAnchorsChangedEventArgs args)
    {
        // If we detect a new anchor being added and we already have one
        if (_hasAnchor && args.added.Count > 0) 
        {
            // Remove all newly added anchors
            foreach (var anchor in args.added)
            {
                Destroy(anchor.gameObject);
            }
        }
        else if (args.added.Count > 0)
        {
            // This is our first anchor
            _hasAnchor = true;
            foreach (var anchor in args.added)
            {
                VisualLogger.Log("anchor add");
                ancherTransform = anchor.transform;
                CreateZombieField();
            }
        }
    }

    [ContextMenu("create zombiField")]
    void CreateZombieField()
    {
#if UNITY_EDITOR
        ancherTransform = new GameObject("Game Field Test").transform;
        ancherTransform.position = new Vector3(0, -0.5f, 2);
#endif
        if (gameFieldPrefap != null&&ancherTransform!=null)
        {
            GameObject zombiField = GameObject.Instantiate(gameFieldPrefap, ancherTransform);
            _hasAnchor = true;
        }
    }

    private void OnDestroy()
    {
        if (_anchorManager != null)
            _anchorManager.anchorsChanged -= OnAnchorsChanged;
    }
}