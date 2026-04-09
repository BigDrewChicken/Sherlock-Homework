using UnityEngine;

public class ReceiptSubmit : MonoBehaviour
{
    public ReceiptBehavior mathLogic;
    public ReceiptInteract receiptInteract;

    public void OnSubmitPressed()
    {
        float playerSum = 0;
        float playerTaxed = 0;

        float.TryParse(mathLogic.totalInput.text, out playerSum);
        float.TryParse(mathLogic.totalTaxedInput.text, out playerTaxed);

        float targetSum = mathLogic.GetTargetSum();
        float targetTaxed = mathLogic.GetTargetTotalWithTax();

        bool sumCorrect = Mathf.Approximately(playerSum, targetSum);
        bool taxedCorrect = Mathf.Abs(playerTaxed - targetTaxed) < 0.02f;

        if (sumCorrect && taxedCorrect)
        {
            // If correct, just close everything and lock the puzzle
            receiptInteract.CloseInteraction(true);
        }
        else
        {
            string errorMessage = "";

            if (!sumCorrect)
            {
                errorMessage = "Wait... these numbers don't add up. I should re-calculate the subtotal of these three items.\n(Press [F] to try again)";
            }
            else if (!taxedCorrect)
            {
                errorMessage = "The subtotal is correct, but the final total with tax is off. I need to double-check my percentage math.\n(Press [F] to try again)";
            }

            if (receiptInteract != null)
            {
                receiptInteract.ShowMathError(errorMessage);
            }
        }
    }
}