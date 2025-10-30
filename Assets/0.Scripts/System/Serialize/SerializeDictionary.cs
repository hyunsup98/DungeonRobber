using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializeDictionary<K, V> : Dictionary<K, V>, ISerializationCallbackReceiver
{
    //직렬화 할 키 리스트
    [SerializeField] private List<K> keys = new List<K>();

    //직렬화 할 값 리스트
    [SerializeField] private List<V> values = new List<V>();

    //직렬화 할 때 사용하는 메서드
    public void OnBeforeSerialize()
    {
        keys.Clear();
        values.Clear();

        foreach(var pair in this)
        {
            keys.Add(pair.Key);
            values.Add(pair.Value);
        }
    }

    //직렬화된 데이터를 다시 객체로 변환할 때 사용하는 메서드
    public void OnAfterDeserialize()
    {
        this.Clear();

        for(int i = 0; i < keys.Count; i++)
        {
            this.Add(keys[i], values[i]);
        }
    }
}
