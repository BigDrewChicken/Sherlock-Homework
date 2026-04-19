using UnityEngine;
using TMPro;

public class ReceiptBehavior : MonoBehaviour
{
    [Header("UI Text Displays")]
    // References to the actual labels on your receipt in the game world
    public TextMeshProUGUI item1Text;
    public TextMeshProUGUI item2Text;
    public TextMeshProUGUI item3Text;
    public TextMeshProUGUI taxPercentText;
    public TextMeshProUGUI amountGivenText;
    public TextMeshProUGUI changeDueText; 

    [Header("Player Input Fields")]
    // Where the player types their answers to solve the puzzle
    public TMP_InputField totalInput;
    public TMP_InputField totalTaxedInput;
    
    // Internal variables to hold the math data
    private int val1, val2, val3;
    private float taxPercentage;
    private int calculatedSum;
    private float calculatedTotalWithTax;
    private float amountGiven;
    private float calculatedChange; 
    private float wrongChange;      

    // OnEnable runs every time the object is turned on (good for resetting the puzzle)
    void OnEnable()
    {
        GenerateCaseData();
    }

    // This function generates the random prices and does the math
    void GenerateCaseData()
    {
        // 1. Generate random prices for three items
        val1 = Random.Range(5, 16); 
        val2 = Random.Range(5, 16);
        val3 = Random.Range(5, 16);

        // 2. Generate a random tax percentage (e.g., 5% to 20%)
        int taxInt = Random.Range(5, 21);
        taxPercentage = taxInt / 100f; // Convert whole number (15) to a decimal (0.15)

        // 3. Calculate the math the player is SUPPOSED to do
        calculatedSum = val1 + val2 + val3;
        float taxAmount = calculatedSum * taxPercentage;
        calculatedTotalWithTax = calculatedSum + taxAmount;

        // 4. Determine how much cash the customer handed over (rounded up + extra)
        amountGiven = Mathf.Ceil(calculatedTotalWithTax + Random.Range(10, 20));
        calculatedChange = amountGiven - calculatedTotalWithTax;

        // --- STORY LOGIC: SHORT-CHANGING ---
        // We calculate a "wrong" change amount to see if the player notices
        float shortAmount = Random.Range(1.0f, 5.0f);
        wrongChange = calculatedChange - shortAmount;

        // Ensure we don't accidentally get a negative number
        if (wrongChange < 0) wrongChange = 0;
        // ------------------------------------

        // 5. Send these numbers to the UI text components so the player can see them
        if (item1Text) item1Text.text = val1.ToString();
        if (item2Text) item2Text.text = val2.ToString();
        if (item3Text) item3Text.text = val3.ToString();
        if (taxPercentText) taxPercentText.text = taxInt.ToString();
        if (amountGivenText) amountGivenText.text = amountGiven.ToString("F2"); // "F2" means 2 decimal places
        
        // IMPORTANT: The receipt shows the WRONG change to trick the player
        if (changeDueText) changeDueText.text = wrongChange.ToString("F2");

        // Clear out any old text in the player's input boxes
        if (totalInput) totalInput.text = "";
        if (totalTaxedInput) totalTaxedInput.text = "";
        
        // Print the answers to the Console so you can cheat while testing!
        Debug.Log($"[Cheat Sheet] Total: {calculatedSum} | Total w/Tax: {calculatedTotalWithTax:F2} | Correct Change: {calculatedChange:F2} | Short-changed: {wrongChange:F2}");
    }

    // --- GETTER FUNCTIONS ---
    // These allow other scripts (like ConversationManager) to ask this script for the correct answers.
    public int GetTargetSum() => calculatedSum;
    public float GetTargetTotalWithTax() => calculatedTotalWithTax;
    public float GetAmountGiven() => amountGiven;
    public float GetCorrectChange() => calculatedChange;
    public float GetWrongChange() => wrongChange;
}