using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    //  Store gaze order (objects the user has looked at)
    private List<string> gazeOrder = new List<string>();

    private List<EyeInteractable> eyeInteractables = new List<EyeInteractable>();

    // Define the correct sequence of objects to look at
    private readonly List<string> correctOrder = new List<string> { "EyeInteractable", "EyeInteractable1", "EyeInteractable2" };


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
            //UnSelect();  // Reset previous interactables

            var eyeInteractable = hit.transform.GetComponent<EyeInteractable>();
            if (eyeInteractable != null && !eyeInteractables.Contains(eyeInteractable))
            {
                eyeInteractables.Add(eyeInteractable);
                //eyeInteractable.IsHovered = true;
                eyeInteractable.OnStartLooking();

                //  Store the order of objects looked at
                StoreGazeOrder(hit.transform.name);

                lineRenderer.startColor = rayColorHoverState;
                lineRenderer.endColor = rayColorHoverState;
            }
        }
        else
        {
            lineRenderer.startColor = rayColorDefaultState;
            lineRenderer.endColor = rayColorDefaultState;
            UnSelect(true);  // Clear the list
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

        if (clear)
        {
            eyeInteractables.Clear();
        }
    }
    // 🔹 Store the order of objects the user looks at
    public void StoreGazeOrder(string objectName)
    {
        Debug.Log($"👀 Looked at: {objectName} | Current Gaze Order: {string.Join(" -> ", gazeOrder)}");

        // Ignore if object is not in the correct sequence list
        if (!correctOrder.Contains(objectName))
        {
            Debug.LogWarning($"⚠ {objectName} is not in the correct sequence list.");
            return;
        }

        // Ignore if object is already looked at
        if (gazeOrder.Contains(objectName))
        {
            Debug.Log($"🔄 {objectName} was already looked at. Ignoring.");
            return;
        }

        // Add object to gaze order
        gazeOrder.Add(objectName);
        Debug.Log($"✅ Added: {objectName} | New Gaze Order: {string.Join(" -> ", gazeOrder)}");
    }
}
