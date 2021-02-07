using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public MatchSettings settings;

    public void Awake()
    {
        if(instance != null)
        {
            Debug.Log("Error: More than one GameManager in the scene");
        }
        instance = this;
    }

    #region Player tracking
    private static Dictionary<string, Player> players = new Dictionary<string, Player>();

    public static void RegisterPlayer(string _netId, Player _player)
    {
        string _playerId = "Player " + _netId;
        players.Add(_playerId, _player);
        _player.transform.name = _playerId;
    }

    public static void UnRegisterPlayer(string _playerId)
    {
        players.Remove(_playerId);
    }
    public static Player GetPlayer(string playerId)
    {
        return players[playerId];
    }

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(200, 200, 200, 500));
        GUILayout.BeginVertical();

        foreach (string playerId in players.Keys)
        {
            GUILayout.Label(playerId + " - " + players[playerId].transform.name);
        }

        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
    #endregion
}
