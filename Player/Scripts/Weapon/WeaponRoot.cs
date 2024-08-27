using System;
using UnityEngine;
using UnityEngine.InputSystem;

public enum WeapnEquipType { MainHand, OffHand, BothHands }
public class WeaponRoot : MonoBehaviour
{
    [SerializeField]
    LayerMask m_LayerMask;

    [SerializeField]
    Weapon m_Weapon;

    [SerializeField]
    InputActionReference m_FireAction;

    private bool m_IsFiering = false;

    CameraManager m_CameraManager;

    [SerializeField]
    HandPositions m_HandPositions;

    public HandPositions HandPositions => m_HandPositions;

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


        transform.rotation = Quaternion.LookRotation(m_LookAt.position - transform.position);
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
    }

    private void FireInput(InputAction.CallbackContext context)
    {
        if (m_Weapon.RecoilInProgress) return;
        if (context.performed) m_IsFiering = true;
        else m_IsFiering = false;
    }

    private void Fire()
    {
        if (m_Weapon.Reloading) return;
        if (m_Weapon.SingleBurst) m_IsFiering = false;

        Ray ray = m_CameraManager.Camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, Mathf.Infinity, m_LayerMask))
        {
            if(hit.collider.TryGetComponent<Enemy>(out Enemy enemy))
            {
                Destroy(hit.collider.gameObject);
            }
            else
            {
                if(m_Weapon.HitEffect)
                {
                    GameObject hitEffect = Instantiate(m_Weapon.HitEffect, hit.point, Quaternion.LookRotation(hit.normal));
                    Destroy(hitEffect, 2f);
                }
            }
        }

        m_Weapon.Recoil();
    }
}

[Serializable]
public class HandPositions
{
    [Header("Left Hand")]
    //Left Hand Position
    [SerializeField]
    Vector3 m_LeftHandPosition;

    public Vector3 LeftHandPosition => m_LeftHandPosition;

    //Left Hand Rotation
    [SerializeField]
    Vector3 m_LeftHandRotation;
 
    public Vector3 LeftHandRotation => m_LeftHandRotation;

    //IK Left Hand Position
    [SerializeField]
    Vector3 m_IKLeftHandPosition;

    public Vector3 IKLeftHandPosition => m_IKLeftHandPosition;

    //IK Left Hand Rotation
    [SerializeField]
    Vector3 m_IKLeftHandRotation;

    public Vector3 IKLeftHandRotation => m_IKLeftHandRotation;

    [Header("Right Hand")]
    //Right Hand Position
    [SerializeField]
    Vector3 m_RightHandPosition;

    public Vector3 RightHandPosition => m_RightHandPosition;

    //Right Hand Rotation
    [SerializeField]
    Vector3 m_RightHandRotation;

    public Vector3 RightHandRotation => m_RightHandRotation;

    //IK Right Hand Position
    [SerializeField]
    Vector3 m_IKRightHandPosition;

    public Vector3 IKRightHandPosition => m_IKRightHandPosition;

    //IK Right Hand Rotation
    [SerializeField]
    Vector3 m_IKRightHandRotation;

    public Vector3 IKRightHandRotation => m_IKRightHandRotation;
}