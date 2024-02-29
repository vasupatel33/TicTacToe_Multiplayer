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

    [SerializeField] GameObject GameOverPanel, Parent, FirstPlayerPanel;
    [SerializeField] PlayerMode[] AllplayerMode;
    [SerializeField] PlayerMode playerMode;
    [SerializeField] Sprite ZeroSprite, CrossSprite;
    public static TileGrid instance;
    private bool isPlayerTurn = true;
    int MoveCount=0;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
      
    }
    private void Update()
    {
        Debug.Log("Turn = "+isPlayerTurn);
    }
    public void UserMove(GameObject g, int no)
    {
        MoveCount++;
        if (!isPlayerTurn)
        {
            Debug.Log("Player's turn: Cross");
            PerformMove(g, no, PlayerMode.cross);
         //   FirstPlayerPanel.SetActive(true);
        }
        else
        {
            Debug.Log("Player's turn: Zero");
            PerformMove(g, no, PlayerMode.zero);
          //  FirstPlayerPanel.SetActive(true);

        }
    }
    private void PerformMove(GameObject g, int index, PlayerMode mode)
    {
        g.GetComponent<SpriteRenderer>().sprite = mode == PlayerMode.cross ? CrossSprite : ZeroSprite;
        g.GetComponent<BoxCollider2D>().enabled = false;
        AllplayerMode[index] = mode;
        playerMode = mode; // Update playerMode

        photonView.RPC("SyncMove", RpcTarget.AllBuffered, index, (int)mode);
        CheckWinOrNot();
    }

    [PunRPC]
    private void SyncMove(int index, int mode)
    {
        isPlayerTurn = !isPlayerTurn;
        Debug.Log("Player switched = " + isPlayerTurn);

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
        if (isPlayerTurn) // If it's now the current player's turn
        {
            if (photonView.IsMine) // If it's the local player's turn
            {
                Debug.Log("It's my turn. Showing panel...");
                FirstPlayerPanel.SetActive(false); // Show panel for the local player
            }
            else
            {
                Debug.Log("It's opponent's turn. Hiding panel...");
                FirstPlayerPanel.SetActive(true); // Hide panel for the opponent
            }
        }
        else // If it's not the current player's turn
        {
            if (photonView.IsMine) // If it's the local player's turn
            {
                Debug.Log("It's my opponent's turn. Hiding panel...");
                FirstPlayerPanel.SetActive(true); // Hide panel for the local player
            }
            else
            {
                Debug.Log("It's my turn. Showing panel...");
                FirstPlayerPanel.SetActive(false); // Show panel for the opponent
            }
        }

        CheckWinOrNot();
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
        if (MoveCount == 9)
        {
            MoveCount = 0;
            GameOverPanel.SetActive(true);
            Debug.Log("No move available");
        }
        else
        {
            Debug.Log("Moves available = "+MoveCount);
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
