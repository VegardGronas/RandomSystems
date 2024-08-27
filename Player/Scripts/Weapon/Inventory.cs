using UnityEngine;
using UnityEngine.InputSystem;

public class Inventory : MonoBehaviour
{
    [SerializeField]
    PlayerEquipment m_Equipment;

    [SerializeField]
    WeaponRoot m_MainWeapon;

    [SerializeField]
    WeaponRoot m_SecondaryWeapon;

    [SerializeField]
    InputActionReference m_MainWeaponAction;

    [SerializeField]
    InputActionReference m_SecondaryWeaponAction;

    private void OnEnable()
    {
        m_MainWeaponAction.action.performed += MainWeaponInput;
        m_SecondaryWeaponAction.action.performed += SecondaryWeaponInput;
    }

    private void OnDisable()
    {
        m_MainWeaponAction.action.performed -= MainWeaponInput;
        m_SecondaryWeaponAction.action.performed -= SecondaryWeaponInput;
    }

    private void MainWeaponInput(InputAction.CallbackContext context)
    {
        m_Equipment.EquipWeapon(m_MainWeapon, WeapnEquipType.MainHand);
    }

    private void SecondaryWeaponInput(InputAction.CallbackContext context)
    {
        m_Equipment.EquipWeapon(m_SecondaryWeapon, WeapnEquipType.MainHand);
    }
}