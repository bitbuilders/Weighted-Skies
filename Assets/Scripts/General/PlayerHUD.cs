using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class PlayerHUD : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI m_slamText = null;
    [SerializeField] Image m_tint = null;
    [SerializeField] Image m_clickPrevention = null;

    Player m_player;

    private void Start()
    {
        Game.Instance.m_paused = true;
        m_player = GameObject.Find("Player").GetComponent<Player>();
        m_tint.gameObject.SetActive(true);
        m_clickPrevention.gameObject.SetActive(true);
        StartCoroutine(Fade());
    }

    private void Update()
    {
        m_slamText.text = "(E) - Slam Ground <color=orange>[ " + m_player.m_slams + " Remaining ]</color>";
    }

    IEnumerator Fade()
    {
        for (float i = 1.0f; i >= 0.0f; i -= Time.deltaTime * 2.0f)
        {
            Color c = m_tint.color;
            c.a = i;
            m_tint.color = c;

            yield return null;
        }

        Color color = m_tint.color;
        color.a = 0.0f;
        m_tint.color = color;

        m_clickPrevention.gameObject.SetActive(false);
        Time.timeScale = 0.0f;
        Game.Instance.m_paused = false;
    }
}
