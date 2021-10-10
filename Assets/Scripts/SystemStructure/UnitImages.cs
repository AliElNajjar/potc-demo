using System;
using UnityEngine;

[Serializable]
public class UnitImages
{
    [SerializeField] private Sprite portraitImage;
    [SerializeField] private Sprite towerImage;
    [SerializeField] private Sprite fullImage;

    public Sprite PortraitImage => portraitImage;
    public Sprite TowerImage => towerImage;
    public Sprite FullImage => fullImage;
}
