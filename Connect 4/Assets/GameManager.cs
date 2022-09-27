using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject[,] field = new GameObject[7, 6];
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
            field[x,y] = Instantiate(pluck, new Vector3(x, y, 0), Quaternion.identity);
            Pluck _pluck = field[x, y].GetComponent<Pluck>();
            _pluck.x = x;
            _pluck.y = y;
            _pluck.SetOwner(currentPlayer);

            columnFillCounter[x]++;
            changeCurrentPlayer();
            ChangePlayerTurnText();
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
    }

}
