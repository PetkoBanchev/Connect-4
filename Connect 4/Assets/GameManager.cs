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
        Debug.Log("X: " + field.GetLength(0) + "; Y: " + field.GetLength(1));
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

        bool isWinner = false;

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
                    break;

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
                            break;

                        case (-1, -1):
                        case (1, 1):
                            Debug.Log("Ascending");
                            if (axes[2] == null)
                                axes[2] = new List<Pluck>();
                            axes[2].Add(_currentPluck);
                            break;

                        case (-1, 1):
                        case (1, -1):
                            Debug.Log("Descending");
                            if (axes[3] == null)
                                axes[3] = new List<Pluck>();
                            axes[3].Add(_currentPluck);
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

            Debug.Log("Axis count pre loop: " + axis.Count);

            for (int l = 1; l < 4; l++)
            {
                int x_Positive = xCur + (l * xDir);
                int x_Negative = xCur - (l * xDir);

                int y_Positive = yCur + (l * yDir);
                int y_Negative = yCur - (l * yDir);

                //Debug.Log("xP: " + x_Positive + " ; yP: " + y_Positive);
                //Debug.Log("xN: " + x_Negative + " ; yN: " + y_Negative);

                //Debug.Log("Loop: " + l + " ; neighbour: " + field[x_Positive, y_Positive].owner);

                //Positive direction
                if ((x_Positive < field.GetLength(0) && x_Positive > - 1) && (y_Positive < field.GetLength(1) && y_Positive > -1))
                {
                    if (field[x_Positive,y_Positive] != null && field[x_Positive, y_Positive].owner == currentPlayer && !end_Positive)
                    {
                        axis.Add(field[x_Positive, y_Positive]);
                        Debug.Log("Loop: " + l + "; Added pos; count: " + axis.Count);
                    }
                    else
                    {
                        Debug.Log("pos end " + end_Positive);
                        end_Positive = true;
                    }
                }


                //Negative direction
                if ((x_Negative < field.GetLength(0) && x_Negative > -1) && (y_Negative < field.GetLength(1) && y_Negative > -1))
                {
                    if (field[x_Negative, y_Negative] != null && field[x_Negative, y_Negative].owner == currentPlayer && !end_Negative)
                    {
                        axis.Add(field[x_Negative, y_Negative]);
                        Debug.Log("Loop: " + l + "; Added neg; count: " + axis.Count);
                    }
                    else
                    {
                        Debug.Log("neg end " + end_Negative);
                        end_Negative = true;
                    }
                }

                //Debug.Log("Loop: " + l + "; Axis count: " + axis.Count);

                if (axis.Count == 4)
                {
                    Debug.Log("Winner is: " + currentPlayer);
                    isWinner = true;    
                    break;
                }
            }
        }
        //if (axes[0] != null)
        //{
        //    int x_Neighbour = axes[0][0].x;
        //    int y_Neighbour = axes[0][0].y;

        //    bool end_Positive = false;
        //    bool end_Negative = false;

        //    int xAxisFixed; // 0 = true; 1 = false
        //    int yAxisFixed;

        //    //Horizontal

        //    xAxisFixed = 1;
        //    yAxisFixed = 0;

        //    for (int l = 1; l < 4; l++)
        //    {
        //        int x_AxisNeigbhourPositive = x_Neighbour + (l * xAxisFixed);
        //        int x_AxisNeigbhourNegative = x_Neighbour - (l * xAxisFixed);

        //        int y_AxisNeigbhourPositive = y_Neighbour + (l * yAxisFixed);
        //        int y_AxisNeigbhourNegative = y_Neighbour - (l * yAxisFixed);

        //        Pluck _positiveNeighbour = null;
        //        Pluck _negativeNeighbour = null;

        //        if (x_AxisNeigbhourPositive < field.GetLength(0) && y_AxisNeigbhourPositive < field.GetLength(1))
        //        {
        //            _positiveNeighbour = field[x_AxisNeigbhourPositive, y_AxisNeigbhourPositive];
        //            if (_positiveNeighbour != null && _positiveNeighbour.owner == currentPlayer && end_Positive != false) // right (1,0)
        //            {
        //                axes[0].Add(_positiveNeighbour);
        //                Debug.Log("pN: " + _positiveNeighbour + " pluck");
        //            }
        //            else
        //                end_Positive = true;
        //        }

        //        if (x_AxisNeigbhourNegative > -1 && y_AxisNeigbhourNegative > -1)
        //        {
        //            _negativeNeighbour = field[x_AxisNeigbhourNegative, y_AxisNeigbhourNegative];
        //            if (_negativeNeighbour != null && _negativeNeighbour.owner == currentPlayer && end_Negative != false) // left (-1,0)
        //            {
        //                axes[0].Add(_negativeNeighbour);
        //                Debug.Log("nN: " + _negativeNeighbour + " pluck");
        //            }
        //            else
        //                end_Negative = true;
        //        }


        //    Debug.Log("Loop: " + l + "Current axis count: " + axes[0].Count);
        //    }




        //    //if (end_Positive == true && end_Negative == true)
        //    //    break;

        //    if (axes[0].Count == 4)
        //    {
        //        Debug.Log(currentPlayer + " is the winner!");
        //    }

        //}


        //for(int k = 0; k < axes.Length; k++)
        //{
        //    if (axes[k] != null)
        //    {
        //        int x_Neighbour = axes[k][0].x;
        //        int y_Neighbour= axes[k][0].y;

        //        bool end_Positive = false;
        //        bool end_Negative = false;

        //        int xAxisFixed; // 0 = true; 1 = false
        //        int yAxisFixed;
        //        switch (k)
        //        {
        //            //Horizontal
        //            case 0:

        //                xAxisFixed = 1;
        //                yAxisFixed = 0;

        //                for(int l = 1; l< 4; l++)
        //                {
        //                    int x_AxisNeigbhourPositive = x_Neighbour + (l * xAxisFixed);
        //                    int x_AxisNeigbhourNegative = x_Neighbour - (l * xAxisFixed);

        //                    int y_AxisNeigbhourPositive = y_Neighbour + (l * yAxisFixed);
        //                    int y_AxisNeigbhourNegative = y_Neighbour - (l * yAxisFixed);

        //                    Pluck _positiveNeighbour = null;
        //                    Pluck _negativeNeighbour = null;

        //                    if (x_AxisNeigbhourPositive < field.GetLength(0) && y_AxisNeigbhourPositive < field.GetLength(1))
        //                    {
        //                        _positiveNeighbour = field[x_AxisNeigbhourPositive, y_AxisNeigbhourPositive];
        //                        if (_positiveNeighbour != null && _positiveNeighbour.owner == currentPlayer && end_Positive != false) // right (1,0)
        //                        {
        //                            axes[k].Add(_positiveNeighbour);
        //                            Debug.Log("pN: " + _positiveNeighbour + " pluck");
        //                        }
        //                        else
        //                            end_Positive = true;
        //                    }

        //                    if (x_AxisNeigbhourNegative > -1 && y_AxisNeigbhourNegative > -1)
        //                    {
        //                        _negativeNeighbour = field[x_AxisNeigbhourNegative, y_AxisNeigbhourNegative];
        //                        if (_negativeNeighbour != null && _negativeNeighbour.owner == currentPlayer && end_Negative != false) // left (-1,0)
        //                        {
        //                            axes[k].Add(_negativeNeighbour);
        //                            Debug.Log("nN: " + _negativeNeighbour + " pluck");
        //                        }
        //                        else
        //                            end_Negative = true;
        //                    }






        //                    Debug.Log("Loop: " + l + "Current axis count: " + axes[k].Count);

        //                    //if (end_Positive == true && end_Negative == true)
        //                    //    break;

        //                    if (axes[k].Count == 4)
        //                    {
        //                        Debug.Log(currentPlayer + " is the winner!");
        //                    }

        //                }
        //                break;
        //            //Vertical
        //            case 1:
        //                for (int l = 1; l < 4; l++)
        //                {
        //                    //if (field[x_Neighbour, y_Neighbour + l] != null && field[x_Neighbour, y_Neighbour + l].owner == currentPlayer && end_Positive != false) // up (0,1)
        //                    //    axes[k].Add(field[x_Neighbour, y_Neighbour + l]);
        //                    //else
        //                    //    end_Positive = true;

        //                    //if (field[x_Neighbour, y_Neighbour - l] != null && field[x_Neighbour, y_Neighbour - l].owner == currentPlayer && end_Negative != false) // down (0,-1)
        //                    //    axes[k].Add(field[x_Neighbour, y_Neighbour - l]);
        //                    //else
        //                    //    end_Negative = true;

        //                    //if (end_Positive == true && end_Negative == true)
        //                    //    break;

        //                    //if (axes[k].Count == 4)
        //                    //{
        //                    //    Debug.Log(currentPlayer + " is the winner!");
        //                    //}
        //                }
        //                break;
        //            //Ascending
        //            case 2:
        //                for (int l = 1; l < 4; l++)
        //                {
        //                    //if (field[x_Neighbour + l, y_Neighbour + l]  != null && field[x_Neighbour + l, y_Neighbour + l].owner == currentPlayer && end_Positive != false) // up & right (1,1)
        //                    //    axes[k].Add(field[x_Neighbour + l, y_Neighbour + l]);
        //                    //else
        //                    //    end_Positive = true;

        //                    //if (field[x_Neighbour - l, y_Neighbour - l] != null && field[x_Neighbour - l, y_Neighbour - l].owner == currentPlayer && end_Negative != false) // down & left (-1,-1)
        //                    //    axes[k].Add(field[x_Neighbour - l, y_Neighbour - l]);
        //                    //else
        //                    //    end_Negative = true;

        //                    //if (end_Positive == true && end_Negative == true)
        //                    //    break;

        //                    //if (axes[k].Count == 4)
        //                    //{
        //                    //    Debug.Log(currentPlayer + " is the winner!");
        //                    //}
        //                }
        //                break;
        //            //Descending
        //            case 3:
        //                for (int l = 1; l < 4; l++)
        //                {
        //                    //if (field[x_Neighbour - l, y_Neighbour + l]  != null && field[x_Neighbour - l, y_Neighbour + l].owner == currentPlayer && end_Positive != false) // up & left (-1,1)
        //                    //    axes[k].Add(field[x_Neighbour - l, y_Neighbour + l]);
        //                    //else
        //                    //    end_Positive = true;

        //                    //if (field[x_Neighbour + l, y_Neighbour - l] != null && field[x_Neighbour + l, y_Neighbour - l].owner == currentPlayer && end_Negative != false) // down & right (1, -1)
        //                    //    axes[k].Add(field[x_Neighbour + l, y_Neighbour - l]);
        //                    //else
        //                    //    end_Negative = true;

        //                    //if (end_Positive == true && end_Negative == true)
        //                    //    break;

        //                    //if (axes[k].Count == 4)
        //                    //{
        //                    //    Debug.Log(currentPlayer + " is the winner!");
        //                    //}
        //                }
        //                break;
        //        }
        //    }
        //}

    }
    private void CheckAxisForWinner()
    {

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
