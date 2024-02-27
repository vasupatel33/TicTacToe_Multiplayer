using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TileGrid : MonoBehaviourPunCallbacks
{
    public enum PlayerMode
    {
        empty,
        zero,
        cross
    }

    [SerializeField] GameObject GameOverPanel, Parent;
    [SerializeField] PlayerMode[] AllplayerMode;
    [SerializeField] PlayerMode playerMode;
    [SerializeField] Sprite ZeroSprite, CrossSprite;
    public static TileGrid instance;
    private static bool isPlayerTurn = true; // Flag to track whose turn it is

    private void Awake()
    {
        instance = this;
    }
    //public void UserMove(GameObject g, int no)
    //{
    //    if (!isPlayerTurn) // Check if it's not the player's turn
    //    {


    //        if (playerMode == PlayerMode.zero)
    //        {
    //            g.GetComponent<SpriteRenderer>().sprite = CrossSprite;
    //            g.GetComponent<BoxCollider2D>().enabled = false;
    //            AllplayerMode[no] = PlayerMode.cross;
    //            playerMode = PlayerMode.cross; // Update playerMode to cross before RPC call

    //            photonView.RPC("SyncMove", RpcTarget.Others, no, (int)PlayerMode.cross);
    //        }
    //        else
    //        {
    //            g.GetComponent<SpriteRenderer>().sprite = ZeroSprite;
    //            g.GetComponent<BoxCollider2D>().enabled = false;
    //            AllplayerMode[no] = PlayerMode.zero;
    //            playerMode = PlayerMode.zero; // Update playerMode to zero before RPC call

    //            photonView.RPC("SyncMove", RpcTarget.Others, no, (int)PlayerMode.zero);
    //        }
    //        CheckWinOrNot();
    //    }
    //    isPlayerTurn = !isPlayerTurn; // Toggle isPlayerTurn to switch to the other player's turn
    //}
    private void Update()
    {
        Debug.Log("Turn = "+isPlayerTurn);
    }
    public void UserMove(GameObject g, int no)
    {
        if (!isPlayerTurn) // Check if it's not the player's turn
        {
            isPlayerTurn = true;
            Debug.Log("IF called");
            g.GetComponent<SpriteRenderer>().sprite = CrossSprite;
            g.GetComponent<BoxCollider2D>().enabled = false;
            AllplayerMode[no] = PlayerMode.cross;
            playerMode = PlayerMode.cross; // Update playerMode to cross before RPC call

            photonView.RPC("SyncMove", RpcTarget.Others, no, (int)PlayerMode.cross);
            CheckWinOrNot();
        }
        else
        {
            isPlayerTurn = false;
            Debug.Log("Else called");
            g.GetComponent<SpriteRenderer>().sprite = ZeroSprite;
            g.GetComponent<BoxCollider2D>().enabled = false;
            AllplayerMode[no] = PlayerMode.zero;
            playerMode = PlayerMode.zero; // Update playerMode to zero before RPC call

            photonView.RPC("SyncMove", RpcTarget.Others, no, (int)PlayerMode.zero);
            CheckWinOrNot();
        }
    }

    [PunRPC]
    private void SyncMove(int index, int mode)
    {
        AllplayerMode[index] = (PlayerMode)mode;
        GameObject tile = Parent.transform.GetChild(index).gameObject;
        tile.GetComponent<BoxCollider2D>().enabled = false;

        if ((PlayerMode)mode == PlayerMode.cross)
        {
            tile.GetComponent<SpriteRenderer>().sprite = CrossSprite;
        }
        else if ((PlayerMode)mode == PlayerMode.zero)
        {
            tile.GetComponent<SpriteRenderer>().sprite = ZeroSprite;
        }

        CheckWinOrNot();

        isPlayerTurn = true; // Switch back to the first player's turn
    }
    public void CheckWinOrNot()
    {
        int[,] data = new int[8, 3] { { 0, 1, 2 }, { 3, 4, 5 }, { 6, 7, 8 }, { 0, 3, 6 }, { 1, 4, 7 }, { 2, 5, 8 }, { 0, 4, 8 }, { 2, 4, 6 } };
        for (int i = 0; i < 8; i++)
        {
            if ((AllplayerMode[data[i, 0]] == PlayerMode.zero && AllplayerMode[data[i, 1]] == PlayerMode.zero && AllplayerMode[data[i, 2]] == PlayerMode.zero) ||
                (AllplayerMode[data[i, 0]] == PlayerMode.cross && AllplayerMode[data[i, 1]] == PlayerMode.cross && AllplayerMode[data[i, 2]] == PlayerMode.cross))
            {
                Debug.Log("Player win = " + playerMode + "...." + i);
                GameOverPanel.SetActive(true);
            }
        }
    }
    public void OnClick_HomeBtn()
    {
        photonView.RPC("SyncPerformAction", RpcTarget.All, (int)ActionType.LoadMainMenu);
    }
    public void OnClick_RetryBtn()
    {
        photonView.RPC("SyncPerformAction", RpcTarget.All, (int)ActionType.ReloadScene);
    }
    [PunRPC]
    private void SyncPerformAction(int actionType)
    {
        switch ((ActionType)actionType)
        {
            case ActionType.LoadMainMenu:
                SceneManager.LoadScene(0);
                break;
            case ActionType.ReloadScene:
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                break;
                // Add cases for other action types if needed
        }
    }
    public enum ActionType
    {
        ReloadScene,
        LoadMainMenu
        // Add more action types as needed
    }
}
