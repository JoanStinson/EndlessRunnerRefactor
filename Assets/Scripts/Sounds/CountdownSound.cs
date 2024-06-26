using UnityEngine;

public class CountdownSound : MonoBehaviour
{
    protected const float k_StartDelay = 0.5f;

    protected AudioSource m_Source;
    protected float m_TimeToDisable;

    private void OnEnable()
    {
        m_Source = GetComponent<AudioSource>();
        m_TimeToDisable = m_Source.clip.length;
        m_Source.PlayDelayed(k_StartDelay);
    }

    private void Update()
    {
        m_TimeToDisable -= Time.deltaTime;

        if (m_TimeToDisable < 0)
        {
            gameObject.SetActive(false);
        }
    }
}
