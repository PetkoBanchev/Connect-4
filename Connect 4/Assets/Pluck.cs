using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pluck : MonoBehaviour
{
    public int x;
    public int y;

    public enum PluckOwner
    {
        Player1,
        Player2
    }

    public PluckOwner owner;

    public void SetOwner(PluckOwner player)
    {
        owner = player;
        ChangeColor();
    }
    private void ChangeColor()
    {
        if(owner == PluckOwner.Player1)
        {
            transform.GetComponent<SpriteRenderer>().color = Color.red;
        }
        else
        {
            transform.GetComponent<SpriteRenderer>().color = Color.yellow;
        }
    }
}
