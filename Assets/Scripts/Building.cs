using System;
using JetBrains.Annotations;
using UnityEngine;

[CreateAssetMenu(fileName = "Building", menuName = "ScriptableObjects/Building", order = 1)]
public class Building : ScriptableObject
{
    public new string name;
    [UsedImplicitly] public GameObject assetReference;
    public Sprite iconReference;

    public event Action<Building> OnConstructionStarted;

    public void Construct()
    {
        Debug.Log($"Constructing {name}");

        // Pass 'this' to give the event handler the Building object
        OnConstructionStarted?.Invoke(this);
    }
}