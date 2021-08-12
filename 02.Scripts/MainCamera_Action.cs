using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera_Action : MonoBehaviour
{
    public GameObject player;
    Vector3 cameraPosition;

    public float setX = 0f;
    public float setY = 25f;
    public float setZ = -35f;

    // Start is called before the first frame update
    void Start()
    {
        
    }
    // Update is called once per frame
    void LateUpdate()
    {
        cameraPosition.x = player.transform.position.x + setX;
        cameraPosition.y = player.transform.position.y + setY;
        cameraPosition.z = player.transform.position.z + setZ;

        transform.position = cameraPosition;
    }
}
