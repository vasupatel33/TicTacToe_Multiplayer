using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonClick : MonoBehaviour
{
    [SerializeField] int no;
    private void OnMouseUp()
    {
        //Debug.Log("Clicked = "+this.gameObject.name);
    
        TileGrid.instance.UserMove(this.gameObject, no);
    }
}