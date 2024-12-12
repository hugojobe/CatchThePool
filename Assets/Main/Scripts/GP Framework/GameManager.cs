using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public GameObject playerPrefab;
    
    [Space]
    public Transform[] spawnpoints;
    public Transform ringCenter;
    private List<Transform> availableSpawnpoints;
    
    [Space]
    public float closePlayerRadius;
    
    private void Start()
    {
        availableSpawnpoints = spawnpoints.ToList();

        if (GameInstance.instance.playerCount == 2)
        {
            int startIndex = Random.value < 0.5f ? 0 : 2;
            availableSpawnpoints.RemoveRange(startIndex, 2);
        }

        SpawnAndInitPlayers();
    }

    private void Update()
    {
        LookForClosePlayers();
    }

    private void LookForClosePlayers()
{
    List<PlayerController> players = GameInstance.instance.playerControllers;

    for (int i = 0; i < players.Count; i++)
    {
        bool isCloseToAnyPlayer = false;

        for (int j = 0; j < players.Count; j++)
        {
            if (i == j) continue;

            float distance = Vector3.Distance(players[i].transform.position, players[j].transform.position);
            if (distance < closePlayerRadius)
            {
                isCloseToAnyPlayer = true;
                players[i].isCloseToAnyPlayer = true;
                Vector3 directionToOtherPlayer = players[j].transform.position - players[i].transform.position;
                players[i].watchRotation = Quaternion.LookRotation(directionToOtherPlayer, Vector3.up).eulerAngles;
            }
        }

        if (!isCloseToAnyPlayer)
        {
            players[i].isCloseToAnyPlayer = false;
        }
    }
}

    private void SpawnAndInitPlayers()
    {
        for (int i = 0; i < GameInstance.instance.playerCount; i++)
        {
            int selectedSpawnpointIndex = Random.Range(0, availableSpawnpoints.Count);
            GameObject newPlayer = Instantiate(playerPrefab, transform.TransformPoint(availableSpawnpoints[selectedSpawnpointIndex].position), Quaternion.identity);
            availableSpawnpoints.RemoveAt(selectedSpawnpointIndex);
            
            PlayerInput input = newPlayer.GetComponent<PlayerInput>();
            Gamepad gamepad = Gamepad.all.FirstOrDefault(g => g.deviceId == GameInstance.instance.gamepadIDs[i]);
            input.SwitchCurrentControlScheme(gamepad);
            
            Rigidbody rb = newPlayer.GetComponent<Rigidbody>();
            
            PlayerController playerController = newPlayer.GetComponent<PlayerController>();
            playerController.index = i;
            playerController.gamepadID = GameInstance.instance.gamepadIDs[i];
            playerController.input = input;
            playerController.chickenConfig = GameInstance.instance.playerConfigs[i];
            playerController.rb = rb;
            
            Vector3 direction = ringCenter.position - newPlayer.transform.position;
            playerController.watchRotation = Quaternion.LookRotation(direction, newPlayer.transform.up).eulerAngles;
            
            playerController.Initialize();
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(ringCenter.position, closePlayerRadius/2);
    }
}
