using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Game : Singleton<Game>
{
    [SerializeField] TextMeshProUGUI m_difficultyWarningText = null;
    [SerializeField] Image m_screenTint = null;
    [SerializeField] [Range(1.0f, 10.0f)] float m_transitionTime = 3.0f;
    [SerializeField] public bool m_canPause = false;
    [SerializeField] public int m_levelsCompleted = 0;
    [SerializeField] public int m_playerSlams = 1;

    public bool Paused { get { return Time.timeScale == 0.0f || m_paused; } }
    
    float m_time = 0.0f;
    const float TIME_BONUS = 11.0f;
    public bool m_paused = false;
    static Game ms_game;
    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        if (ms_game == null)
        {
            ms_game = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        m_time += Time.deltaTime;

        if (m_canPause)
        {

        }
    }

    public void StartLevelTransition(string level, string text)
    {
        GetTexts();
        ResetColor();
        m_difficultyWarningText.text = "This next level will be " + text + " difficult";

        StartCoroutine(LevelTransition(level));
    }

    private void GetTexts()
    {
        GameObject canvas = GameObject.Find("PlayerHUD");
        Transform[] children = canvas.GetComponentsInChildren<Transform>();
        foreach (RectTransform t in children)
        {
            if (t.name.Equals("DifficultyWarning"))
            {
                m_difficultyWarningText = t.gameObject.GetComponent<TextMeshProUGUI>();
            }
            else if (t.name.Equals("Tint"))
            {
                m_screenTint = t.gameObject.GetComponent<Image>();
            }
        }
    }

    IEnumerator LevelTransition(string level)
    {
        ResetColor();

        for (float i = 0.0f; i <= 1.0f; i += Time.deltaTime / (m_transitionTime / 2.5f))
        {
            Color c = m_difficultyWarningText.color;
            c.a = i;
            m_difficultyWarningText.color = c;
            yield return null;
        }

        for (float i = 0.0f; i <= 1.0f; i += Time.deltaTime / (m_transitionTime / 1.5f))
        {
            Color c = m_screenTint.color;
            c.a = i;
            m_screenTint.color = c;
            yield return null;
        }

        LoadScene(level);
    }

    private void ResetColor()
    {
        Color color1 = m_difficultyWarningText.color;
        color1.a = 0.0f;
        m_difficultyWarningText.color = color1;
        Color color2 = m_difficultyWarningText.color;
        color2.a = 0.0f;
        m_difficultyWarningText.color = color2;
    }

    public void ResetValues()
    {
        m_playerSlams = 1;
        m_levelsCompleted = 1;
    }

    public void StartTimer()
    {
        m_time = 0.0f;
    }

    public void NextLevel()
    {
        print("you completed that level in " + m_time + "seconds");
        if (m_time <= TIME_BONUS) PlayerBonusChance();
        m_levelsCompleted++;
        LoadScene("MainWorld");
    }

    private void PlayerBonusChance()
    {
        int x = Random.Range(0, 2);

        if (x == 1)
        {
            m_playerSlams++;
            print("YOU GOT A BONUS SLAM!!!");
        }
    }

    public void EndGame()
    {
        LoadScene("EndScreen");
    }

    public void LoadScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }

    public void Unpause()
    {
        Time.timeScale = 1.0f;
        print("unpaused");
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
