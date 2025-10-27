using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // wasd move
        float moveX = Input.GetAxis("Horizontal") * Time.deltaTime * 5f;
        float moveZ = Input.GetAxis("Vertical") * Time.deltaTime * 5f;
        transform.Translate(new Vector3(moveX, 0, moveZ));

        Debug.Log("Player Position: " + transform.position);
    }
}
