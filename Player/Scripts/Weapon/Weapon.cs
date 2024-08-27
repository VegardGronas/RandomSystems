using UnityEngine;
using UnityEngine.InputSystem;

public enum WeapnEquipType { MainHand, OffHand, BothHands }
public class Weapon : MonoBehaviour
{
    [SerializeField]
    InputActionReference m_FireAction;

    private bool m_IsFiering = false;

    CameraManager m_CameraManager;

    private void OnDisable()
    {
        m_FireAction.action.performed -= FireInput;
    }

    public void Equip(CameraManager cameraManager)
    {
        m_CameraManager = cameraManager;
        m_FireAction.action.performed += FireInput;
    }

    public void UnEquip()
    {
        m_CameraManager = null;
        m_FireAction.action.performed -= FireInput;
    }

    private void Update()
    {
        if (m_IsFiering) Fire();
    }

    private void FireInput(InputAction.CallbackContext context)
    {
        if (context.performed) m_IsFiering = true;
        else m_IsFiering = false;
    }

    private void Fire()
    {
        Ray ray = m_CameraManager.Camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            if(hit.collider.TryGetComponent<Enemy>(out Enemy enemy))
            {
                Destroy(hit.collider.gameObject);
            }
        }
    }
}