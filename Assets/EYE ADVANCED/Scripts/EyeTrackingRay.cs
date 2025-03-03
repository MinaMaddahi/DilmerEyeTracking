using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class EyeTrackingRay : MonoBehaviour
{
    public static EyeTrackingRay Instance;

    [SerializeField]
    private float rayDinstance = 1.0f;

    [SerializeField]
    private float rayWidth = 0.01f;

    [SerializeField]
    private LayerMask layersToInclude;

    [SerializeField]
    private Color rayColorDefaultState = Color.yellow;

    [SerializeField]
    private Color rayColorHoverState = Color.red;

    private LineRenderer lineRenderer;

    // Store interactables
    private List<EyeInteractable> eyeInteractables = new List<EyeInteractable>();
    private List<PatternBallGazeHandler> patternBalls = new List<PatternBallGazeHandler>();

    void Awake()
    {
        Instance = this; // Assign instance to singleton
    }

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        SetupRay();
    }

    void SetupRay()
    {
        lineRenderer.useWorldSpace = false;
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = rayWidth;
        lineRenderer.endWidth = rayWidth;
        lineRenderer.startColor = rayColorDefaultState;
        lineRenderer.endColor = rayColorDefaultState;
        lineRenderer.SetPosition(0, Vector3.zero);
        lineRenderer.SetPosition(1, Vector3.forward * rayDinstance);
    }

    void FixedUpdate()
    {
        RaycastHit hit;
        Vector3 rayCastDirection = transform.TransformDirection(Vector3.forward);

        if (Physics.Raycast(transform.position, rayCastDirection, out hit, rayDinstance, layersToInclude))
        {
            // Detect normal eye-interactable objects
            var eyeInteractable = hit.transform.GetComponent<EyeInteractable>();
            if (eyeInteractable != null && !eyeInteractables.Contains(eyeInteractable))
            {
                eyeInteractables.Add(eyeInteractable);
                eyeInteractable.OnStartLooking();
            }

            // Detect pattern balls separately
            var patternBall = hit.transform.GetComponent<PatternBallGazeHandler>();
            if (patternBall != null && !patternBalls.Contains(patternBall))
            {
                patternBalls.Add(patternBall);
                patternBall.OnStartLooking(); // Trigger pattern interaction
            }

            lineRenderer.startColor = rayColorHoverState;
            lineRenderer.endColor = rayColorHoverState;
        }
        else
        {
            lineRenderer.startColor = rayColorDefaultState;
            lineRenderer.endColor = rayColorDefaultState;
            UnSelect(true);
        }
    }

    void UnSelect(bool clear = false)
    {
        foreach (var interactable in eyeInteractables)
        {
            if (interactable != null)
            {
                interactable.OnStopLooking();
            }
        }

        foreach (var patternBall in patternBalls)
        {
            if (patternBall != null)
            {
                patternBall.OnStopLooking();
            }
        }

        if (clear)
        {
            eyeInteractables.Clear();
            patternBalls.Clear();
        }
    }
}
