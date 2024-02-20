using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject LoadingPanel, RoomPanel, CreateRoomPanel, PlayerListPanel, RoomListPanel;
    [SerializeField] TMP_InputField createInputText, joinInputText;
    [SerializeField] TextMeshProUGUI roomTitle;

    [SerializeField] GameObject PlayerNamePref, PlayerNameContent, RoomNamePref, RoomnNameContent;

    private void Start()
    {
        LoadingPanel.SetActive(true);
        PhotonNetwork.ConnectUsingSettings();
    }

    public void CreateRoom()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 5;
        roomOptions.IsVisible = true;
        PhotonNetwork.CreateRoom(createInputText.text, roomOptions, TypedLobby.Default);
    }
    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(joinInputText.text);
    }
    public override void OnConnected()
    {
        Debug.Log("Photon connected");
    }
    public override void OnConnectedToMaster()
    {
        Debug.Log("Master connected");
        PhotonNetwork.JoinLobby();
    }
    public override void OnCreatedRoom()
    {
        Debug.Log("Room created successfully");
        PlayerListPanel.SetActive(true);
        CreateRoomPanel.SetActive(false);
        
    }
    public override void OnJoinedLobby()
    {
        Debug.Log("Lobby joined");
        RoomPanel.SetActive(true);
        LoadingPanel.SetActive(false);
    }
    public override void OnJoinedRoom()
    {
        Debug.Log("Room Joined");
        PlayerListPanel.SetActive(true);
        CreateRoomPanel.SetActive(false);
        RoomListPanel.SetActive(false);
        PhotonNetwork.NickName = "Player" + Random.Range(0, 500);
        roomTitle.text = PhotonNetwork.CurrentRoom.Name;

        Player[] player = PhotonNetwork.PlayerList;

        for (int i = 0; i < player.Length; i++)
        {
            GameObject g = Instantiate(PlayerNamePref, PlayerNameContent.transform);
            g.GetComponent<TextMeshProUGUI>().text = player[i].NickName;
        }
    }

    public void OnBtnClickCreateRoom()
    {
        CreateRoomPanel.SetActive(true);
        RoomPanel.SetActive(false);
           LoadingPanel.SetActive(false);
    }
    public void OnBtnClickJoinRoom()
    {
        RoomListPanel.SetActive(true);
        LoadingPanel.SetActive(false);
        RoomPanel.SetActive(false);
        CreateRoomPanel.SetActive(false);
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        for (int i = 0; i < roomList.Count; i++)
        {
            Debug.Log("Room updateddddd");
            GameObject g = Instantiate(RoomNamePref, RoomnNameContent.transform);
            g.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = roomList[i].Name;

            // Check if the room has player count and max player count information
            if (roomList[i].PlayerCount > -1 && roomList[i].MaxPlayers > 0)
            {
                string playerInfo = $"{roomList[i].PlayerCount}/{roomList[i].MaxPlayers}";
                g.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = playerInfo;
            }

            // Check if the room exists
            if (roomList[i].PlayerCount > 0)
            {
                Transform joinButtonTransform = g.transform.GetChild(3);
                Button joinButton = joinButtonTransform.GetComponent<Button>();
                Debug.Log(joinButton.name);
                // Enable join button
                joinButton.interactable = true;

                // Add click listener to join the room
                joinButton.onClick.RemoveAllListeners();
                joinButton.onClick.AddListener(() => OnJoinButtonClicked(joinButton.transform.parent.GetChild(0).GetComponent<TextMeshProUGUI>().text));
                Debug.Log("Btn clicked = "+ joinButton.transform.parent.GetChild(0).GetComponent<TextMeshProUGUI>().text);
            }
            else
            {
                // Room does not exist, disable join button
                Transform joinButtonTransform = g.transform.GetChild(2);
                Button joinButton = joinButtonTransform.GetComponent<Button>();
                joinButton.interactable = false;
            }
        }
    }
    void OnJoinButtonClicked(string roomName)
    {
        // Code to join the room with the given roomName
        Debug.Log("Joining room: " + roomName);
        PhotonNetwork.JoinRoom(roomName); // Assuming you're using Photon for multiplayer functionality
    }

}
