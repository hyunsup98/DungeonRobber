using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] float startTime;
    [SerializeField] float dangerousTime;
    [SerializeField] float currentTime;
    [SerializeField] float timeMultiplier = 60f; //시간 배율 

    void OnEnable()
    {
        Init();
    }

    void Update()
    {
       
    }
    void Init()
    {
        currentTime = startTime;
    }
}



