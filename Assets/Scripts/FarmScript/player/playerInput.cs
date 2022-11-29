using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerInput : MonoBehaviour
{
    #region Variable
    private Vector2 _movement = Vector2.zero;
    private float mouseX;
    private float mouseY;
    #endregion
    #region propriétés
    public Vector2 Movement => _movement;
    public float MouseX => mouseX;
    public float MouseY => mouseY;
    // Start is called before the first frame update
    #endregion
    #region builtin method
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //defini les axes sur les quel on vas bouger et leur touches
        _movement.Set(Input.GetAxis("Horizontal"),Input.GetAxis("Vertical"));
        mouseX=Input.GetAxis("Mouse X");
        mouseY=Input.GetAxis("Mouse Y");
    }
    #endregion
}   
