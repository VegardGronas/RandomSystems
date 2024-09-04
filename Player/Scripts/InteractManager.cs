using UnityEngine;
using UnityEngine.InputSystem;

public class InteractManager : MonoBehaviour
{
    [SerializeField]
    CameraManager m_CameraManager;

    [SerializeField]
    InputActionReference m_UseAction;

    private void OnEnable()
    {
        m_UseAction.action.performed += Use;
    }

    private void OnDisable()
    {
        m_UseAction.action.performed -= Use;
    }

    private void Use(InputAction.CallbackContext context)
    {
        if(m_CameraManager)
        {
            Ray ray = m_CameraManager.Camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if(Physics.Raycast(ray, out hit, Mathf.Infinity)) 
            {
                if(hit.collider.TryGetComponent<Target>(out Target target))
                {
                    target.Use();
                }
            }
        }
    }
}