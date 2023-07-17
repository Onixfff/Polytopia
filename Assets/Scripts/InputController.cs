using UnityEngine;
using UnityEngine.InputSystem;

public class InputTest : MonoBehaviour
{
    [SerializeField] private InputActionReference mouseActionReference;

    private void Awake()
    {
        mouseActionReference.action.Enable();
        mouseActionReference.action.performed += MouseTest;
    }

    private void OnDestroy()
    {
        mouseActionReference.action.Disable();
    }

    private void MouseTest(InputAction.CallbackContext context)
    {
        Debug.Log("mouse");
    }
}
