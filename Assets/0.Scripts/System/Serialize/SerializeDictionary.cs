using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��ųʸ��� ����ȭ ���ִ� Ŭ����
/// </summary>
/// <typeparam name="K"> ��ųʸ��� Ű(Key) </typeparam>
/// <typeparam name="V"> ��ųʸ��� ��(Value) </typeparam>
[System.Serializable]
public class SerializeDictionary<K, V> : Dictionary<K, V>, ISerializationCallbackReceiver
{
    //����ȭ �� Ű ����Ʈ
    [SerializeField] private List<K> keys = new List<K>();

    //����ȭ �� �� ����Ʈ
    [SerializeField] private List<V> values = new List<V>();

    //����ȭ �� �� ����ϴ� �޼���
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

    //����ȭ�� �����͸� �ٽ� ��ü�� ��ȯ�� �� ����ϴ� �޼���
    public void OnAfterDeserialize()
    {
        Clear();

        for(int i = 0; i < keys.Count; i++)
        {
            Add(keys[i], values[i]);
        }
    }
}
