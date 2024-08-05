using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour
{
    public int timerValue;
    [SerializeField] private CarController player;
    
    [Header("UI")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI rewardText;
    [SerializeField] private TextMeshProUGUI coinText;
    private int coins = 0;

    void Start()
    {
        SaveValues saveData = SaveLoadManager.Load();
        player.data = saveData.ownedVehicles[saveData.selectedVehicleIndex];
        coins = saveData.coins;

        coinText.text = coins.ToString();
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        InvokeRepeating(nameof(CalculateTime), 0, 1);
    }

    private void CalculateTime()
    {
        timerValue--;
        int minutes = timerValue / 60;
        int seconds = timerValue % 60;
        timerText.text = $"{minutes}:{seconds}";
        if(timerValue <= 0) EndTimer();
    }

    private void EndTimer()
    {
        CancelInvoke(nameof(CalculateTime));
        player.canMove = false;
        timerValue = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        coins += player.driftScore / 100;
        rewardText.text = coins.ToString();
        gameOverPanel.SetActive(true);
    }

    public void OnBackToMenuButtonClick()
    {
        SceneManager.LoadScene(0);
    }
}
