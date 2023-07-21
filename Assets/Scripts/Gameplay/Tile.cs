using System;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    public Action<Tile> OnClickOnTile;
    public Vector2Int pos;
    public GameObject unitOnTile;
    
    [SerializeField] private Image groundImage;
    [SerializeField] private Image environmentTileImage;
    [SerializeField] private Image animalTileImage;
    [SerializeField] private Image blueTargetImage;
    [SerializeField] private Image redTargetImage;
    [SerializeField] private Image selectedOutlineImage;
    [SerializeField] private GameObject fog;
    [SerializeField] private Button getInfoButton;
    
    private string _tileName = "";
    private bool _isHomeOnTile = false;
    private bool _isSelected = false;
    
    public void SelectTile()
    {
        if (fog.activeSelf || _isSelected)
        {
            LevelManager.Instance.SelectObject(null);
            return;
        }
        _isSelected = true;
        LevelManager.Instance.currentName = _tileName;
        LevelManager.Instance.SelectObject(gameObject);
        selectedOutlineImage.gameObject.SetActive(true);

        GetInfoTile();
    }
    
    [Button()]
    public void UnlockTile()
    {
        fog.SetActive(false);
        groundImage.enabled = true;
    }

    public string GetTileName()
    {
        return _tileName;
    }

    public void SetEnvironmentSprite(Sprite sprite, string tileName)
    {
        if(sprite == null || _isHomeOnTile) return;
        environmentTileImage.sprite = sprite;
        environmentTileImage.enabled = true;
        
        if (_tileName != "")
            _tileName = _tileName + ", " + tileName;
        else
            _tileName = tileName;
    }
    
    public void SetAnimalSprite(Sprite sprite, string tileName)
    {
        if(sprite == null || _isHomeOnTile) return;
        animalTileImage.sprite = sprite;
        animalTileImage.enabled = true;
        
        if (_tileName != "")
            _tileName = _tileName + ", " + tileName;
        else
            _tileName = tileName;
    }

    public void SetGroundSprite(Sprite sprite)
    {
        groundImage.sprite = sprite;
    }

    public void ShowBlueTarget()
    {
        if (unitOnTile != null)
        {
            ShowRedTarget();
            return;
        }
        blueTargetImage.gameObject.SetActive(true);
    }

    public void ShowRedTarget()
    {
        redTargetImage.gameObject.SetActive(true);
    }
    
    public void HideTargets()
    {
        blueTargetImage.gameObject.SetActive(false);
        redTargetImage.gameObject.SetActive(false);
    }
    
    public bool IsTileFree()
    {
        return unitOnTile == null;
    }
    
    public bool IsCanMoveTo()
    {
        return blueTargetImage.gameObject.activeSelf;
    }
    
    public bool IsCanAttackTo()
    {
        return redTargetImage.gameObject.activeSelf;
    }
    
    public RectTransform GetEnvironmentRectTransform()
    {
        return environmentTileImage.GetComponent<RectTransform>();
    }

    public void BuildHome()
    {
        _isHomeOnTile = true;
        _tileName = "Home";
    }
    
        
    public void HideTargetsTime()
    {
        var inValY = 0;
        DOTween.To(() => inValY, x => inValY = x, 0, 0.01f).OnComplete((() =>
        {
            blueTargetImage.gameObject.SetActive(false);
            redTargetImage.gameObject.SetActive(false);   
        }));
    }

    
    private void Start()
    {
        getInfoButton.onClick.AddListener(SelectTile);
        LevelManager.Instance.OnObjectSelect += SelectEvent;
    }

    private void OnDestroy()
    {
        LevelManager.Instance.OnObjectSelect -= SelectEvent;
    }

    private void SelectEvent(GameObject pastO, GameObject currO)
    {
        if(currO == null || currO.TryGetComponent(out Tile tile))
            HideTargetsTime();
        
        if(_isSelected == false) return;
        if(pastO == gameObject)
            DeselectedTile();
    }

    private void DeselectedTile()
    {
        selectedOutlineImage.gameObject.SetActive(false);
        _isSelected = false;
    }

    private void GetInfoTile()
    {
        OnClickOnTile?.Invoke(this);
    }
}
