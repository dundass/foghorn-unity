using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    //[SerializeField] GameObject interactButton;
    private IInteractable _focussedInteractable;

    private void Update()
    {
        if(Input.GetButtonDown("Fire1") && CanInteract())
        {
            InitiateInteraction();
        }
    }

    public bool CanInteract()
    {
        return _focussedInteractable != null;
    }

    public void InitiateInteraction()
    {
        if (_focussedInteractable != null) _focussedInteractable.Interact();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Interactable"))
        {
            _focussedInteractable = collision.GetComponent<IInteractable>();
            //interactButton.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Interactable"))
        {
            IInteractable exitedInteractable = collision.GetComponent<IInteractable>();
            if (_focussedInteractable == exitedInteractable)
            {
                // only hide interact button if we exited the one we just entered!
                // hopefully prevents bug where you enter, enter another, then exit the 1st and the button goes despite still being next to the 2nd
                _focussedInteractable = null;
                //interactButton.SetActive(false);
            }
        }
    }
}
