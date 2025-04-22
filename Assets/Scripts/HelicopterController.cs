using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(ClickHandler))]
public class HelicopterController : MonoBehaviour,IPooledObject,UIRayCaster.IInteractable
{
    [Header("Movement Settings")]
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private float maxSpeed = 8f;
    [SerializeField] private float acceleration = 2f;
    [SerializeField] private float decelerationDistance = 5f;
    [SerializeField] private float rotationSpeed = 3f;
    [SerializeField] private float tiltAmount = 25f;
    [SerializeField] private float hoverHeight = 0f;
    [SerializeField] private float movementDelay = 3f;

    [Header("Hover Wobble Settings")]
    [SerializeField] private float wobbleIntensity = 0.2f;
    [SerializeField] private float wobbleFrequency = 1f;

    [Header("other")]
    [SerializeField] private bool isMoving = false;
    [SerializeField] private Animator animator;
    [SerializeField] private ParticleSystem explosionParticleSystem;
    [SerializeField] private float removeObjectAfterDeathDelay = 4;
    [SerializeField] private Vector3 targetPosition;

    private int currentWaypointIndex = 0;
    private float currentSpeed = 0f;
    private float noiseOffset;
    private new Rigidbody rigidbody;

    void Intialize()
    {
        Debug.Log("start new Helicopter");
        if (waypoints.Length == 0) Debug.LogError("No way-points assigned!");
        noiseOffset = Random.Range(0f, 100f);
        Invoke("PickNewWaypoint", 1f);
        this.rigidbody = GetComponent<Rigidbody>();
    }

    public void OnObjectSpawn()
    {
        Debug.Log("new helicopter spawned");
        Intialize();
    }

    void Update()
    {
        if (!isMoving || waypoints.Length == 0) return;

        // Speed control with acceleration/deceleration
        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);
        if (distanceToTarget > decelerationDistance)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, maxSpeed, acceleration * Time.deltaTime);
        }
        else
        {
            currentSpeed = Mathf.Lerp(currentSpeed, maxSpeed * 0.3f, acceleration * 2 * Time.deltaTime);
        }

        // Apply movement
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, currentSpeed * Time.deltaTime);

        // Rotation and banking
        Vector3 direction = (targetPosition - transform.position).normalized;
        HandleRotation(direction);

        // Hover wobble effect
        ApplyWobble();

        // Check if reached target
        if (distanceToTarget < 0.02f)
        {
            currentSpeed = 0f;
            isMoving = false;
            Invoke("PickNewWaypoint", movementDelay);
        }
    }

    void OnPlaneClciked()
    {
        ToggleMovingState(false);
        CancelInvoke(); // to stop Invoke("PickNewWaypoint", movementDelay);
        rigidbody.useGravity = true;
        animator.SetTrigger("die");
        explosionParticleSystem.gameObject.SetActive(true);
        explosionParticleSystem.Play();
        Invoke("AfterDeath", removeObjectAfterDeathDelay);
    }

    //gets called from OnPlaneClciked with invoke()
    void AfterDeath()
    {
        gameObject.SetActive(false);
    }

    void HandleRotation(Vector3 direction)
    {
        if (direction != Vector3.zero)
        {
            // Face exactly toward target (Y rotation only)
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );

            // Banking effect (Z tilt based on turn direction)
            float bankAngle = -direction.x * tiltAmount;
            // Additional pitch when accelerating/decelerating
            float pitchAngle = Mathf.Clamp(-direction.y * tiltAmount * 0.5f, -10f, 10f);

            Quaternion tiltRotation = Quaternion.Euler(
                pitchAngle,
                transform.eulerAngles.y,
                bankAngle
            );

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                tiltRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }

    void ApplyWobble()
    {
        if (currentSpeed < maxSpeed * 0.3f) // Only wobble when moving slowly/hovering
        {
            float time = Time.time * wobbleFrequency + noiseOffset;
            float wobbleX = Mathf.PerlinNoise(time, noiseOffset) * 2 - 1;
            float wobbleZ = Mathf.PerlinNoise(noiseOffset, time) * 2 - 1;

            Vector3 wobble = new Vector3(
                wobbleX * wobbleIntensity,
                0,
                wobbleZ * wobbleIntensity
            );

            transform.position += transform.TransformDirection(wobble) * Time.deltaTime;
        }
    }

    void PickNewWaypoint()
    {
        if (waypoints.Length == 0) return;

        int newIndex;
        do
        {
            newIndex = Random.Range(0, waypoints.Length);
        } while (newIndex == currentWaypointIndex);

        currentWaypointIndex = newIndex;
        targetPosition = waypoints[currentWaypointIndex].position + Vector3.up * hoverHeight;
        ToggleMovingState(true);
    }

    void ToggleMovingState(bool _isMoving)
    {
        isMoving = _isMoving;
    }

    void OnDrawGizmos()
    {
        if (waypoints == null) return;

        Gizmos.color = Color.green;
        foreach (Transform waypoint in waypoints)
        {
            Gizmos.DrawSphere(waypoint.position, 0.08f);
        }

        if (isMoving)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, targetPosition);
            //Gizmos.DrawWireSphere(targetPosition, decelerationDistance);
        }
    }

    public void OnInteract()
    {
        Debug.Log("got hit by the cross-hair ");
        OnPlaneClciked();
    }
}