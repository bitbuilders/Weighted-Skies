using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class EndLevel : Singleton<EndLevel>
{
    [SerializeField] Image m_tint = null;

    public void EndFade()
    {
        m_tint.gameObject.SetActive(true);
        StartCoroutine(StartFade());
    }

    IEnumerator StartFade()
    {
        Color color = m_tint.color;
        color.a = 0.0f;
        m_tint.color = color;

        for (float i = 0.0f; i <= 1.0f; i += Time.deltaTime * 2.0f)
        {
            Color c = m_tint.color;
            c.a = i;
            m_tint.color = c;

            yield return null;
        }

        Color color2 = m_tint.color;
        color2.a = 1.0f;
        m_tint.color = color2;

        Game.Instance.NextLevel();
    }
}
