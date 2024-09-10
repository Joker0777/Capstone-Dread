using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Mod")]
public class Mod : ScriptableObject
{

    [SerializeField] protected GameObject modVisual;
    [SerializeField] protected Sprite modIcon;
    [SerializeField] protected ModType modType;

    public GameObject ModVisual { get => modVisual; set => modVisual = value; }
    public Sprite ModIcon { get => modIcon; set => modIcon = value; }
    public ModType ModType { get => modType; set => modType = value; }
}
