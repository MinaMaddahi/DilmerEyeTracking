using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class GazePatternLock : MonoBehaviour
{
    public static GazePatternLock Instance;

    [SerializeField] private Transform cameraTransform; // Assign the camera (eye tracking source)
    [SerializeField] private float gazeDistance = 5f;
    [SerializeField] private LayerMask patternLayer; // Layer for pattern balls
    [SerializeField] private LineRenderer patternLine; // Line to show drawn pattern

    private List<GameObject> selectedBalls = new List<GameObject>(); // Store gaze sequence
    public List<string> correctPattern = new List<string> { "PatternBall1", "PatternBall5", "PatternBall9" }; // Define correct pattern

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        RaycastHit hit;
        Vector3 direction = cameraTransform.forward;

        if (Physics.Raycast(cameraTransform.position, direction, out hit, gazeDistance, patternLayer))
        {
            GameObject hitBall = hit.transform.gameObject;

            // Check if it's a pattern ball and not already selected
            if (hitBall.CompareTag("PatternBall") && !selectedBalls.Contains(hitBall))
            {
                selectedBalls.Add(hitBall);
                DrawPatternLine();
            }
        }
    }

    void DrawPatternLine()
    {
        patternLine.positionCount = selectedBalls.Count;
        for (int i = 0; i < selectedBalls.Count; i++)
        {
            patternLine.SetPosition(i, selectedBalls[i].transform.position);
        }
    }

    public void CheckPattern()
    {
        if (selectedBalls.Count != correctPattern.Count)
        {
            Debug.Log("❌ Pattern Incorrect (Length Mismatch)");
            ResetPattern();
            return;
        }

        for (int i = 0; i < correctPattern.Count; i++)
        {
            if (selectedBalls[i].name != correctPattern[i])
            {
                Debug.Log("❌ Pattern Incorrect (Wrong Sequence)");
                ResetPattern();
                return;
            }
        }

        Debug.Log("✅ Pattern Correct! Unlocking...");
        // Add your unlock action here (e.g., load new scene, open door, etc.)
    }

    public void ResetPattern()
    {
        selectedBalls.Clear();
        patternLine.positionCount = 0;
        Debug.Log("🔄 Pattern Reset");
    }
}
