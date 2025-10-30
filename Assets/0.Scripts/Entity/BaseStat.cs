using System;
using Unity.VisualScripting;
using UnityEngine;

public enum StatType
{
    Health,
    CurrentHP,
    MoveSpeed,
    RunSpeed,
}

[System.Serializable]
public class BaseStat
{
    [SerializeField]
    private SerializeDicStats stats = new SerializeDicStats()
    {
        {StatType.Health, 0 },
        {StatType.CurrentHP, 0 },
        {StatType.MoveSpeed, 0 },
        {StatType.RunSpeed, 0 }
    };

    private void Start()
    {
        
    }

    private void Update()
    {
        
    }
}
