using UnityEngine;

public class PatternValidator : MonoBehaviour
{
    public GazePatternLock patternLock; // Assign the GazePatternLock script in the Inspector

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // Press Space to validate pattern
        {
            patternLock.CheckPattern();
        }

        if (Input.GetKeyDown(KeyCode.R)) // Press R to reset the pattern
        {
            patternLock.ResetPattern();
        }
    }
}
