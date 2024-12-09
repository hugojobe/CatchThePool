using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField]
    private GameObject defaultSelectedButton;
    
    private void Start()
    {
        EventSystem.current.SetSelectedGameObject(defaultSelectedButton);
    }

    public void Play()
    {
        CSceneManager.LoadScene(SceneNames.PSM);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
