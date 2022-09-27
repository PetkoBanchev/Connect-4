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

        for (int a = 0; a < axes.Length; a++)
        {
            Debug.Log(axes[a]);
        }

        //List<Pluck> horizontal; (-1, 0) && (1, 0)  - 0
        //List<Pluck> vertical;   (0, -1) && (0, 1)  - 1
        //List<Pluck> ascending;  (-1, -1) && (1, 1) - 2
        //List<Pluck> descending; (-1, 1) && (1, -1) - 3

        for (var i = -1; i < 2; i++)
        {
            for(var j = -1; j < 2; j++)
            {
                if((i == 0 && j == 0) || (x + j) < 0 || (x + j) > 6 || (y + i) < 0 || (y + i) > 5)
                {
                    continue;
                }
                if (field[x+j, y+i] != null && field[x + j, y + i].GetComponent<Pluck>().owner == currentPlayer)
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
                            axes[0].Add(_currentPluck);
                            break;

                        case (0, -1):
                        case (0, 1):    
                            Debug.Log("Vertical");
                            if (axes[1] == null)
                                axes[1] = new List<Pluck>();
                            axes[1].Add(_neighbourPluck);
                            axes[1].Add(_currentPluck);
                            break;

                        case (-1, -1):
                        case (1, 1):
                            Debug.Log("Ascending");
                            if (axes[2] == null)
                                axes[2] = new List<Pluck>();
                            axes[2].Add(_neighbourPluck);
                            axes[2].Add(_currentPluck);
                            break;

                        case (-1, 1):
                        case (1, -1):
                            Debug.Log("Descending");
                            if (axes[3] == null)
                                axes[3] = new List<Pluck>();
                            axes[3].Add(_neighbourPluck);
                            axes[3].Add(_currentPluck);
                            break;
                    }
                }
            }
        }

        for(int a = 0; a < axes.Length; a++)
        {
            if (axes[a] != null)
            {
                switch (a)
                {
                    //Horizontal
                    case 0:
                        int y0= axes[0][0].y;
                        if (field[axes[0][0].x + 1, y0].owner == currentPlayer)
                            axes[0].Add(field[axes[0][0].x + 1, y0]);
                        break;
                    //Vertical
                    case 1:
                        break;
                    //Ascending
                    case 2:
                        break;
                    //Descending
                    case 3:
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
