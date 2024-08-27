using UnityEngine;
using UnityEngine.InputSystem;

public enum WeapnEquipType { MainHand, OffHand, BothHands }
public class WeaponRoot : MonoBehaviour
{
    [SerializeField]
    Weapon m_Weapon;

    [SerializeField]
    InputActionReference m_FireAction;

    private bool m_IsFiering = false;

    CameraManager m_CameraManager;

    [SerializeField]
    Vector3 m_LeftHandPosition;

    public Vector3 LeftHandPosition => m_LeftHandPosition;

    [SerializeField]
    Vector3 m_LeftHandRotation;

    public Vector3 LeftHandRotation => m_LeftHandRotation;

    [SerializeField]
    Vector3 m_RightHandPosition;

    public Vector3 RightHandPosition => m_RightHandPosition;

    [SerializeField]
    Vector3 m_RightHandRotation;

    public Vector3 RightHandRotation => m_RightHandRotation;

    Transform m_LookAt;

    private void OnDisable()
    {
        m_FireAction.action.performed -= FireInput;
        m_FireAction.action.canceled -= FireInput;
    }

    public void Equip(CameraManager cameraManager, Transform lookAt)
    {
        m_LookAt = lookAt;
        m_CameraManager = cameraManager;
        m_FireAction.action.performed += FireInput;
        m_FireAction.action.canceled += FireInput;
    }

    public void UnEquip()
    {
        m_CameraManager = null;
        m_FireAction.action.performed -= FireInput;
        m_FireAction.action.canceled -= FireInput;
    }

    private void Update()
    {
        if (m_IsFiering) Fire();

        transform.rotation = Quaternion.LookRotation(m_LookAt.position - transform.position);
    }

    private void FireInput(InputAction.CallbackContext context)
    {
        if (context.performed) m_IsFiering = true;
        else m_IsFiering = false;
    }

    private void Fire()
    {
        if (m_Weapon.Reloading) return;
        
        Ray ray = m_CameraManager.Camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            if(hit.collider.TryGetComponent<Enemy>(out Enemy enemy))
            {
                Destroy(hit.collider.gameObject);
            }
        }

        m_Weapon.Recoil();
    }
}