using System;
using UnityEngine;

public class WeaponScript : MonoBehaviour
{
    public PlayerMovement playerMovement;
    private bool isFacingRight = true;

    void FixedUpdate()
    {
        if (Math.Sign(playerMovement.MoveValue) == 1 && !isFacingRight)
        {
            isFacingRight = true;
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (Math.Sign(playerMovement.MoveValue) == -1 && isFacingRight)
        {
            isFacingRight = false;
            transform.localScale = new Vector3(1, 1, 1);
        }
    }
}
