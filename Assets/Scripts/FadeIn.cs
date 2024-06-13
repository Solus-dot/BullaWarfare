using UnityEngine;
using System.Collections;

public class StepwiseFadeIn : MonoBehaviour
{
	public float fadeInTime =2.5f;          // Time taken for the fade-in effect
	public int numberOfSteps = 5;         // Number of steps for the fade-in

	private CanvasGroup canvasGroup;

	void Start()
	{
		// Ensure the object has a CanvasGroup component
		canvasGroup = GetComponent<CanvasGroup>();

		if (canvasGroup == null)
		{
			// If CanvasGroup component is missing, add one
			canvasGroup = gameObject.AddComponent<CanvasGroup>();
		}

		// Start the stepwise fade-in process
		StartStepwiseFadeIn();
	}

	void StartStepwiseFadeIn()
	{
		// Set initial alpha to 0
		canvasGroup.alpha = 0f;

		// Start the stepwise fade-in coroutine
		StartCoroutine(StepwiseFadeInCoroutine());
	}

	IEnumerator StepwiseFadeInCoroutine()
	{
		float stepTime = fadeInTime / numberOfSteps;

		for (int step = 0; step < numberOfSteps; step++)
		{
			// Calculate the target alpha for this step
			float targetAlpha = (step + 1) / (float)numberOfSteps;

			// Update alpha value
			canvasGroup.alpha = targetAlpha;

			// Wait for the next step
			yield return new WaitForSeconds(stepTime);
		}

		// Ensure alpha is exactly 1 at the end
		canvasGroup.alpha = 1f;
	}
}
