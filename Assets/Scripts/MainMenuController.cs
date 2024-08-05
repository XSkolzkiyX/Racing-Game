using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public int coins;
    public List<VehicleData> vehicles;
    public List<VehicleData> ownedVehicles;
    [SerializeField] private Material vehicleMaterial;
    [SerializeField] private Color[] colors;
    [SerializeField] private Transform carPlace;

    [Header("Animation")]
    [SerializeField] private Animator notEnoughMoneyAnimator;
    [SerializeField] private Animator crossfade;
    
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private Image loadingImage;
    [SerializeField] private List<Button> vehicleButtons;
    [SerializeField] private Button selectButton;
    [SerializeField] private TextMeshProUGUI selectText;

    private int selectedVehicleIndex;
    private AsyncOperation loading;
    private VehicleData selectedData;

    private void Start()
    {
        Time.timeScale = 1.0f;
        SaveLoadManager.path = Application.persistentDataPath;
        SaveValues data = SaveLoadManager.Load();
        if (data == null)
        {
            SaveLoadManager.Save(new SaveValues
            {
                coins = coins,
                ownedVehicles = ownedVehicles,
                selectedVehicleIndex = selectedVehicleIndex
            });
            Debug.Log("Initial Save");
        }
        else
        {
            Debug.Log("Get Values");
            coins = data.coins;
            ownedVehicles = data.ownedVehicles;
            selectedVehicleIndex = data.selectedVehicleIndex;
        }
        coinText.text = coins.ToString();
        for(int i = 0; i < vehicles.Count; i++)
        {
            if (ownedVehicles.Contains(vehicles[i]))
                Destroy(vehicleButtons[i].gameObject);
        }
    }

    private void FixedUpdate()
    {
        if (loading == null) return;
        loadingImage.fillAmount = loading.progress / .9f;
    }

    public void OnChangeVehicleButtonClick(int direction)
    {
        selectedVehicleIndex += direction;
        if(selectedVehicleIndex < 0) selectedVehicleIndex = ownedVehicles.Count - 1;
        else if(selectedVehicleIndex > ownedVehicles.Count - 1) selectedVehicleIndex = 0;
        if(carPlace.childCount > 0) Destroy(carPlace.GetChild(0).gameObject);
        Instantiate(ownedVehicles[selectedVehicleIndex].model, carPlace);
        selectButton.interactable = true;
        selectText.text = "Select";
        vehicles.IndexOf(ownedVehicles[selectedVehicleIndex]);
    }

    public void OnSelectButtonClick()
    {
        selectedData = ownedVehicles[selectedVehicleIndex];
        SaveLoadManager.Save(new SaveValues { coins = coins, ownedVehicles = ownedVehicles, selectedVehicleIndex = selectedVehicleIndex });
    }

    public void OnBuyVehicleButtonClick(int index)
    {
        if (vehicles[index].price > coins)
        {
            notEnoughMoneyAnimator.SetTrigger("Start");
            return;
        }
        coins -= vehicles[index].price;
        coinText.text = coins.ToString();
        ownedVehicles.Add(vehicles[index]);
        Destroy(vehicleButtons[index].gameObject);
        SaveLoadManager.Save(new SaveValues { coins = coins, ownedVehicles = ownedVehicles, selectedVehicleIndex = selectedVehicleIndex });
    }

    public void OnSelectColorButtonClick(int colorIndex)
    {
        vehicleMaterial.color = colors[colorIndex];
    }

    public void OnPlayButtonClick(int sceneIndex)
    {
        crossfade.SetBool("Fade", true);
        StartCoroutine(LoadGame(sceneIndex));
    }

    public void OnQuitButtonClick()
    {
        Application.Quit();
    }

    IEnumerator LoadGame(int sceneIndex)
    {
        yield return new WaitForSeconds(.5f);
        loading = SceneManager.LoadSceneAsync(sceneIndex);
    }
}
