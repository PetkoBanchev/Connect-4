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

    private Pluck.PluckOwner currentPlayer = Pluck.PluckOwner.Player1;

    [SerializeField] private Text playerTurnText; 
    // Start is called before the first frame update
    void Start()
    {
        //DrawField();
        ResetColumnCounter();
    }

    // Update is called once per frame
    void Update()
    {
        
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
            field[x,y] = Instantiate(pluck, new Vector3(x, y, 0), Quaternion.identity).GetComponent<Pluck>();
            field[x, y].x = x;
            field[x, y].y = y;
            field[x, y].SetOwner(currentPlayer);

            CheckForWinner(x, y);

            columnFillCounter[x]++;
            changeCurrentPlayer();
            
        }
    }

    private void ResetColumnCounter()
    {
        for(int i = 0; i < columnFillCounter.Length; i++)
        {
            columnFillCounter[i] = 0;
        }

        currentPlayer = Pluck.PluckOwner.Player1;
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
            for(var j = -1; j < 2; j++)  // loops through neigbours (x axis)
            {
                if((i == 0 && j == 0) || (x + j) < 0 || (x + j) > 6 || (y + i) < 0 || (y + i) > 5) //excludes _currentPluck and indexes outside array bounds
                {
                    continue;
                }
                if (field[x+j, y+i] != null && field[x + j, y + i].GetComponent<Pluck>().owner == currentPlayer) //check if neighbour belongs to current player
                {
                    Pluck _neighbourPluck = field[x + j, y + i].GetComponent<Pluck>();
                    switch ((j, i))
                    {
                        case (-1, 0):
                        case (1, 0):
                            Debug.Log("Horizontal");
                            if(axes[0] == null)
                                axes[0] = new List<Pluck>();
                            axes[0].Add(_neighbourPluck);
                            break;

                        case (0, -1):
                        case (0, 1):    
                            Debug.Log("Vertical");
                            if (axes[1] == null)
                                axes[1] = new List<Pluck>();
                            axes[1].Add(_neighbourPluck);
                            break;

                        case (-1, -1):
                        case (1, 1):
                            Debug.Log("Ascending");
                            if (axes[2] == null)
                                axes[2] = new List<Pluck>();
                            axes[2].Add(_neighbourPluck);
                            break;

                        case (-1, 1):
                        case (1, -1):
                            Debug.Log("Descending");
                            if (axes[3] == null)
                                axes[3] = new List<Pluck>();
                            axes[3].Add(_neighbourPluck);
                            break;
                    }
                }
            }
        }

        for(int k = 0; k < axes.Length; k++)
        {
            if (axes[k] != null)
            {
                int x_Neighbour = axes[k][0].x;
                int y_Neighbour= axes[k][0].y;

                bool end_Positive = false;
                bool end_Negative = false;
                switch (k)
                {
                    //Horizontal
                    case 0:

                        for(int l = 1; l< 4; l++)
                        {
                            int x_AxisNeigbhourPositive = x_Neighbour + l;
                            int x_AxisNeigbhourNegative = x_Neighbour - l;

                            int y_AxisNeigbhourPositive = y_Neighbour + l;
                            int y_AxisNeigbhourNegative = y_Neighbour - l;

                            if (field[x_Neighbour + l, y_Neighbour] != null && field[x_Neighbour + l, y_Neighbour].owner == currentPlayer && end_Positive != false) // right (1,0)
                                axes[k].Add(field[x_Neighbour + l, y_Neighbour]);
                            else
                                end_Positive = true;

                            if (field[x_Neighbour - l, y_Neighbour] != null && field[x_Neighbour - l, y_Neighbour].owner == currentPlayer && end_Negative != false) // left (-1,0)
                                axes[k].Add(field[x_Neighbour - l, y_Neighbour]);
                            else
                                end_Negative = true;

                            if(end_Positive == true && end_Negative == true)
                                break;

                            if (axes[k].Count == 4)
                            {
                                Debug.Log(currentPlayer + " is the winner!");
                            }
                        }
                        break;
                    //Vertical
                    case 1:
                        for (int l = 1; l < 4; l++)
                        {
                            if (field[x_Neighbour, y_Neighbour + l] != null && field[x_Neighbour, y_Neighbour + l].owner == currentPlayer && end_Positive != false) // up (0,1)
                                axes[k].Add(field[x_Neighbour, y_Neighbour + l]);
                            else
                                end_Positive = true;

                            if (field[x_Neighbour, y_Neighbour - l] != null && field[x_Neighbour, y_Neighbour - l].owner == currentPlayer && end_Negative != false) // down (0,-1)
                                axes[k].Add(field[x_Neighbour, y_Neighbour - l]);
                            else
                                end_Negative = true;

                            if (end_Positive == true && end_Negative == true)
                                break;

                            if (axes[k].Count == 4)
                            {
                                Debug.Log(currentPlayer + " is the winner!");
                            }
                        }
                        break;
                    //Ascending
                    case 2:
                        for (int l = 1; l < 4; l++)
                        {
                            if (field[x_Neighbour + l, y_Neighbour + l]  != null && field[x_Neighbour + l, y_Neighbour + l].owner == currentPlayer && end_Positive != false) // up & right (1,1)
                                axes[k].Add(field[x_Neighbour + l, y_Neighbour + l]);
                            else
                                end_Positive = true;

                            if (field[x_Neighbour - l, y_Neighbour - l] != null && field[x_Neighbour - l, y_Neighbour - l].owner == currentPlayer && end_Negative != false) // down & left (-1,-1)
                                axes[k].Add(field[x_Neighbour - l, y_Neighbour - l]);
                            else
                                end_Negative = true;

                            if (end_Positive == true && end_Negative == true)
                                break;

                            if (axes[k].Count == 4)
                            {
                                Debug.Log(currentPlayer + " is the winner!");
                            }
                        }
                        break;
                    //Descending
                    case 3:
                        for (int l = 1; l < 4; l++)
                        {
                            if (field[x_Neighbour - l, y_Neighbour + l]  != null && field[x_Neighbour - l, y_Neighbour + l].owner == currentPlayer && end_Positive != false) // up & left (-1,1)
                                axes[k].Add(field[x_Neighbour - l, y_Neighbour + l]);
                            else
                                end_Positive = true;

                            if (field[x_Neighbour + l, y_Neighbour - l] != null && field[x_Neighbour + l, y_Neighbour - l].owner == currentPlayer && end_Negative != false) // down & right (1, -1)
                                axes[k].Add(field[x_Neighbour + l, y_Neighbour - l]);
                            else
                                end_Negative = true;

                            if (end_Positive == true && end_Negative == true)
                                break;

                            if (axes[k].Count == 4)
                            {
                                Debug.Log(currentPlayer + " is the winner!");
                            }
                        }
                        break;
                }
            }
        }
    }

    private void CheckAxix(int x, int y)
    {
        switch ((x, y))
        {
            case (-1, 0) :
            case (1, 0):
                Debug.Log("Horizontal");
                break;
            case (0, -1):
            case (0, 1):
                Debug.Log("Vertical");
                break;
            case (-1, -1):
            case (1, 1):
                Debug.Log("Ascending");
                break;
            case (-1, 1):
            case (1, -1):
                Debug.Log("Descending");
                break;
        }
    }

}
