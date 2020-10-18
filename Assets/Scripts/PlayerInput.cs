using System;
using BustaGames.Joystick;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public Joystick joystick;
    public ShipController shipController;

    private void Update()
    {
        var direction = joystick.Direction;
        
        if (direction != Vector3.zero)
        {
            shipController.Move(direction);
        }
        
        if (joystick.IsDown)
        {
            shipController.Shoot();
        }
    }
}
