using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class GameInstance : MonoBehaviour
{
    public static GameInstance instance;
    
    public bool debugMode;

    public int playerCount;
    public List<ChickenConfig> playerConfigs = new List<ChickenConfig>();
    public List<PlayerController> playerControllers = new List<PlayerController>();
    
    public List<int> gamepadIDs = new List<int>();
    
    public Dictionary<Gamepad, Coroutine> gamepadRumbleCoroutines = new Dictionary<Gamepad, Coroutine>();
    
    [Space] 
    public List<bool> playerAlive;
    public List<int> playerScores;
    public List<int> playerKills;
    public List<int> playerDeaths;

    public int winningPlayerIndex;
    
    [Space] 
    public int requiredPointsToWin;

    public int currentRoundNumber = 1;
    public Action OnRoundStart;

    public bool isRoundRunning;

    public Color[] playerColors;
    public Sprite[] playerNumberImages;
    
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
            if (GameLoopManager.instance != null)
            {
                if (GameLoopManager.instance.skipMainMenu)
                {
                    StartCoroutine(MainMenuskipCoroutine());
                }
                else
                    CSceneManager.LoadScene(SceneNames.MainMenu);
            }
        }
    }
    
    private IEnumerator MainMenuskipCoroutine()
    {
        yield return new WaitForSeconds(1.5f);
        CSceneManager.LoadScene(SceneNames.PSM);
    }

    public void InitNewRound()
    {
        StartCoroutine(InitNewRoundCoroutine());
    }
    
    public IEnumerator FirstRoundCoroutine()
    {
        yield return new WaitForSecondsRealtime(0.05f);
        OnRoundStart?.Invoke();
        yield return new WaitForSeconds(1.2f);
        playerControllers.ForEach(player => player.playerState = PlayerState.Normal);
        isRoundRunning = true;
        
        playerControllers.ForEach(player => player.ShowPopup());
    }

    public IEnumerator InitNewRoundCoroutine()
    {
        currentRoundNumber++;
        OnRoundStart?.Invoke();
        
        var availableSpawnpoints = GameManager.instance.spawnpoints.ToList();
        if (GameInstance.instance.playerCount == 2)
        {
            int startIndex = Random.value < 0.5f ? 0 : 2;
            availableSpawnpoints.RemoveRange(startIndex, 2);
        }
        
        for (int i = 0; i < playerControllers.Count; i++)
        {
            int selectedSpawnpointIndex = Random.Range(0, availableSpawnpoints.Count);
            
            playerControllers[i].rb.MovePosition(transform.TransformPoint(availableSpawnpoints[selectedSpawnpointIndex].position));
            Vector3 direction = GameManager.instance.ringCenter.position - playerControllers[i].transform.position;
            playerControllers[i].watchRotation = Quaternion.LookRotation(direction, playerControllers[i].transform.up).eulerAngles;
            
            availableSpawnpoints.RemoveAt(selectedSpawnpointIndex);
            
            playerControllers[i].playerState = PlayerState.Uncontrolled;
            playerAlive[i] = true;
            playerControllers[i].damageable.currentHealth = playerControllers[i].chickenConfig.chickenHealthGameplay;
            playerControllers[i].circleRend.enabled = true;
            playerControllers[i].ReleaseRopesWithoutDelay();
            playerControllers[i].abilityCooldownElapsed = true;
            playerControllers[i].dashCooldownElapsed = true;
            playerControllers[i].circlePercent = 1;

            if (playerControllers[i].chickenConfig.chickenType == ParticlesMPB.ChickenSelected.ConPollo)
            {
                playerControllers[i].trailInstance.GetComponent<TrailRenderer>().emitting = false;
                playerControllers[i].flameInstance.GetComponent<ParticleSystem>().Stop();
            }

            playerControllers[i].feathers.ResetFeathers();
            playerControllers[i].ropePullArrow.transform.parent.gameObject.SetActive(false);
        }
        
        yield return new WaitForSeconds(1.5f);
        playerControllers.ForEach(player => player.playerState = PlayerState.Normal);
        isRoundRunning = true;
    }
    
    public void EndRound()
    {
        if (!isRoundRunning) return;
        
        isRoundRunning = false;
        
        foreach (PlayerController player in playerControllers)
        {
            if (player.playerState == PlayerState.Dead)
                continue;
            
            player.playerState = PlayerState.Dead;
            
            int trueIndex = playerAlive.Select((value, index) => new { value, index })
                .Where(x => x.value)
                .Select(x => x.index)
                .SingleOrDefault();
            
            GameManager.instance.ShowEndOfRoundUi(trueIndex, currentRoundNumber);
            
            playerScores[trueIndex]++;
        }
    }

    public void CheckForEndOfRound()
    {
        if (playerAlive.Count(value => value) == 1)
            EndRound();
        
        else if(playerAlive.Count(value => value) == 0)
            GameManager.instance.ShowEndOfRoundUi(-1, currentRoundNumber);
    }
}