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

    private bool isPlayer1Turn = true;

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

            if (isPlayer1Turn)
            {
                _pluck.SetOwner(Pluck.PluckOwner.Player1);
            }
            else
            {
                _pluck.SetOwner(Pluck.PluckOwner.Player2);
            }
            columnFillCounter[x]++;
            isPlayer1Turn = !isPlayer1Turn;
            ChangePlayerTurnText(isPlayer1Turn);
        }
    }

    private void ResetColumnCounter()
    {
        for(int i = 0; i < columnFillCounter.Length; i++)
        {
            columnFillCounter[i] = 0;
        }

        isPlayer1Turn = true;
    }

    private void ChangePlayerTurnText(bool _isPlayer1)
    {
        if (_isPlayer1)
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

}
