using UnityEngine;
using TMPro;

public class ReceiptSubmit : MonoBehaviour
{
    // References to the other scripts so this one can "talk" to them.
    public ReceiptBehavior mathLogic;      // Used to get the correct answers.
    public ReceiptInteract receiptInteract; // Used to close the UI or trigger errors.

    // This is called when the "Submit" button on your receipt UI is clicked.
    public void OnSubmitPressed()
    {
        float playerSum = 0;
        float playerTaxed = 0;

        // 1. CONVERT STRING TO NUMBER
        // float.TryParse looks at the text field. If it's a number, it saves it to 'playerSum'.
        // This prevents the game from crashing if the player accidentally types letters.
        float.TryParse(mathLogic.totalInput.text, out playerSum);
        float.TryParse(mathLogic.totalTaxedInput.text, out playerTaxed);

        // 2. GET THE ANSWERS
        // We ask the mathLogic script what the numbers SHOULD be.
        float targetSum = mathLogic.GetTargetSum();
        float targetTaxed = mathLogic.GetTargetTotalWithTax();

        // 3. COMPARE ANSWERS
        // Mathf.Approximately is used for whole numbers or simple floats.
        bool sumCorrect = Mathf.Approximately(playerSum, targetSum);

        // For the taxed total, we use 'Mathf.Abs' comparison. 
        // Floats can be messy (e.g., 10.5000001). 
        // This checks if the player's answer is within 0.02 of the target.
        bool taxedCorrect = Mathf.Abs(playerTaxed - targetTaxed) < 0.02f;

        // 4. THE VERDICT
        if (sumCorrect && taxedCorrect)
        {
            // SUCCESS: Both answers are right! 
            // We tell ReceiptInteract to shut down the puzzle and mark it as 'Solved'.
            receiptInteract.CloseInteraction(true);
        }
        else
        {
            // FAILURE: Something is wrong.
            // We tell ReceiptInteract to trigger an error message.
            // We pass 'true' if the Subtotal was wrong, or 'false' if it was just the Tax.
            receiptInteract.TriggerMathError(!sumCorrect);
        }
    }
}