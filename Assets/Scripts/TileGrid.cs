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
    [SerializeField] Sprite ZeroSprite, CrossSprite;
    public static TileGrid instance;
    private PlayerMode currentPlayerMode = PlayerMode.zero; // Current player's mode
    private bool isGameOver = false;

    private void Awake()
    {
        instance = this;
    }

    public void UserMove(GameObject g, int no)
    {
        if (isGameOver || !photonView.IsMine) // Check if the game is over or if it's not the local player's turn
            return;

        PlayerMode modeToSet = currentPlayerMode == PlayerMode.zero ? PlayerMode.cross : PlayerMode.zero;
        SetTileState(g, no, modeToSet); // Set the tile state for the local player

        photonView.RPC("SyncMove", RpcTarget.Others, no, (int)modeToSet); // Sync the move with other players
        CheckWinOrNot(); // Check if the game is won after the move
    }

    [PunRPC]
    private void SyncMove(int index, int mode)
    {
        PlayerMode modeToSet = (PlayerMode)mode;
        GameObject tile = Parent.transform.GetChild(index).gameObject;
        SetTileState(tile, index, modeToSet); // Set the tile state for other players
        CheckWinOrNot(); // Check if the game is won after the move
    }

    private void SetTileState(GameObject tile, int index, PlayerMode mode)
    {
        AllplayerMode[index] = mode;
        tile.GetComponent<BoxCollider2D>().enabled = false;

        if (mode == PlayerMode.cross)
        {
            tile.GetComponent<SpriteRenderer>().sprite = CrossSprite;
        }
        else if (mode == PlayerMode.zero)
        {
            tile.GetComponent<SpriteRenderer>().sprite = ZeroSprite;
        }
    }

    private void CheckWinOrNot()
    {
        // Implement win condition checking
        // Set isGameOver to true if the game is won
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
