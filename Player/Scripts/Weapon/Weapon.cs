using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField]
    bool m_Reloading = false;

    public bool Reloading => m_Reloading;

    /// <summary>
    /// Add some kind of shooting effect, or recoil when fired
    /// </summary>
    public void Recoil()
    {
        
    }

    /// <summary>
    /// If weapon should reload do it here
    /// </summary>
    public void Reload()
    {

    }
}