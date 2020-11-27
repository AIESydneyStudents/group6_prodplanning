using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PauseManager : MonoBehaviour
{

    public CanvasGroup PauseGroup;
    public CanvasGroup RootCanvasGroup;

    private bool active = false;

    private F_PlayerMovement player;
    private F_PlayerMovement.PlayerState oldPlayerState = F_PlayerMovement.PlayerState.Gameplay;

    private void Start()
    {
        GameObject[] obj = GameObject.FindGameObjectsWithTag("Player");

        for(int i = 0; i < obj.Length; i++)
        {
            player = obj[i].GetComponent<F_PlayerMovement>();
            if (player != null) break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        bool pressed = Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Escape);

        if(pressed)
        {
            if(!active)
            {
                if(player.enabled != false && player.PlayerCurrentState == F_PlayerMovement.PlayerState.Gameplay)
                {
                    TogglePause(true);
                }
            }
            else
            {
                TogglePause(false);
            }
        }

    }

    public void TogglePause(bool on)
    {
        active = on;

        if (on)
        {
            PauseGroup.alpha = 1;
            PauseGroup.interactable = true;
            PauseGroup.blocksRaycasts = true;
            RootCanvasGroup.blocksRaycasts = true;
            player.LockPlayerMovement(true);
            oldPlayerState = player.PlayerCurrentState;
            player.ChangeState(F_PlayerMovement.PlayerState.Cutscene);
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0;
        }
        else
        {
            PauseGroup.alpha = 0;
            PauseGroup.interactable = false;
            PauseGroup.blocksRaycasts = false;
            RootCanvasGroup.blocksRaycasts = false;
            player.LockPlayerMovement(false);
            Cursor.lockState = CursorLockMode.Locked;
            player.ChangeState(oldPlayerState);
            Time.timeScale = 1;
        }
    }

    public void ExitGame()
    {
        #if UNITY_EDITOR
            EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
