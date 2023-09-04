using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CivilisationChooser : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private CivilisationInfo civilisationInfo;
    [SerializeField] private GameObject civInfoWindow;

    private void Awake()
    {
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(ShowInfoWindow);
    }
    
    private void ShowInfoWindow()
    {
        GameManager.Instance.playerCivInfo = civilisationInfo;
        civInfoWindow.SetActive(true);
    }
}
