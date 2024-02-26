using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class ClickAction : MonoBehaviourPunCallbacks
{
    RoomInfo room;
    public void setup(RoomInfo info)
    {
        room = info;
    }
    public void buttonClickAc()
    {
        GameManager.instance.OnJoinButtonClicked(room);
    }
}
