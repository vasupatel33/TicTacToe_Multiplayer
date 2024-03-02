using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TileGrid : MonoBehaviour
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
    [SerializeField] TextMeshProUGUI TitleGameOverPanel;
    public static TileGrid instance;

    int MoveCount;

    private void Awake()
    {
        instance = this;
    }
    public void UserMove(GameObject g, int no)
    {
        if (AllplayerMode[no] == PlayerMode.empty) // Check if the position is empty
        {
            PerformMove(g, no, PlayerMode.cross); // Player's move
            MoveCount++; // Increment move count after player's move
            Debug.Log("Current count = " + MoveCount); // Log the current move count

            if (!CheckGameOver(AllplayerMode)) // Proceed only if the game is not over
            {
                int bestMove = Minimax(AllplayerMode, PlayerMode.zero, 0); // Start Minimax with depth 0

                // Check if bestMove is valid
                if (bestMove >= 0 && bestMove < Parent.transform.childCount)
                {
                    GameObject computerTile = Parent.transform.GetChild(bestMove).gameObject;
                    PerformMove(computerTile, bestMove, PlayerMode.zero); // Computer's move
                    MoveCount++; // Increment move count after computer's move
                    Debug.Log("Current count = " + MoveCount); // Log the current move count
                    CheckGameOver(AllplayerMode); // Check game over after computer's move
                }
                else if (bestMove == -1)
                {
                    Debug.LogWarning("No valid moves left!"); // Handle the case when no valid moves are left
                }
                else
                {
                    Debug.LogError("Invalid bestMove index: " + bestMove);
                }
            }
        }
    }


    private void PerformMove(GameObject g, int index, PlayerMode mode)
    {
        g.GetComponent<SpriteRenderer>().sprite = mode == PlayerMode.cross ? CrossSprite : ZeroSprite;
        g.GetComponent<BoxCollider2D>().enabled = false;
        AllplayerMode[index] = mode;
    }
    private int Minimax(PlayerMode[] board, PlayerMode player, int depth)
    {
        const int MAX_DEPTH = 4; // Adjust the depth limit for the Minimax algorithm

        int[,] winConditions = new int[8, 3] { { 0, 1, 2 }, { 3, 4, 5 }, { 6, 7, 8 }, { 0, 3, 6 }, { 1, 4, 7 }, { 2, 5, 8 }, { 0, 4, 8 }, { 2, 4, 6 } };

        int bestScore;
        int bestMove = -1;

        // Check if the game is over or if the depth limit is reached
        if (CheckGameOver(board) || depth == MAX_DEPTH)
        {
            int evaluation = Evaluate(board);
            Debug.Log($"Evaluation at depth {depth}: {evaluation}");
            return evaluation;
        }

        if (player == PlayerMode.zero) // Computer's turn
        {
            bestScore = int.MinValue;
            for (int i = 0; i < board.Length; i++)
            {
                if (board[i] == PlayerMode.empty) // If the position is empty
                {
                    board[i] = PlayerMode.zero;
                    int score = Minimax(board, PlayerMode.cross, depth + 1); // Switch player and increment depth
                    board[i] = PlayerMode.empty; // Undo move
                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestMove = i;
                    }
                }
            }
        }
        else // Player's turn
        {
            bestScore = int.MaxValue;
            for (int i = 0; i < board.Length; i++)
            {
                if (board[i] == PlayerMode.empty) // If the position is empty
                {
                    board[i] = PlayerMode.cross;
                    int score = Minimax(board, PlayerMode.zero, depth + 1); // Switch player and increment depth
                    board[i] = PlayerMode.empty; // Undo move
                    if (score < bestScore)
                    {
                        bestScore = score;
                        bestMove = i;
                    }
                }
            }
        }

        if (depth == 0) // Return the best move at the top level of Minimax
        {
            Debug.Log($"At depth {depth}: Player {player} evaluated score: {bestScore}");

            return bestMove;
        }

        return bestScore;
    }

    private int Evaluate(PlayerMode[] board)
    {
        int score = 0;

        // Check for win or loss
        for (int i = 0; i < winConditions.GetLength(0); i++)
        {
            int a = winConditions[i, 0];
            int b = winConditions[i, 1];
            int c = winConditions[i, 2];

            if (board[a] == PlayerMode.zero && board[b] == PlayerMode.zero && board[c] == PlayerMode.zero)
            {
                Debug.Log("Computer wins!");
                return int.MaxValue; // Computer wins
            }
            else if (board[a] == PlayerMode.cross && board[b] == PlayerMode.cross && board[c] == PlayerMode.cross)
            {
                Debug.Log("Player wins!");
                return int.MinValue; // Player wins
            }
        }

        // Evaluate based on positions
        score += EvaluatePosition(board, PlayerMode.zero); // Computer's positions
        score -= EvaluatePosition(board, PlayerMode.cross); // Player's positions

        Debug.Log("Final score: " + score);

        return score;
    }

    private int EvaluatePosition(PlayerMode[] board, PlayerMode player)
    {
        int score = 0;

        // Evaluate based on possible winning lines
        for (int i = 0; i < winConditions.GetLength(0); i++)
        {
            int countPlayer = 0;
            int countEmpty = 0;

            for (int j = 0; j < winConditions.GetLength(1); j++)
            {
                int index = winConditions[i, j];
                if (board[index] == player)
                {
                    countPlayer++;
                }
                else if (board[index] == PlayerMode.empty)
                {
                    countEmpty++;
                }
            }

            // Assign scores based on the number of player's pieces and empty spaces in winning lines
            if (countPlayer == 2 && countEmpty == 1)
            {
                score += 10; // Two in a row with one empty space
            }
            else if (countPlayer == 1 && countEmpty == 2)
            {
                score += 5; // One in a row with two empty spaces
            }
        }

        Debug.Log($"Evaluation for {player}: {score}");

        return score;
    }


    int[,] winConditions = new int[8, 3] { { 0, 1, 2 }, { 3, 4, 5 }, { 6, 7, 8 }, { 0, 3, 6 }, { 1, 4, 7 }, { 2, 5, 8 }, { 0, 4, 8 }, { 2, 4, 6 } };
    private bool CheckGameOver(PlayerMode[] board)
    {
        // Remove MoveCount increment from this method as it should be handled in the UserMove method

        for (int i = 0; i < winConditions.GetLength(0); i++)
        {
            int a = winConditions[i, 0];
            int b = winConditions[i, 1];
            int c = winConditions[i, 2];

            if (board[a] != PlayerMode.empty &&
                board[a] == board[b] &&
                board[b] == board[c])
            {
                // Log the winning player
                Debug.Log("Player " + board[a] + " wins!");

                return true;
            }
        }

        // Check for draw
        bool isDraw = true;
        foreach (PlayerMode mode in board)
        {
            if (mode == PlayerMode.empty)
            {
                isDraw = false;
                break;
            }
        }

        if (isDraw && MoveCount == 9)
        {
            MoveCount = 0;
            GameOverPanel.SetActive(true);
            TitleGameOverPanel.text = "Game Drawn Well Played!";
            Debug.Log("Game drawn");
            return true;
        }

        return false;
    }

    public void OnClick_HomeBtn()
    {
        SceneManager.LoadScene(0);
    }
    public void OnClick_RetryBtn()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
