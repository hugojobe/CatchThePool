using System.Collections;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class CSceneManager : MonoBehaviour
{
    public static CSceneManager instance;
    
    private CanvasGroup fadeCanvas;
    private bool isLoading = false;
    
    private GameObject transitionCanvasPrefab;

    private void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
        
        transitionCanvasPrefab = Resources.Load<GameObject>("SceneTransition/SceneTransitionCanvas");
    }

    public static void LoadScene(Scene scene, bool async = true) {
        LoadScene(scene.name, async);
    }

    public static void LoadScene(string sceneName, bool async = true) {
        if (instance.isLoading) return;
        
        instance.StartSceneTransition(() => {
            if (async) {
                instance.LoadSceneAsync(sceneName);
            } else {
                instance.LoadSceneSync(sceneName);
            }
        });
    }

    private void StartSceneTransition(System.Action loadSceneAction) {
        isLoading = true;
        StartCoroutine(SceneTransitionCoroutine(loadSceneAction));
    }

    private IEnumerator SceneTransitionCoroutine(System.Action loadSceneAction) {
        CreateFadeCanvas();
        TransitionCanvas transitionCanvas = fadeCanvas.gameObject.GetComponent<TransitionCanvas>();
        yield return fadeCanvas.DOFade(1, 1f).WaitForCompletion();
        
        loadSceneAction.Invoke();
        transitionCanvas.loaderImage.SetActive(true);
        
        yield return new WaitUntil(() => SceneManager.GetActiveScene().isLoaded);

        yield return new WaitForSeconds(0.15f);
        
        transitionCanvas.loaderImage.SetActive(false);
        yield return fadeCanvas.DOFade(0, 1f).WaitForCompletion();
        
        Destroy(fadeCanvas.gameObject);
        isLoading = false;
    }

    private void CreateFadeCanvas() {
        if (!fadeCanvas) {
            fadeCanvas = Instantiate(transitionCanvasPrefab).GetComponent<CanvasGroup>();
            fadeCanvas.alpha = 0;
            DontDestroyOnLoad(fadeCanvas.gameObject);
        }
    }

    private void LoadSceneSync(string sceneName) {
        SceneManager.LoadScene(sceneName);
    }

    private async Task LoadSceneAsync(string sceneName) {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncOperation.isDone) {
            await Task.Yield();
        }
    }
}