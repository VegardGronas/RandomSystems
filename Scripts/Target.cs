using UnityEngine;

public class Target : MonoBehaviour
{
    public void Hit(float damage)
    {
        Debug.Log(gameObject.name);
        OnHit(damage);
    }

    public void Use()
    {
        OnUse();
    }

    protected virtual void OnHit(float damage) { }
    protected virtual void OnUse() { }
}