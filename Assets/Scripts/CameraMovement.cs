using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float speed = 15f;
        transform.position += new Vector3(Input.GetAxisRaw("Mouse X") * Time.deltaTime * speed, Input.GetAxisRaw("Mouse Y") * Time.deltaTime * speed, 0f);
    }
}
