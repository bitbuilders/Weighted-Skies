using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class Leaderboard : MonoBehaviour
{
    [SerializeField] GameObject m_playerDataTemplate = null;
    [SerializeField] Transform m_playerDataLocation = null;
    [SerializeField] [Range(1, 100)] int m_leaderBoardLength = 5;


    List<PlayerData> m_playerData = new List<PlayerData>();
    string m_fileName = "\\playerData.json";

    private void Start()
    {
        CreatePlayerData();
    }

    private void CreatePlayerData()
    {
        string path = Application.streamingAssetsPath;

        if (File.Exists(path + m_fileName))
        {
            m_playerData = GetAllPlayers(path + m_fileName).players;
        }
        
        for (int i = 0; i < m_leaderBoardLength; i++)
        {
            CreateUI(m_playerData[i]);
        }
    }

    private GameObject CreateUI(PlayerData player)
    {
        GameObject playerData = Instantiate(m_playerDataTemplate, Vector3.zero, Quaternion.identity, m_playerDataLocation);
        Transform[] children = playerData.GetComponentsInChildren<Transform>();
        TextMeshProUGUI name = null;
        TextMeshProUGUI level = null;
        foreach (Transform child in children)
        {
            if (child.name.Equals("Name"))
            {
                name = child.GetComponent<TextMeshProUGUI>();
            }
            else if (child.name.Equals("Level"))
            {
                level = child.GetComponent<TextMeshProUGUI>();
            }
        }
        name.text = player.name;
        level.text = "Level " + player.level;

        return playerData;
    }

    private AllPlayerData GetAllPlayers(string path)
    {
        string jsonData = File.ReadAllText(path);
        AllPlayerData data = JsonUtility.FromJson<AllPlayerData>(jsonData);

        return data;
    }

    private void SaveAllPlayers(AllPlayerData players, string path)
    {
        string jsonData = JsonUtility.ToJson(players);
        File.WriteAllText(path, jsonData);
    }

    public void Submit(TextMeshProUGUI playerName)
    {
        string levelText = Game.Instance.m_levelsCompleted.ToString();
        string pName = playerName.text;
        if (string.IsNullOrEmpty(pName.Trim())) pName = "Anonymous";
        PlayerData player = new PlayerData() { name = pName, level = levelText };
        m_playerData.Add(player);

        AllPlayerData data = new AllPlayerData();
        data.players = new List<PlayerData>();
        data.players.AddRange(m_playerData);
        data.players.Sort(new GreaterPlayerData());
        string path = Application.streamingAssetsPath + m_fileName;
        SaveAllPlayers(data, path);

        Game.Instance.ResetValues();
        Game.Instance.LoadScene("MainMenu");
    }
}

public class GreaterPlayerData : IComparer<PlayerData>
{
    int IComparer<PlayerData>.Compare(PlayerData x, PlayerData y)
    {
        return int.Parse(y.level) - int.Parse(x.level);
    }

}
