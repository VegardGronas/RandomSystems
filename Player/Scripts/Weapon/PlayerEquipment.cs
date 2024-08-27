using UnityEngine;

public class PlayerEquipment : MonoBehaviour
{
    [SerializeField]
    CameraManager m_CameraManager;

    [SerializeField]
    Transform m_MainHand;

    [SerializeField]
    Transform m_OffHand;

    public void EquipWeapon(Weapon weapon, WeapnEquipType type)
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

    public void EquipMainHand(Weapon weapon)
    {
        SlotWeapon(weapon, m_MainHand);
    }

    public void EquipOffHand(Weapon weapon)
    {
        SlotWeapon(weapon, m_OffHand);
    }

    private void SlotWeapon(Weapon weapon, Transform parent)
    {
        weapon = Instantiate(weapon);

        weapon.transform.SetParent(parent);
        weapon.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(Vector3.zero));

        weapon.Equip(m_CameraManager);
    }
}