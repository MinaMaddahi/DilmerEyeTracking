using UnityEngine;

[RequireComponent(typeof(EyeInteractable))]
public class PatternBallConnector : MonoBehaviour
{
    [SerializeField] private PatternUnlockController patternController;
    private EyeInteractable eyeInteractable;
    private bool wasLookedAt = false;

    void Start()
    {
        // Get the EyeInteractable component
        eyeInteractable = GetComponent<EyeInteractable>();
        if (eyeInteractable == null)
        {
            Debug.LogError($"EyeInteractable component missing on {gameObject.name}");
            enabled = false;
            return;
        }

        // Find pattern controller if not assigned
        if (patternController == null)
        {
            patternController = FindObjectOfType<PatternUnlockController>();
            if (patternController == null)
            {
                Debug.LogError("Pattern controller not found! Please assign in inspector or create one in the scene.");
                enabled = false;
                return;
            }
        }

        // Set the tag for pattern balls
        gameObject.tag = "PatternBall";
    }

    void Update()
    {
        // Check if the ball is being looked at
        if (eyeInteractable == null) return;
        bool isCurrentlyLookedAt = eyeInteractable.IsHovered;

        // If the ball just started being looked at
        if (isCurrentlyLookedAt && !wasLookedAt)
        {
            // Add this ball to the pattern
            patternController.AddBallToPattern(gameObject);
            wasLookedAt = true;  // Update flag to prevent duplicate adds
        }
        else if (!isCurrentlyLookedAt)
        {
            wasLookedAt = false; // Reset flag when gaze leaves
        }
    }
}
