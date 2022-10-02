using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Pluck[,] field = new Pluck[7, 6];
    private int[] columnFillCounter = new int[7];
    [SerializeField] private GameObject fieldBlock;
    [SerializeField] private GameObject pluck;
    [SerializeField] private GameObject pluckHolder;

    private Pluck.PluckOwner currentPlayer = Pluck.PluckOwner.Player1;

    [SerializeField] private Text playerTurnText;
    [SerializeField] private GameObject endGamePanel;

    private bool isWinner;

    // Start is called before the first frame update
    void Start()
    {
        isWinner = false;
        //DrawField();
        ResetColumnCounter();
        currentPlayer = Pluck.PluckOwner.Player1;
        playerTurnText.text = "Player 1";

    }

    private void DrawField()
    {
        for (int y = 0; y < field.GetLength(1); y++)
        {
            for (int x = 0; x < field.GetLength(0); x++)
            {
                Instantiate(fieldBlock, new Vector3(x, y, 0), Quaternion.identity);
            }
        }
    }

    public void InsertPluck(int x)
    {
        if(columnFillCounter[x] < 6)
        {
            int y = columnFillCounter[x];
            Pluck _pluck =  Instantiate(pluck, new Vector3(x, y, 0), Quaternion.identity).GetComponent<Pluck>();
            _pluck.x = x;
            _pluck.y = y;
            _pluck.SetOwner(currentPlayer);
            _pluck.transform.parent = pluckHolder.transform;

            field[x, y] = _pluck;

            CheckForWinner(x, y);
            columnFillCounter[x]++;

            if (columnFillCounter[x] > 4)
                CheckForDraw();

            if(!isWinner)
                changeCurrentPlayer();
            
        }
    }
    public void ResetGame()
    {
        Array.Clear(field, 0, field.Length);
        foreach (Transform child in pluckHolder.transform)
            Destroy(child.gameObject);
        ResetColumnCounter();
        currentPlayer = Pluck.PluckOwner.Player1;
        playerTurnText.text = "Player 1";
        isWinner = false;
        endGamePanel.SetActive(false);
    }

    private void ResetColumnCounter()
    {
        for(int i = 0; i < columnFillCounter.Length; i++)
        {
            columnFillCounter[i] = 0;
        }
    }
    private void ChangePlayerTurnText()
    {
        if (currentPlayer == Pluck.PluckOwner.Player1)
        {
            playerTurnText.text = "Player 1";
            playerTurnText.color = Color.red;
        }
        else
        {
            playerTurnText.text = "Player 2";
            playerTurnText.color = Color.yellow;
        }
    }
    private void changeCurrentPlayer()
    {
        if (currentPlayer == Pluck.PluckOwner.Player1)
        {
            currentPlayer = Pluck.PluckOwner.Player2;
        }
        else
        {
            currentPlayer = Pluck.PluckOwner.Player1;
        }
        ChangePlayerTurnText();
    }
    private void CheckForWinner(int x, int y)
    {
        List<Pluck>[] axes = new List<Pluck>[4];
        Pluck _currentPluck = field[x, y].GetComponent<Pluck>();

        //List<Pluck> horizontal; (-1, 0) && (1, 0)  - 0
        //List<Pluck> vertical;   (0, -1) && (0, 1)  - 1
        //List<Pluck> ascending;  (-1, -1) && (1, 1) - 2
        //List<Pluck> descending; (-1, 1) && (1, -1) - 3


        for (var i = -1; i < 2; i++) // loops through neigbours (y axis)
        {
            for (var j = -1; j < 2; j++)  // loops through neigbours (x axis)
            {
                //Debug.Log("j: " + j + " ; i: " + i);
                if ((i == 0 && j == 0) || (x + j) < 0 || (x + j) > field.GetLength(0) - 1 || (y + i) < 0 || (y + i) > field.GetLength(1) - 1) //excludes _currentPluck and indexes outside array bounds
                {
                    continue;
                }

                if (isWinner)
                {
                    endGamePanel.SetActive(true);
                    playerTurnText.text = " ";
                    if (currentPlayer == Pluck.PluckOwner.Player1)
                        endGamePanel.GetComponentInChildren<Text>().text = "Player 1 wins";
                    else
                        endGamePanel.GetComponentInChildren<Text>().text = "Player 2 wins";
                    break;
                }

                if (field[x + j, y + i] != null && field[x + j, y + i].GetComponent<Pluck>().owner == currentPlayer) //check if neighbour belongs to current player
                {
                    Pluck _neighbourPluck = field[x + j, y + i].GetComponent<Pluck>();
                    switch ((j, i))
                    {
                        case (-1, 0):
                        case (1, 0):
                            Debug.Log("Horizontal");
                            if (axes[0] == null)
                                axes[0] = new List<Pluck>();
                            axes[0].Add(_currentPluck);
                            CheckFurtherNeighbours(j, i, _currentPluck, axes[0]);
                            break;

                        case (0, -1):
                        case (0, 1):
                            Debug.Log("Vertical");
                            if (axes[1] == null)
                                axes[1] = new List<Pluck>();
                            axes[1].Add(_currentPluck);
                            CheckFurtherNeighbours(j, i, _currentPluck, axes[1]);
                            break;

                        case (-1, -1):
                        case (1, 1):
                            Debug.Log("Ascending");
                            if (axes[2] == null)
                                axes[2] = new List<Pluck>();
                            axes[2].Add(_currentPluck);
                            CheckFurtherNeighbours(j, i, _currentPluck, axes[2]);
                            break;

                        case (-1, 1):
                        case (1, -1):
                            Debug.Log("Descending");
                            if (axes[3] == null)
                                axes[3] = new List<Pluck>();
                            axes[3].Add(_currentPluck);
                            CheckFurtherNeighbours(j, i, _currentPluck, axes[3]);
                            break;
                    }
                }
            }
        }

        void CheckFurtherNeighbours(int xDir, int yDir, Pluck _curPluck,List<Pluck> axis)
        {
            int xCur = _curPluck.x;
            int yCur = _curPluck.y;

            bool end_Positive = false;
            bool end_Negative = false;

            for (int l = 1; l < 4; l++)
            {
                int x_Positive = xCur + (l * xDir);
                int x_Negative = xCur - (l * xDir);

                int y_Positive = yCur + (l * yDir);
                int y_Negative = yCur - (l * yDir);

                //Positive direction
                if ((x_Positive < field.GetLength(0) && x_Positive > - 1) && (y_Positive < field.GetLength(1) && y_Positive > -1))
                {
                    if (field[x_Positive,y_Positive] != null && field[x_Positive, y_Positive].owner == currentPlayer && !end_Positive)
                    {
                        axis.Add(field[x_Positive, y_Positive]);
                    }
                    else
                    {
                        end_Positive = true;
                    }
                }

                //Negative direction
                if ((x_Negative < field.GetLength(0) && x_Negative > -1) && (y_Negative < field.GetLength(1) && y_Negative > -1))
                {
                    if (field[x_Negative, y_Negative] != null && field[x_Negative, y_Negative].owner == currentPlayer && !end_Negative)
                    {
                        axis.Add(field[x_Negative, y_Negative]);
                    }
                    else
                    {
                        end_Negative = true;
                    }
                }

                if (axis.Count == 4)
                {
                    Debug.Log("Winner is: " + currentPlayer);
                    isWinner = true;    
                    break;
                }
            }
        }

    }

    private void CheckForDraw()
    {
        int filledColumns = 0;
        for(int i = 0; i < columnFillCounter.Length; i++)
        {
            if (columnFillCounter[i] == 6)
                filledColumns++;
        }
        if(filledColumns == 7)
        {
            isWinner=true; 
            endGamePanel.SetActive(true);
            playerTurnText.text = " ";
            endGamePanel.GetComponentInChildren<Text>().text = "DRAW";
        }
    }

}
