using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct StatData
{
    public float amount;
    public float price;
}

[CreateAssetMenu]
public class SOStatData : ScriptableObject
{
    public string upgradeName;
    public List<StatData> datas = new List<StatData>();
}
