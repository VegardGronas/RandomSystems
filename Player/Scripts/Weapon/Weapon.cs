using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField]
    bool m_Reloading = false;

    public bool Reloading => m_Reloading;

    [SerializeField]
    bool m_SingleBurst = false;

    public bool SingleBurst => m_SingleBurst;


    [SerializeField]
    bool m_RecoilInProgress = false;

    public bool RecoilInProgress => m_RecoilInProgress;

    [SerializeField]
    Animator m_Animator;

    [SerializeField]
    GameObject m_HitEffect;

    public GameObject HitEffect => m_HitEffect;

    /// <summary>
    /// Add some kind of shooting effect, or recoil when fired
    /// </summary>
    public void Recoil()
    {
        if(m_Animator != null)
        {
            m_Animator.SetTrigger("Fire");
            m_RecoilInProgress = true;
        }
    }

    /// <summary>
    /// If weapon should reload do it here
    /// </summary>
    public void Reload()
    {

    }

    public void RecoilEnd()
    {
        m_RecoilInProgress = false;
    }
}