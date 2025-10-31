using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    private GameObject[] _ObjectArray = new GameObject[0];
    private int ArrayCount;

    private void ArrayObject(int index)
    {
        index = ArrayCount;
        _ObjectArray[index] = null;
    }


}
