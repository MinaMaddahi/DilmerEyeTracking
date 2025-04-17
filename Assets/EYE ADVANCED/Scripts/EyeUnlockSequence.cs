using System.Collections;
using UnityEngine;

public class EyeUnlockSequence : MonoBehaviour
{
    [SerializeField] private GameObject[] segmentSequence;
    [SerializeField] private float[] timeWindows;

    [Header("Visual Feedback")]
    [SerializeField] private Material completedMaterial;

    private int currentStep = -1;
    private Coroutine sequenceCoroutine;
    private EyeInteractable eyeInteractable;
    private MeshRenderer meshRenderer;
    private bool isSequenceCompleted = false;

    private void Start()
    {
        eyeInteractable = GetComponent<EyeInteractable>();
        meshRenderer = GetComponent<MeshRenderer>();
        DeactivateAllSegments();
    }

    private void Update()
    {
        // Only start sequence if not already completed
        if (!isSequenceCompleted && eyeInteractable != null && eyeInteractable.IsHovered && currentStep == -1)
        {
            Debug.Log("Starting sequence from EyeInteractableUnlock...");
            currentStep = 0;
            StartNextSegment();
        }
    }

    private void StartNextSegment()
    {
        if (isSequenceCompleted)
        {
            Debug.Log("Sequence already completed — no further steps.");
            return;
        }

        if (currentStep >= segmentSequence.Length)
        {
            Debug.Log("Sequence completed!");
            OnSequenceCompleted();
            return;
        }

        GameObject currentSegment = segmentSequence[currentStep];
        float timeout = (timeWindows.Length > currentStep) ? timeWindows[currentStep] : 3f;

        Debug.Log($"Activating {currentSegment.name} with {timeout}s timeout.");
        currentSegment.SetActive(true);

        if (sequenceCoroutine != null)
            StopCoroutine(sequenceCoroutine);

        sequenceCoroutine = StartCoroutine(HandleSegmentWithTimeout(currentSegment, timeout));
    }

    private IEnumerator HandleSegmentWithTimeout(GameObject segment, float timeLimit)
    {
        // Immediately exit if sequence is already completed
        if (isSequenceCompleted) yield break;

        EyeInteractable segmentInteractable = segment.GetComponent<EyeInteractable>();
        if (segmentInteractable == null)
        {
            Debug.LogWarning($"{segment.name} does not have an EyeInteractable component.");
            yield break;
        }

        float timer = 0f;
        bool success = false;

        while (timer < timeLimit)
        {
            if (isSequenceCompleted) yield break; // Prevent continuing after completion

            if (segmentInteractable.IsHovered)
            {
                Debug.Log($"{segment.name} hovered in time.");
                success = true;
                break;
            }

            timer += Time.deltaTime;
            yield return null;
        }

        if (isSequenceCompleted) yield break;

        if (success)
        {
            currentStep++;
            StartNextSegment();
        }
        else
        {
            Debug.LogWarning($"Timeout on {segment.name}! Resetting sequence.");
            segment.SetActive(false);
            ResetSequence();
        }
    }

    private void OnSequenceCompleted()
    {
        Debug.Log("All segments completed! 🎉");
        isSequenceCompleted = true;

        // Stop any ongoing coroutine
        if (sequenceCoroutine != null)
        {
            StopCoroutine(sequenceCoroutine);
            sequenceCoroutine = null;
        }

        if (meshRenderer != null && completedMaterial != null)
        {
            meshRenderer.material = completedMaterial;
        }

        foreach (GameObject segment in segmentSequence)
        {
            if (segment != null)
                segment.SetActive(false);
        }
    }

    private void ResetSequence()
    {
        if (isSequenceCompleted) return; // Don't reset if sequence is completed

        DeactivateAllSegments();
        currentStep = -1;
    }

    private void DeactivateAllSegments()
    {
        foreach (GameObject go in segmentSequence)
        {
            if (go != null)
                go.SetActive(false);
        }
    }
}