using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    public Action<string> OnClickOnTile;
    public Vector2Int pos;
    
    [SerializeField] private Image groundImage;
    [SerializeField] private Image environmentTileImage;
    [SerializeField] private Image animalTileImage;
    [SerializeField] private GameObject fog;
    [SerializeField] private Button getInfoButton;
    
    private GameObject _unitOnTile;
    private string _tileName = "";
    private bool _isHomeOnTile = false;
    private bool _isSelected = false;

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
        if(_isSelected == false) return;
        if(pastO == gameObject)
            DeselectedTile();
    }

    public void SelectTile()
    {
        if (fog.activeSelf || _isSelected)
        {
            LevelManager.Instance.SelectObject(null);
            return;
        }
        _isSelected = true;
        LevelManager.Instance.SelectObject(gameObject);
        GetInfoTile();
    }

    private void DeselectedTile()
    {
        _isSelected = false;
    }
    
    private void GetInfoTile()
    {
        OnClickOnTile?.Invoke(_tileName);
    }

    [Button()]
    public void UnlockTile()
    {
        fog.SetActive(false);
        groundImage.enabled = true;
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

    public bool IsTileFree()
    {
        return _unitOnTile == null;
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
}
