using UnityEngine;
using UnityEngine.InputSystem;


// Unused
public class Controller
{
    public Gamepad controller;


    public Controller() {
        controller = Gamepad.current;

    }

    public float GetHorizontalInput() {

        return Input.GetAxis("Horizontal");
    }

    public float GetVerticalInput() {

        return Input.GetAxis("Vertical");
    }

}