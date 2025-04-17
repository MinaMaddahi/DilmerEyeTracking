using UnityEngine;
using System.Collections;
using Modularify.LoadingBars3D;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class EyeInteractable : MonoBehaviour
{
    public bool IsHovered { get; private set; }

    [Header("Mode")]
    [Tooltip("If true, this object acts as an unlocker and skips progress bar logic.")]
    [SerializeField] private bool isUnlocker = false;

    [Header("Materials")]
    [SerializeField] private Material OnHoverActiveMaterial;
    [SerializeField] private Material OnHoverInactiveMaterial;

    [Header("Sequence Setup")]
    [SerializeField] private GameObject nextSegment;
    [SerializeField] private GameObject segmentUnlock;

    [Header("Progress Settings (for non-unlockers)")]
    [SerializeField] private LoadingBarSegments progressBar;
    [SerializeField] private int totalSegments = 5;
    [SerializeField] private float segmentFillTime = 1f;

    [Header("Unlocker Settings (if isUnlocker is true)")]
    [SerializeField] private float unlockDelay = 2f;

    private MeshRenderer meshRenderer;
    public GameObject selectionRing;

    private int currentFilledSegments = 0;
    private Coroutine progressCoroutine;
    private Coroutine resetCoroutine;
    private Coroutine unlockCoroutine;
    private bool isCompleted = false;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();

        if (segmentUnlock != null)
            segmentUnlock.SetActive(false);

        if (selectionRing != null)
            selectionRing.SetActive(false);

        if (progressBar != null)
            progressBar.SetPercentage(0);

        if (nextSegment != null)
            nextSegment.SetActive(false);
    }

    public void OnStartLooking()
    {
        if (isCompleted) return;

        Debug.Log("Started looking at: " + gameObject.name);
        IsHovered = true;

        if (meshRenderer != null && OnHoverActiveMaterial != null)
            meshRenderer.material = OnHoverActiveMaterial;

        if (selectionRing != null)
            selectionRing.SetActive(true);

        if (segmentUnlock != null)
            segmentUnlock.SetActive(true);

        if (resetCoroutine != null)
        {
            StopCoroutine(resetCoroutine);
            resetCoroutine = null;
        }

        if (isUnlocker)
        {
            if (unlockCoroutine == null)
                unlockCoroutine = StartCoroutine(UnlockAfterDelay());
        }
        else
        {
            if (progressBar != null && progressCoroutine == null)
                progressCoroutine = StartCoroutine(FillProgress());
        }
    }

    public void OnStopLooking()
    {
        if (isCompleted) return;

        Debug.Log("Stopped looking at: " + gameObject.name);
        IsHovered = false;

        if (meshRenderer != null && OnHoverInactiveMaterial != null)
            meshRenderer.material = OnHoverInactiveMaterial;

        if (selectionRing != null)
            selectionRing.SetActive(false);

        if (segmentUnlock != null && !isCompleted)
            segmentUnlock.SetActive(false);

        if (isUnlocker)
        {
            if (unlockCoroutine != null)
            {
                StopCoroutine(unlockCoroutine);
                unlockCoroutine = null;
            }
        }
        else
        {
            if (progressCoroutine != null)
            {
                StopCoroutine(progressCoroutine);
                progressCoroutine = null;
            }

            if (currentFilledSegments > 0)
            {
                resetCoroutine = StartCoroutine(ResetAfterDelay());
            }
        }
    }

    private IEnumerator FillProgress()
    {
        while (currentFilledSegments < totalSegments)
        {
            if (!IsHovered)
            {
                progressCoroutine = null;
                yield break;
            }

            currentFilledSegments++;
            float percentage = (float)currentFilledSegments / totalSegments;
            progressBar.SetPercentage(percentage);

            Debug.Log($"{gameObject.name} progress: {currentFilledSegments}/{totalSegments}");

            if (currentFilledSegments >= totalSegments)
            {
                CompleteSegment();
                progressCoroutine = null;
                yield break;
            }

            yield return new WaitForSeconds(segmentFillTime);
        }

        progressCoroutine = null;
    }

    private IEnumerator UnlockAfterDelay()
    {
        yield return new WaitForSeconds(unlockDelay);

        Debug.Log("Unlocker activated: " + gameObject.name);
        isCompleted = true;

        if (nextSegment != null)
        {
            Debug.Log("Activating next segment: " + nextSegment.name);
            nextSegment.SetActive(true);
        }
    }

    private void CompleteSegment()
    {
        Debug.Log(gameObject.name + " COMPLETED!");
        isCompleted = true;

        progressBar.SetPercentage(1.0f);

        if (segmentUnlock != null)
            segmentUnlock.SetActive(true);

        if (selectionRing != null)
            selectionRing.SetActive(false);

        StartCoroutine(ActivateNextSegmentAfterDelay());
    }

    private IEnumerator ActivateNextSegmentAfterDelay()
    {
        yield return new WaitForSeconds(1.0f);

        if (nextSegment != null)
        {
            Debug.Log("Activating next segment: " + nextSegment.name);
            nextSegment.SetActive(true);
        }
        else
        {
            Debug.Log("No next segment assigned to " + gameObject.name);
        }
    }

    private IEnumerator ResetAfterDelay()
    {
        yield return new WaitForSeconds(2.0f);

        if (!IsHovered && !isCompleted)
        {
            Debug.Log("Resetting progress for " + gameObject.name);
            currentFilledSegments = 0;
            progressBar.SetPercentage(0);
        }

        resetCoroutine = null;
    }
}
