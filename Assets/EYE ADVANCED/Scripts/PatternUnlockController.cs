using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PatternUnlockController : MonoBehaviour
{
    [Header("Pattern Configuration")]
    [SerializeField] private string patternBallTag = "PatternBall"; // Tag for identifying pattern balls
    [SerializeField] private List<string> correctPattern = new List<string> { "EyeInteractable (9)", "EyeInteractable (10)", "EyeInteractable (11)", "EyeInteractable (12)", "EyeInteractable (17)", "EyeInteractable (21)" };

    [Header("Visual Feedback")]
    [SerializeField] private Material correctPatternMaterial; // Green material for correct pattern
    [SerializeField] private Material incorrectPatternMaterial; // Red material for incorrect pattern
    [SerializeField] private LineRenderer patternLine; // LineRenderer to draw pattern lines
    [SerializeField] private float lineWidth = 0.02f;
    [SerializeField] private Color lineColor = Color.cyan;

    [Header("Events")]
    [SerializeField] private UnityEvent onPatternCorrect; // Event triggered when pattern is correct
    [SerializeField] private UnityEvent onPatternIncorrect; // Event triggered when pattern is incorrect
    [SerializeField] private float patternResetDelay = 2f; // Delay before resetting pattern after completion

    private List<string> currentPattern = new List<string>();
    private List<Transform> selectedBalls = new List<Transform>();
    private Dictionary<string, Transform> patternBalls = new Dictionary<string, Transform>();
    private bool patternCompleted = false;

    void Start()
    {
        GameObject[] balls = GameObject.FindGameObjectsWithTag(patternBallTag);
        foreach (GameObject ball in balls)
        {
            patternBalls.Add(ball.name, ball.transform);
        }

        if (patternLine != null)
        {
            patternLine.startWidth = lineWidth;
            patternLine.endWidth = lineWidth;
            patternLine.startColor = lineColor;
            patternLine.endColor = lineColor;
            patternLine.positionCount = 0;
        }

        Debug.Log($"Pattern Unlock initialized with {patternBalls.Count} balls");
    }

    public void AddBallToPattern(GameObject ball)
    {
        if (patternCompleted || !ball.CompareTag(patternBallTag)) return;

        string ballName = ball.name;
        if (currentPattern.Contains(ballName)) return;

        currentPattern.Add(ballName);
        selectedBalls.Add(ball.transform);

        UpdatePatternLine();
        Debug.Log($"Ball added to pattern: {ballName}. Current pattern: {string.Join(" -> ", currentPattern)}");

        if (currentPattern.Count >= correctPattern.Count)
        {
            CheckPattern();
        }
    }

    private void UpdatePatternLine()
    {
        if (patternLine == null || selectedBalls.Count == 0) return;

        patternLine.positionCount = selectedBalls.Count;
        for (int i = 0; i < selectedBalls.Count; i++)
        {
            patternLine.SetPosition(i, transform.InverseTransformPoint(selectedBalls[i].position));
        }
    }

    private void CheckPattern()
    {
        patternCompleted = true;
        bool isCorrect = currentPattern.Count == correctPattern.Count;

        for (int i = 0; i < correctPattern.Count; i++)
        {
            if (currentPattern[i] != correctPattern[i])
            {
                isCorrect = false;
                break;
            }
        }

        if (isCorrect)
        {
            Debug.Log("✅ Correct pattern entered!");
            onPatternCorrect?.Invoke();
            ChangeBallColors(correctPatternMaterial);
        }
        else
        {
            Debug.Log("❌ Incorrect pattern entered!");
            onPatternIncorrect?.Invoke();
            ChangeBallColors(incorrectPatternMaterial);
        }

        StartCoroutine(ResetPatternAfterDelay());
    }

    private void ChangeBallColors(Material material)
    {
        foreach (Transform ballTransform in selectedBalls)
        {
            MeshRenderer renderer = ballTransform.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                renderer.material = material;
            }
        }
    }

    private IEnumerator ResetPatternAfterDelay()
    {
        yield return new WaitForSeconds(patternResetDelay);
        ResetPattern();
    }

    public void ResetPattern()
    {
        currentPattern.Clear();
        selectedBalls.Clear();
        patternCompleted = false;
        if (patternLine != null)
        {
            patternLine.positionCount = 0;
        }
        Debug.Log("🔄 Pattern reset");
    }
}
