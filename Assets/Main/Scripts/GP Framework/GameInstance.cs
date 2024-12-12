using System.Collections.Generic;
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
}