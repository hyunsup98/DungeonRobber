using System.Collections.Generic;
using UnityEngine;

public enum StatType
{
    MaxHp,
    CurrentHP,
    MoveSpeed,
    RunSpeed,
}

public class BaseStat : MonoBehaviour
{
    [field : SerializeField]
    public Dictionary<StatType, float> stats = new()
    {
        {StatType.MaxHp, 0 },
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
