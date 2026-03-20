using UnityEngine;

public class Interactable : MonoBehaviour
{
    public GameObject interactUI;
    public Camera mainCamera;
    public Camera interactionCamera;
    public GameObject player;

    private bool playerInRange = false;
    private bool isInteracting = false;

    void Start()
    {
        interactUI.SetActive(false);
        interactionCamera.gameObject.SetActive(false);
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.F))
        {
            if (!isInteracting)
            {
                StartInteraction();
            }
            else
            {
                StopInteraction();
            }
        }
    }

    void StartInteraction()
    {
        isInteracting = true;

        mainCamera.gameObject.SetActive(false);
        interactionCamera.gameObject.SetActive(true);

        player.SetActive(false); // disable movement
    }

    void StopInteraction()
    {
        isInteracting = false;

        mainCamera.gameObject.SetActive(true);
        interactionCamera.gameObject.SetActive(false);

        player.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            interactUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            interactUI.SetActive(false);
        }
    }
}