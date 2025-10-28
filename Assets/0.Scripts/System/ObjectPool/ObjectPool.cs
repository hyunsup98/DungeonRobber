using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public abstract class ObjectPool<T> : Singleton<ObjectPool<T>> where T : MonoBehaviour
{
    //�� ���� Ÿ�Ը� ������ poolQueue�� ���� ����
    protected Queue<T> poolQueue = new Queue<T>();

    //����, ������ �� ���� ��Ȳ�� ���������� ���� ������ �ִ� ��� poolDic�� ���� ����
    protected Dictionary<string, Queue<T>> poolDic = new Dictionary<string, Queue<T>>();

    /// <summary>
    /// ���׸� ������Ʈ Ǯ �� ������Ʈ ��������
    /// </summary>
    /// <param name="type"> ������Ʈ Ÿ�� </param>
    /// <param name="pos"> ������Ʈ�� ��ġ�� ��ġ(���̾��Ű �θ� ������Ʈ) </param>
    /// <returns></returns>
    public T GetObject(T type, Transform pos)
    {
        if (type == null || pos == null) return null;

        T obj;

        if(poolQueue.Any())
        {
            obj = poolQueue.Dequeue();
            obj.gameObject.SetActive(true);
        }
        else
        {
            obj = Instantiate(type, pos);
        }

        return obj;
    }

    /// <summary>
    /// ���׸� ������Ʈ Ǯ �� ������Ʈ ��������
    /// </summary>
    /// <param name="type"> ������Ʈ Ÿ�� </param>
    /// <param name="pos"> ������Ʈ�� ��ġ�� ��ġ(���̾��Ű �θ� ������Ʈ) </param>
    /// <returns></returns>
    public T GetObjects(T type, Transform pos)
    {
        string name = type.name;
        T obj;

        if(!poolDic.ContainsKey(name))
        {
            poolDic.Add(name, new Queue<T>());
        }
        Queue<T> queue = poolDic[name];

        if(queue.Count <= 0)
        {
            obj = Instantiate(type, pos);
            obj.name = name;
            queue.Enqueue(obj);
        }

        obj = queue.Dequeue();
        obj.gameObject.SetActive(true);

        return obj;
    }

    /// <summary>
    /// ���׸� ������Ʈ Ǯ �� ������Ʈ ���
    /// </summary>
    /// <param name="value"> ������Ʈ </param>
    public void TakeObject(T value)
    {
        if (value == null) return;

        value.gameObject.SetActive(false);
        poolQueue.Enqueue(value);
    }

    /// <summary>
    /// ���׸� ������Ʈ Ǯ �� ������Ʈ ���
    /// </summary>
    /// <param name="value"> ������Ʈ </param>
    public void TakeObjects(T value)
    {
        if (value == null) return;

        string name = value.name;
        
        if(!poolDic.ContainsKey(name))
        {
            poolDic.Add(name, new Queue<T>());
        }

        value.gameObject.SetActive(false);
        poolDic[name].Enqueue(value);
    }
}
