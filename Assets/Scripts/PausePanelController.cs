using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PausePanelController : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)) 
            ChangePauseState();
    }

    private void ChangePauseState()
    {
        pausePanel.SetActive(!pausePanel.activeSelf);
        Time.timeScale = pausePanel.activeSelf ? 0 : 1;
        Cursor.lockState = pausePanel.activeSelf ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = pausePanel.activeSelf;
    }

    public void OnResumeButtonClick()
    {
        ChangePauseState();
    }
}
