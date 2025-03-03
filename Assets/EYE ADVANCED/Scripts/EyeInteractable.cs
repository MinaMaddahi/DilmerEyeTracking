using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI; // For UI-based progress bar

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class EyeInteractable : MonoBehaviour
{
    public bool IsHovered { get; set; }

    [SerializeField] private Material triangleColor;
    [SerializeField] private UnityEvent<GameObject> OnObjectHover;
    [SerializeField] private Material OnHoverActiveMaterial;
    [SerializeField] private Material OnHoverInactiveMaterial;

    private MeshRenderer meshRenderer;
    public GameObject selectionRing; // Ring object to show selection

    // 🔹 Progress Bar Elements
    public GameObject progressBar; // Assign the Progress Bar (Activity Indicator) in Unity Inspector
    public Image progressFill; // Assign UI Image if using UI-based progress bar
    private float fillAmount = 0f;
    private bool isFilling = false;
    public float fillSpeed = 0.5f; // Adjust fill speed

    // 🔹 Static list to track gaze order
    private static List<string> gazeOrder = new List<string>();

    // 🔹 Define the correct sequence
    private static readonly List<string> correctOrder = new List<string>
    {
        "EyeInteractable",
        "EyeInteractable (1)",
        "EyeInteractable (2)"
    };

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"Scene Loaded: {scene.name}");
        Initialize();
    }

    void Start()
    {
        if (SceneManager.GetActiveScene().isLoaded)
        {
            Initialize();
        }

        // Hide progress bar at start
        if (progressBar != null)
        {
            progressBar.SetActive(false);
        }
    }

    void Initialize()
    {
        meshRenderer = GetComponent<MeshRenderer>();

        if (meshRenderer == null)
        {
            Debug.LogError("MeshRenderer component is missing!");
        }

        if (OnHoverActiveMaterial == null || OnHoverInactiveMaterial == null)
        {
            Debug.LogWarning("Hover materials are not assigned! Please assign them in the Inspector.");
        }
    }

    // 🔹 Called when the user starts looking at this object
    public void OnStartLooking()
    {
        Debug.Log($"🔹 OnStartLooking() triggered for {gameObject.name}");

        if (!gazeOrder.Contains(gameObject.name))
        {
            gazeOrder.Add(gameObject.name);
            Debug.Log($"✅ Gaze Order Updated: {string.Join(" -> ", gazeOrder)}");
        }

        // Change material when looking at the object
        if (meshRenderer != null && OnHoverActiveMaterial != null)
        {
            meshRenderer.material = OnHoverActiveMaterial;
        }

        // Show selection ring
        if (selectionRing != null)
        {
            selectionRing.SetActive(true);
        }

        OnObjectHover?.Invoke(gameObject);

        // 🔹 Check if gaze order is correct and update color
        CheckSequenceAndChangeColor();

        // 🔹 Start the progress bar
        if (progressBar != null)
        {
            progressBar.SetActive(true);
            StartCoroutine(FillProgressBar());
        }
    }

    // 🔹 Called when the user stops looking at this object
    public void OnStopLooking()
    {
        Debug.Log(gameObject.name + " is being unhovered.");

        // Restore default material
        if (meshRenderer != null && OnHoverInactiveMaterial != null)
        {
            meshRenderer.material = OnHoverInactiveMaterial;
        }

        // Hide selection ring
        if (selectionRing != null)
        {
            selectionRing.SetActive(false);
        }

        // 🔹 Reset progress bar
        if (progressBar != null)
        {
            StopAllCoroutines();
            fillAmount = 0f;
            if (progressFill != null)
            {
                progressFill.fillAmount = 0f; // Reset UI progress bar fill amount
            }
            progressBar.SetActive(false);
        }
    }

    // 🔹 Check if the user looked at objects in the correct order, then change color
    private void CheckSequenceAndChangeColor()
    {
        if (gazeOrder.Count == correctOrder.Count && !gazeOrder.Except(correctOrder).Any())
        {
            Debug.Log("✅ Correct order detected! Changing all sphere colors.");

            // Find all spheres and change their color
            EyeInteractable[] allSpheres = FindObjectsOfType<EyeInteractable>();
            foreach (var sphere in allSpheres)
            {
                if (sphere.meshRenderer != null && sphere.triangleColor != null)
                {
                    sphere.meshRenderer.material = sphere.triangleColor;
                }
            }
        }
    }

    // 🔹 Fill progress bar gradually
    IEnumerator FillProgressBar()
    {
        isFilling = true;
        while (fillAmount < 1f)
        {
            fillAmount += Time.deltaTime * fillSpeed;
            if (progressFill != null)
            {
                progressFill.fillAmount = fillAmount;
            }
            yield return null;
        }
    }

    // 🔹 Reset gaze order if needed
    public static void ResetGazeOrder()
    {
        gazeOrder.Clear();
        Debug.Log("🔄 Gaze order has been reset.");
    }
}
