using UnityEngine;
using TMPro;

public class CaseFile_MathLogic : MonoBehaviour
{
    [Header("UI Text Displays")]
    public TextMeshProUGUI item1Text;
    public TextMeshProUGUI item2Text;
    public TextMeshProUGUI item3Text;
    public TextMeshProUGUI taxPercentText;

    [Header("Player Input Fields")]
    public TMP_InputField totalInput;
    public TMP_InputField totalTaxedInput;
    
    private int val1, val2, val3;
    private float taxPercentage;
    
    private int calculatedSum;
    private float calculatedTotalWithTax;

    void OnEnable()
    {
        GenerateCaseData();
    }

    void GenerateCaseData()
    {
        val1 = Random.Range(1, 16);
        val2 = Random.Range(1, 16);
        val3 = Random.Range(1, 16);

        int taxInt = Random.Range(1, 21);
        taxPercentage = taxInt / 100f; 

        calculatedSum = val1 + val2 + val3;
        
        float taxAmount = calculatedSum * taxPercentage;
        calculatedTotalWithTax = calculatedSum + taxAmount;

        if (item1Text) item1Text.text = val1.ToString();
        if (item2Text) item2Text.text = val2.ToString();
        if (item3Text) item3Text.text = val3.ToString();
        
        if (taxPercentText) taxPercentText.text = taxInt.ToString();

        if (totalInput) totalInput.text = "";
        if (totalTaxedInput) totalTaxedInput.text = "";
        
        Debug.Log($"[Cheat Sheet] Total: {calculatedSum} | Total w/ Tax: {calculatedTotalWithTax}");
    }

    public int GetTargetSum() => calculatedSum;
    public float GetTargetTotalWithTax() => calculatedTotalWithTax;
}