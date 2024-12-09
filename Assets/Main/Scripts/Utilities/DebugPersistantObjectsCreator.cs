using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugPersistantObjectsCreator : MonoBehaviour
{
    private void Awake()
    {
        if (GameInstance.instance == null)
        {
            CreateNewObject<GameInstance>();
        }
        
        if(AudioManager.instance == null)
        {
            CreateNewObject<AudioManager>();
        }
        
        if(CSceneManager.instance == null)
        {
            CreateNewObject<CSceneManager>();
        }
    }

    private void CreateNewObject<T>() where T : Component {
        GameObject newObject = new GameObject();
        newObject.name = "[Debug] " + typeof(T).Name;
        T newComponent = newObject.AddComponent<T>();
        
        if(typeof(T) == typeof(GameInstance)) {
            GameInstance.instance.debugMode = true;
        }
        
        DontDestroyOnLoad(newObject);
    }
}
