using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class GameInstance : MonoBehaviour
{
    public static GameInstance instance;
    
    public bool debugMode;

    public int playerCount;
    public List<ChickenConfig> playerConfigs = new List<ChickenConfig>();
    public List<PlayerController> playerControllers = new List<PlayerController>();
    
    public List<int> gamepadIDs = new List<int>();
    
    public Dictionary<Gamepad, Coroutine> gamepadRumbleCoroutines = new Dictionary<Gamepad, Coroutine>();
    
    [Space] public List<bool> playerAlive;
    
    private void Awake() {
        if (instance == null)
        { 
            instance = this;
            DontDestroyOnLoad(gameObject);

            if (debugMode)
            {
                Gamepad[] pads = Gamepad.all.ToArray();
                gamepadIDs.Add(pads[0].deviceId);
                gamepadIDs.Add(pads[1].deviceId);
            }
        }
        else
            Destroy(gameObject);
    }
    
    private async void Start() {
        if (!debugMode) {
            CSceneManager.LoadScene(SceneNames.MainMenu);
        }
    }

    public void StartRound()
    {
        playerAlive.ForEach(p => p = true);

        
        var availableSpawnpoints = GameManager.instance.spawnpoints.ToList();
        
        
        for (int i = 0; i < playerControllers.Count; i++)
        {

            if (GameInstance.instance.playerCount == 2)
            {
                int startIndex = Random.value < 0.5f ? 0 : 2;
                availableSpawnpoints.RemoveRange(startIndex, 2);
            }

            int selectedSpawnpointIndex = Random.Range(0, availableSpawnpoints.Count);

            playerControllers[i].transform.position = transform.TransformPoint(availableSpawnpoints[selectedSpawnpointIndex].position);
            
            availableSpawnpoints.RemoveAt(selectedSpawnpointIndex);
        }
    }

    public void EndRound()
    {
        
    }

    public void CheckForEndOfRound()
    {
        
    }
}