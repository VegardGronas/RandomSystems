using UnityEngine;

public class PlayerEquipment : MonoBehaviour
{
    [SerializeField]
    CameraManager m_CameraManager;

    [SerializeField]
    Transform m_MainHand;

    [SerializeField]
    Transform m_IKMainHand;

    [SerializeField]
    Transform m_OffHand;

    [SerializeField]
    Transform m_IKOffHand;

    WeaponRoot m_CurrentWeapon;

    public void EquipWeapon(WeaponRoot weapon, WeapnEquipType type)
    {
        switch(type)
        {
            case WeapnEquipType.MainHand:
                SlotWeapon(weapon, m_MainHand);
                break;
            case WeapnEquipType.OffHand:
                SlotWeapon(weapon, m_OffHand);
                break;
            case WeapnEquipType.BothHands:
                //Dont know yet
                break;
        }
    }

    public void EquipMainHand(WeaponRoot weapon)
    {
        SlotWeapon(weapon, m_MainHand);
    }

    public void EquipOffHand(WeaponRoot weapon)
    {
        SlotWeapon(weapon, m_OffHand);
    }

    private void SlotWeapon(WeaponRoot weapon, Transform parent)
    {
        if(m_CurrentWeapon != null) Destroy(m_CurrentWeapon.gameObject);

        weapon = Instantiate(weapon);

        weapon.transform.SetParent(parent);

        m_MainHand.localPosition = weapon.HandPositions.RightHandPosition;
        m_MainHand.localRotation = Quaternion.Euler(weapon.HandPositions.RightHandRotation);
        m_IKMainHand.localPosition = weapon.HandPositions.IKRightHandPosition;
        m_IKMainHand.localRotation = Quaternion.Euler(weapon.HandPositions.IKRightHandRotation);

        m_OffHand.localPosition = weapon.HandPositions.LeftHandPosition;
        m_OffHand.localRotation = Quaternion.Euler(weapon.HandPositions.LeftHandRotation);
        m_IKOffHand.localPosition = weapon.HandPositions.IKLeftHandPosition;
        m_IKOffHand.localRotation = Quaternion.Euler(weapon.HandPositions.IKLeftHandRotation);

        weapon.transform.localPosition = Vector3.zero;
        weapon.Equip(m_CameraManager, m_OffHand);

        m_CurrentWeapon = weapon;
    }
}