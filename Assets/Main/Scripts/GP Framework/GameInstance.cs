using UnityEngine;

public class GameInstance : MonoBehaviour
{
    public static GameInstance instance;
    
    public bool debugMode;

    public int playerCount;
    
    private void Awake() {
        if(instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }
    }
    
    private async void Start() {
        if (!debugMode) {
            CSceneManager.LoadScene(SceneNames.MainMenu);
        }
    }
}
