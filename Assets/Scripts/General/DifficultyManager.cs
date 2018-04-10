using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultyManager : Singleton<DifficultyManager>
{
    [SerializeField] [Range(1, 10)] int m_minDifficulty = 1;
    [SerializeField] [Range(1, 10)] int m_maxDifficulty = 6;

    string[] m_difficultyWords = new string[]
    {
        "<color=red>extremely</color>",
        "<color=orange>very</color>",
        "<color=yellow>fairly</color>",
        "<color=green>somewhat</color>",
        "<color=purple>not very</color>",
        "<color=#00BBFFFF>hardly</color>"
    };

    public int Difficulty
    {
        get { return m_difficulty; }
        set
        {
            int val = Mathf.Clamp(value, m_minDifficulty, m_maxDifficulty);
            m_difficulty = val;
        }
    }

    static DifficultyManager ms_difficultyManager = null;
    int m_difficulty = 0;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        if (ms_difficultyManager == null)
        {
            ms_difficultyManager = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public string GetDifficultyWord()
    {
        return m_difficultyWords[Difficulty - 1];
    }
}
