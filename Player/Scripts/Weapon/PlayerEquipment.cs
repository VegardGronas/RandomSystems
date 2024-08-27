using UnityEngine;

public class PlayerEquipment : MonoBehaviour
{
    [SerializeField]
    CameraManager m_CameraManager;

    [SerializeField]
    Transform m_MainHand;

    [SerializeField]
    Transform m_OffHand;

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
        weapon = Instantiate(weapon);

        weapon.transform.SetParent(parent);

        m_MainHand.localPosition = weapon.RightHandPosition;
        m_MainHand.rotation = Quaternion.Euler(weapon.RightHandRotation);

        m_OffHand.localPosition = weapon.LeftHandPosition;
        m_OffHand.rotation = Quaternion.Euler(weapon.LeftHandRotation);

        weapon.transform.localPosition = Vector3.zero;
        weapon.Equip(m_CameraManager, m_OffHand);
    }
}