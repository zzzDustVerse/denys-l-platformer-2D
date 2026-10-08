using UnityEngine;

public class DynamicCamera : MonoBehaviour
{

    
    
    [Header("Target")]
    [SerializeField] private Transform player;
    

    [Header("Follow")]
    [SerializeField] private float smoothTime = 0.15f;
    [SerializeField] private float maxFollowSpeed = 30f;

    [Header("Look Ahead")]
    [SerializeField] private float lookAheadDistance = 2.5f;
    [SerializeField] private float lookAheadSmoothTime = 0.2f;

    [Header("Vertical Movement")]
    [SerializeField] private float verticalOffset = 1f;
    [SerializeField] private float verticalSmoothTime = 0.25f;

    [Header("Dash")]
    [SerializeField] private float dashLookAhead = 3f;
    [SerializeField] private float dashSmoothTime = 0.08f;

    private Rigidbody2D playerRb;

    private Vector3 followVelocity;
    private float currentLookAhead;
    private float lookAheadVelocity;

    private float currentVerticalOffset;
    private float verticalVelocity;

    private void Awake()
    {
        if (player != null)
        {
            playerRb = player.GetComponent<Rigidbody2D>();
        }
    }

    private void LateUpdate()
    {
        if (player == null)
            return;

        if (playerRb == null)
        {
            playerRb = player.GetComponent<Rigidbody2D>();
        }

        UpdateCamera();
    }

    private void UpdateCamera()
    {
        // -------------------------------------------------
        // LOOK AHEAD
        // -------------------------------------------------

        float targetLookAhead = 0f;

        if (Mathf.Abs(playerRb.velocity.x) > 0.1f)
        {
            targetLookAhead =
                Mathf.Sign(playerRb.velocity.x) *
                lookAheadDistance;
        }

        currentLookAhead = Mathf.SmoothDamp(
            currentLookAhead,
            targetLookAhead,
            ref lookAheadVelocity,
            lookAheadSmoothTime
        );

        // -------------------------------------------------
        // VERTICAL OFFSET
        // -------------------------------------------------

        float targetVerticalOffset = verticalOffset;

        // Look slightly up when jumping
        if (playerRb.velocity.y > 2f)
        {
            targetVerticalOffset = verticalOffset + 0.5f;
        }

        // Look down when falling
        if (playerRb.velocity.y < -2f)
        {
            targetVerticalOffset = verticalOffset - 0.5f;
        }

        currentVerticalOffset = Mathf.SmoothDamp(
            currentVerticalOffset,
            targetVerticalOffset,
            ref verticalVelocity,
            verticalSmoothTime
        );

        // -------------------------------------------------
        // TARGET POSITION
        // -------------------------------------------------

        Vector3 targetPosition = new Vector3(
            player.position.x + currentLookAhead,
            player.position.y + currentVerticalOffset,
            transform.position.z
        );

        // -------------------------------------------------
        // SMOOTH FOLLOW
        // -------------------------------------------------

        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref followVelocity,
            smoothTime,
            maxFollowSpeed
        );
    }

    
}