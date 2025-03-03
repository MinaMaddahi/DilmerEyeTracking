using UnityEngine;

public class PatternBallGazeHandler : MonoBehaviour
{
    [SerializeField] private PatternUnlockController patternController;
    private bool wasLookedAt = false;

    void Start()
    {
        // Automatically find the pattern controller if not assigned
        if (patternController == null)
        {
            patternController = FindObjectOfType<PatternUnlockController>();
            if (patternController == null)
            {
                Debug.LogError("❌ PatternUnlockController not found! Assign in Inspector or create one in the scene.");
                enabled = false;
                return;
            }
        }

        // Set tag to differentiate pattern balls
        gameObject.tag = "PatternBall";
    }

    public void OnStartLooking()
    {
        if (!wasLookedAt)
        {
            Debug.Log($"👁 Looking at: {gameObject.name}");
            patternController.AddBallToPattern(gameObject);
            wasLookedAt = true;  // Prevent duplicate entries
        }
    }

    public void OnStopLooking()
    {
        wasLookedAt = false;
    }
}
