using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour
{
    public bool isOnline;
    public int timerValue;
    [SerializeField] private CarController player;
    
    [Header("UI")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI rewardText;
    [SerializeField] private TextMeshProUGUI coinText;

    private SaveValues saveData;
    private int coins = 0;

    [PunRPC]
    private void Start()
    {
        saveData = SaveLoadManager.Load();
        coins = saveData.coins;
        if(isOnline) player = PhotonNetwork.Instantiate(saveData.ownedVehicles[saveData.selectedVehicleIndex].model.name, Vector3.zero, Quaternion.identity, 0).GetComponent<CarController>();
        player.data = saveData.ownedVehicles[saveData.selectedVehicleIndex];

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
        saveData.coins = coins;
        SaveLoadManager.Save(saveData);
    }

    public void OnBackToMenuButtonClick()
    {
        if (isOnline) PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene(0);
    }
}
