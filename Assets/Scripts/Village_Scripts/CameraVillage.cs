using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraVillage : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    public GameObject player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        target = player.transform;
    }
    // Update is called once per frame
    void Update()
    {
        transform.position = target.position + offset;
    }
}
