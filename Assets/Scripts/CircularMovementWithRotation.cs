using UnityEngine;

public class CircularMovementWithRotation : MonoBehaviour,UIRayCaster.IInteractable
{
    [Header("Circle Settings")]
    public float radius = 5f;
    public float speed = 1f;
    public bool clockwise = true;
    [SerializeField] Transform centerPointTransform;

    [Header("Animation")]
    public Animator animator;
    public string walkAnimationParam = "Walk";
    public string dieingAnimationParam = "die";
    public float animationSpeedMultiplier = 1f;

    private float angle = 0f;
    private Vector3 centerPosition;
    private CharacterController characterController;
    private bool isZombieAlive = true;
    private float currentSpeed = 0f;

    void Start()
    {
        centerPosition = centerPointTransform.position;
        if (animator == null) animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        Invoke("TriggerWalkingAnimation",2f);

        // Set initial position on the circle
        angle = GetCurrentAngle();
        UpdatePositionOnCircle();
    }

    void Update()
    {
        if (!isZombieAlive) return;
        centerPosition = centerPointTransform.position;

        // Calculate desired speed based on circular path
        currentSpeed = speed * radius; // Circumference speed
        animator.speed = currentSpeed * animationSpeedMultiplier;
    }

    void OnAnimatorMove()
    {
        if (!isZombieAlive) return;

        // Get root motion movement
        Vector3 rootMotion = animator.deltaPosition;

        // Calculate movement direction in circle space
        angle = GetCurrentAngle();
        Vector3 tangent = GetTangentDirection(angle);

        // Project root motion onto tangent direction
        float forwardMotion = Vector3.Dot(rootMotion, transform.forward*-1);
        float angularMovement = (forwardMotion / (2f * Mathf.PI * radius)) * Mathf.PI * 2f;
        angle += clockwise ? -angularMovement : angularMovement;

        // Update position and rotation
        UpdatePositionOnCircle();
        UpdateRotation(tangent);

        // Optional: Adjust animator speed based on remaining distance
        float distanceToTarget = forwardMotion;
        animator.speed = Mathf.Clamp(distanceToTarget * 0.5f, 0.8f, 1.2f);
    }

    void OnDrawGizmosSelected()
    {
        if (centerPointTransform == null) return;

        // Draw the movement circle
        Gizmos.color = Color.cyan;
        Vector3 center = Application.isPlaying ? centerPosition : centerPointTransform.position;

        const int segments = 36;
        float anglePerSegment = 2 * Mathf.PI / segments;

        for (int i = 0; i < segments; i++)
        {
            Vector3 start = center + new Vector3(
                Mathf.Sin(anglePerSegment * i) * radius,
                0,
                Mathf.Cos(anglePerSegment * i) * radius
            );

            Vector3 end = center + new Vector3(
                Mathf.Sin(anglePerSegment * (i + 1)) * radius,
                0,
                Mathf.Cos(anglePerSegment * (i + 1)) * radius
            );

            Gizmos.DrawLine(start, end);
        }

        // Draw current path tangent
        if (Application.isPlaying && isZombieAlive)
        {
            Gizmos.color = Color.yellow;
            Vector3 tangent = GetTangentDirection(angle);
            Gizmos.DrawRay(transform.position, tangent * 2f);
        }
    }

    void OnZombieClicked()
    {
        isZombieAlive = false;
        animator.SetTrigger(dieingAnimationParam);
    }

    void TriggerWalkingAnimation()
    {
        animator.SetTrigger(walkAnimationParam);
        animator.speed = currentSpeed * animationSpeedMultiplier;
    }

    float GetCurrentAngle()
    {
        Vector3 toObject = transform.position - centerPosition;
        return Mathf.Atan2(toObject.x, toObject.z);
    }

    Vector3 GetTangentDirection(float currentAngle)
    {
        return clockwise ?
            new Vector3(Mathf.Cos(currentAngle), 0, -Mathf.Sin(currentAngle)) :
            new Vector3(-Mathf.Cos(currentAngle), 0, Mathf.Sin(currentAngle));
    }

    void UpdatePositionOnCircle()
    {
        Vector3 newPosition = centerPosition + new Vector3(
            Mathf.Sin(angle) * radius,
            0,
            Mathf.Cos(angle) * radius
        );

        if (characterController != null && characterController.enabled)
        {
            characterController.Move(newPosition - transform.position);
        }
        else
        {
            transform.position = newPosition;
        }
    }

    void UpdateRotation(Vector3 tangent)
    {
        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            Quaternion.LookRotation(tangent),
            10f * Time.deltaTime
        );
    }

    void OnDisable()
    {
        if (animator != null)
        {
            animator.SetTrigger("Wait");
        }
    }

    public void OnInteract()
    {
        OnZombieClicked();
    }
}