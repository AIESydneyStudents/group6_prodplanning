using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PauseManager : MonoBehaviour
{

    public GameObject PauseUI;
    public GameObject PausePanel;

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
        if (Input.GetKeyDown(KeyCode.Return))
        {
            TogglePause(!active);
        }
    }

    public void TogglePause(bool on)
    {
        active = on;

        if (on)
        {
            PauseUI.SetActive(true);
            PausePanel.SetActive(true);
            player.LockPlayerMovement(true);
            oldPlayerState = player.PlayerCurrentState;
            player.ChangeState(F_PlayerMovement.PlayerState.Cutscene);
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0;
        }
        else
        {
            PauseUI.SetActive(false);
            PausePanel.SetActive(false);
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
