using UnityEngine;

/// <summary>
/// 가장 하위의 파생 클래스에서 SingletonInit() 메서드를 통해 싱글턴 초기 세팅 수행
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    private static object _lock = new object();
    private static bool applicationQuit = false;

    public static T Instance
    {
        get
        {
            if(applicationQuit)
            {
                return null;
            }

            lock(_lock)
            {
                if (instance == null)
                {
                    instance = FindAnyObjectByType<T>();

                    if (instance == null)
                    {
                        GameObject obj = new GameObject(typeof(T).Name, typeof(T));
                        instance = obj.AddComponent<T>();
                    }
                }
                return instance;
            }
        }
    }

    protected void SingletonInit()
    {
        if (instance == null)
        {
            instance = this as T;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        if (transform.parent != null && transform.root != null)
        {
            DontDestroyOnLoad(transform.root.gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    protected virtual void OnApplicationQuit()
    {
        applicationQuit = true;
    }

    protected virtual void OnDestroy()
    {
        applicationQuit = true;
    }

}
