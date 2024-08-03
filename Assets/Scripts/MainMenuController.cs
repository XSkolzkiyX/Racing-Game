using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private Image loadingImage;
    [SerializeField] private Animator crossfade;
    private AsyncOperation loading;

    private void FixedUpdate()
    {
        if (loading == null) return;
        loadingImage.fillAmount = loading.progress / .9f;
    }

    public void OnPlayButtonClick()
    {
        crossfade.SetBool("Fade", true);
        StartCoroutine(LoadGame());
    }

    public void OnQuitButtonClick()
    {
        Application.Quit();
    }

    IEnumerator LoadGame()
    {
        yield return new WaitForSeconds(.5f);
        loading = SceneManager.LoadSceneAsync(1);
    }
}
