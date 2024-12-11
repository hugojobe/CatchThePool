using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DebugPersistantObjectsCreator : MonoBehaviour
{
    public bool spawnGameInstance = true;
    
    private void Awake()
    {
        if (GameInstance.instance == null && spawnGameInstance)
            CreateNewObject<GameInstance>();
            
        
        
        if(AudioManager.instance == null)
            CreateNewObject<AudioManager>();
        
        
        if(CSceneManager.instance == null)
            CreateNewObject<CSceneManager>();
        
    }

    private void CreateNewObject<T>() where T : Component {
        GameObject newObject = new GameObject();
        newObject.name = "[Debug] " + typeof(T).Name;
        T newComponent = newObject.AddComponent<T>();
        
        if(typeof(T) == typeof(GameInstance)) {
            GameInstance.instance.debugMode = true;
            SetGamepadIDs();
        }
        
        DontDestroyOnLoad(newObject);
    }
    
    private void SetGamepadIDs() {
        try
        {
            Gamepad[] pads = Gamepad.all.ToArray();
            GameInstance.instance.gamepadIDs = new List<int>();
            GameInstance.instance.gamepadIDs.Add(pads[0].deviceId);
            GameInstance.instance.gamepadIDs.Add(pads[1].deviceId);
        } catch {}
    }
}
