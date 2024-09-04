using UnityEngine;

public class Target : MonoBehaviour
{
    public void Hit(float damage)
    {
        Debug.Log(gameObject.name);
        OnHit(damage);
    }

    protected virtual void OnHit(float damage) { }
}