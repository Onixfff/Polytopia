using DG.Tweening;
using Gameplay.SO;
using UnityEngine;
using UnityEngine.UI;

public class TechIcon : MonoBehaviour
{
    [SerializeField] private Button techButton; 
    [SerializeField] private GameObject lockObject; 
    [SerializeField] private GameObject lineObject;
    [SerializeField] private GameObject buyObject;
    [SerializeField] private Color unlockColor;
    [SerializeField] private Color buyColor;

    public TechInfo.Technology type;
    public bool isTechUnlock = false;

    public void OpenTech()
    {
        techButton.onClick?.Invoke();
    }
    
    private void Start()
    {
        if (lockObject != null)
        {
            lockObject.SetActive(true);
            techButton.enabled = false;
        }
        else
        {
            isTechUnlock = true;
        }
        
        lineObject.SetActive(true);
        lineObject.transform.SetParent(transform.parent.parent.GetChild(0).transform);
        var inVal = 0;
        DOTween.To(() => inVal, x => x = inVal, 1, 0.03f).OnComplete(() =>
        {
            if (LevelManager.Instance.gameBoardWindow.playerCiv.technologies.Contains(type))
            {
                if (techButton != null) 
                    techButton.onClick.Invoke();
            }
        });
    }

    public void UnlockTech()
    {
        isTechUnlock = true;
        lineObject.GetComponent<Image>().color = unlockColor;
        if (lockObject != null) lockObject.SetActive(false);
        techButton.enabled = true;
    }

    public void BuyTech()
    {
        isTechUnlock = false;
        if (lockObject != null) lockObject.SetActive(false);
        lineObject.GetComponent<Image>().color = buyColor;
        buyObject.SetActive(true);
        techButton.enabled = false;
    }
}
