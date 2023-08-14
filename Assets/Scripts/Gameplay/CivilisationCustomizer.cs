using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CivilisationCustomizer : MonoBehaviour
{
    [SerializeField] private List<Image> heads;
    [SerializeField] private List<Image> battleShipHeads;
    [SerializeField] private List<Image> animals;
    [SerializeField] private List<Image> fruits;
    [SerializeField] private List<Image> trees;
    [SerializeField] private List<Image> backs;
    [SerializeField] private List<Image> grounds;
    [SerializeField] private List<Image> mountains;

    private CivilisationInfo _civilisationInfo;

    private void Awake()
    {
        var inVal = 0;
        DOTween.To(() => inVal, x => x = inVal, 1, 1).OnComplete(() =>
        {
            _civilisationInfo = LevelManager.Instance.gameBoardWindow.playerCiv.civilisationInfo;
            Customize();
        });
    }

    private void Customize()
    {
        heads ??= new List<Image>(); 
        battleShipHeads ??= new List<Image>(); 
        animals ??= new List<Image>(); 
        fruits ??= new List<Image>(); 
        trees ??= new List<Image>(); 
        backs ??= new List<Image>(); 
        grounds ??= new List<Image>(); 
        mountains ??= new List<Image>(); 
        foreach (var image in animals)
        {
            image.sprite = _civilisationInfo.AnimalSprite;
        }
        foreach (var image in heads)
        {
            image.sprite = _civilisationInfo.HeadSprite;
        }
        foreach (var image in battleShipHeads)
        {
            image.sprite = _civilisationInfo.BattleShipHeadSprite;
        }
        foreach (var image in fruits)
        {
            image.sprite = _civilisationInfo.FruitSprite;
        }
        foreach (var image in trees)
        {
            image.sprite = _civilisationInfo.TreeSprite;
        }
        foreach (var image in backs)
        {
            image.color = _civilisationInfo.CivilisationColor;
        }
        foreach (var image in grounds)
        {
            image.sprite = _civilisationInfo.GroundSprite;
        }
        foreach (var image in mountains)
        {
            image.sprite = _civilisationInfo.MountainSprite;
        }
    }
}
