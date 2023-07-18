using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    [SerializeField] private TileSO tileSo;
    [SerializeField] private Image image;
    [SerializeField] private RectTransform centerTile;
    [SerializeField] private GameObject fog;
    private GameObject _unitOnTile;
    private RectTransform _decoration;
    
    
    [Button()]
    public void UnlockTile()
    {
        fog.SetActive(false);
        image.enabled = true;
        centerTile.gameObject.SetActive(true);
    }
    
    private void Start()
    {
        image.sprite = tileSo.tileSprite;
        GetDecoration();
    }
    
    [Button()]
    public void GetDecoration()
    {
        if(_decoration != null)
            Destroy(_decoration.gameObject);
        var instantiate = Instantiate(ResourceStorage.Instance.GetRandomEnvironments(), centerTile);
        _decoration = instantiate.GetComponent<RectTransform>();
        _decoration.anchoredPosition = Vector2.zero;
    }

    private string GetTileName()
    {
        return tileSo.tileName;
    }
    
    private string GetTileDescription()
    {
        return tileSo.tileDescription;
    }
    
    private Sprite GetTileSprite()
    {
        return tileSo.tileSprite;
    }

    private bool IsTileFree()
    {
        return _unitOnTile == null;
    }
}
