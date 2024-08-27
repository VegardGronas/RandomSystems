using UnityEngine;

public class PlayerEquipment : MonoBehaviour
{
    [SerializeField]
    Transform m_MainHand;

    [SerializeField]
    Transform m_OffHand;

    public void SlotWeapon(Weapon weapon, WeapnEquipType type)
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

    private void SlotWeapon(Weapon weapon, Transform parent)
    {
        weapon.transform.SetParent(parent);
        weapon.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(Vector3.zero));
    }
}