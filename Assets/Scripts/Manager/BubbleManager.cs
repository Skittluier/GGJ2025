using SpiritLevel;
using SpiritLevel.Player;
using System.Collections.Generic;
using UnityEngine;

public class BubbleManager : MonoBehaviour
{
    public GameObject bubblePrefab;
    public List<PlayerIdentity> bubbleList;
    public Transform[] startPosition;

    public void Awake()
    {
        bubbleList = new List<PlayerIdentity>();
    }

    private void OnEnable()
    {
        PlayerManager.Instance.OnPlayerJoined += PlayerJoined;
        PlayerManager.Instance.OnPlayerLeft += PlayerLeft;
    }

    private void OnDisable()
    {
        PlayerManager.Instance.OnPlayerJoined -= PlayerJoined;
        PlayerManager.Instance.OnPlayerLeft -= PlayerLeft;
    }

    private void Start()
    {

    }

    private void PlayerJoined(PlayerIdentity playerID)
    {
        GameObject player = GameObject.Instantiate(bubblePrefab,null);
        Bubble bubble = player.GetComponent<Bubble>();
        bubble.player.UUID = playerID.UUID;
        bubble.player.PlayerIndex = playerID.PlayerIndex;
        player.transform.position = startPosition[bubbleList.Count].position;
        bubbleList.Add(playerID);
    }

    private void PlayerLeft(string playerUUID)
    {
        PlayerManager.Instance.Players.ForEach(p => 
        {
            if(p.UUID == playerUUID)
            {
                PlayerManager.Instance.Players.Remove(p);
            }
        });
    }
}
