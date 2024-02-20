using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject LoadingPanel, RoomPanel, CreateRoomPanel, PlayerListPanel, RoomListPanel, GameStartBtn;
    [SerializeField] TMP_InputField createInputText, joinInputText;
    [SerializeField] TextMeshProUGUI roomTitle;

    [SerializeField] GameObject PlayerNamePref, PlayerNameContent, RoomNamePref, RoomnNameContent;

    private void Start()
    {
        LoadingPanel.SetActive(true);
        PhotonNetwork.ConnectUsingSettings();
    }

   

    #region PHOTON CALL BACKS
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
        GameStartBtn.SetActive(PhotonNetwork.IsMasterClient);
        Debug.Log("Room Joined");
        PlayerListPanel.SetActive(true);
        CreateRoomPanel.SetActive(false);
        RoomListPanel.SetActive(false);
        PhotonNetwork.NickName = "Player" + Random.Range(0, 500);
        roomTitle.text = PhotonNetwork.CurrentRoom.Name;

        // Clear existing player list UI
        foreach (Transform child in PlayerNameContent.transform)
        {
            Destroy(child.gameObject);
        }

        // Get the updated player list
        Player[] players = PhotonNetwork.PlayerList;

        // Update player list UI for all players
        foreach (Player player in players)
        {
            GameObject g = Instantiate(PlayerNamePref, PlayerNameContent.transform);
            g.GetComponent<TextMeshProUGUI>().text = player.NickName;
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        // Update player list UI when a new player enters the room
        GameObject g = Instantiate(PlayerNamePref, PlayerNameContent.transform);
        g.GetComponent<TextMeshProUGUI>().text = newPlayer.NickName;
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        // Update player list UI when a player leaves the room
        foreach (Transform child in PlayerNameContent.transform)
        {
            if (child.GetComponent<TextMeshProUGUI>().text == otherPlayer.NickName)
            {
                Destroy(child.gameObject);
                break;
            }
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        for (int i = 0; i < RoomnNameContent.transform.childCount; i++)
        {
            Destroy(RoomnNameContent.transform.GetChild(i).gameObject);
        }

        for (int i = 0; i < roomList.Count; i++)
        {
            Debug.Log("Room updateddddd");
            GameObject g = Instantiate(RoomNamePref, RoomnNameContent.transform);
            g.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = roomList[i].Name;

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
                
                joinButton.interactable = true;

                joinButton.onClick.RemoveAllListeners();
                joinButton.onClick.AddListener(() => OnJoinButtonClicked(joinButton.transform.parent.GetChild(0).GetComponent<TextMeshProUGUI>().text));
                Debug.Log("Btn clicked = " + joinButton.transform.parent.GetChild(0).GetComponent<TextMeshProUGUI>().text);
            }
            else
            {
                Transform joinButtonTransform = g.transform.GetChild(3);
                Button joinButton = joinButtonTransform.GetComponent<Button>();
                joinButton.interactable = false;
            }
        }
    }
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        PhotonNetwork.SetMasterClient(newMasterClient);
    }

    public override void OnLeftRoom()
    {
        RoomPanel.SetActive(true);
        PlayerListPanel.SetActive(false);
        RoomListPanel.SetActive(false);
    }

    #endregion

    #region PHOTON BUTTON CLICK METHODS
    public void CreateRoom()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 5;
        roomOptions.IsVisible = true;
        PhotonNetwork.CreateRoom(createInputText.text, roomOptions, TypedLobby.Default);
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
    public void OnBtnClickRoomLeft()
    {
        PhotonNetwork.LeaveRoom();
    }
    #endregion
    void OnJoinButtonClicked(string roomName)
    {
        // Code to join the room with the given roomName
        Debug.Log("Joining room: " + roomName);
        PhotonNetwork.JoinRoom(roomName); // Assuming you're using Photon for multiplayer functionality
    }

}

