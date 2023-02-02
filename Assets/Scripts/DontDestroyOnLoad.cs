using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoad<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance;


    virtual protected void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        if (Instance == null)
        {
            Instance = (T)FindObjectOfType(typeof(T));
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
