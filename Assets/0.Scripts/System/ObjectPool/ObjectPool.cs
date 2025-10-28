using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public abstract class ObjectPool<T> : Singleton<ObjectPool<T>> where T : MonoBehaviour
{
    //한 가지 타입만 있으면 poolQueue를 통해 관리
    protected Queue<T> poolQueue = new Queue<T>();

    //몬스터, 아이템 등 같은 상황에 스폰하지만 여러 종류가 있는 경우 poolDic을 통해 관리
    protected Dictionary<string, Queue<T>> poolDic = new Dictionary<string, Queue<T>>();

    /// <summary>
    /// 제네릭 오브젝트 풀 → 오브젝트 꺼내오기
    /// </summary>
    /// <param name="type"> 오브젝트 타입 </param>
    /// <param name="pos"> 오브젝트를 배치할 위치(하이어라키 부모 오브젝트) </param>
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
    /// 제네릭 오브젝트 풀 → 오브젝트 꺼내오기
    /// </summary>
    /// <param name="type"> 오브젝트 타입 </param>
    /// <param name="pos"> 오브젝트를 배치할 위치(하이어라키 부모 오브젝트) </param>
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
    /// 제네릭 오브젝트 풀 → 오브젝트 담기
    /// </summary>
    /// <param name="value"> 오브젝트 </param>
    public void TakeObject(T value)
    {
        if (value == null) return;

        value.gameObject.SetActive(false);
        poolQueue.Enqueue(value);
    }

    /// <summary>
    /// 제네릭 오브젝트 풀 → 오브젝트 담기
    /// </summary>
    /// <param name="value"> 오브젝트 </param>
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
