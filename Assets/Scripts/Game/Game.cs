using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    public static Game game;
    public SetCell[] ImageBoard;
    public Sprite[] spriteCell;
    public int playerTurn = 0;//0 - человек, 1 - бот

    public bool[] playerBot = new bool[] { false, true };

    [SerializeField] Image finishLine;
    int sizeFieldGame = 3;
    public string[,] fieldGame;

    public string[] playerSign = { "X", "O" };
    public bool endGame = false;

    public event System.Action NextPlayer = () => { };

    public GameObject prefabEndGame;
    GameObject windowEndGame;

    void Awake()
    {
        game = this;
        playerTurn = Random.Range(0, 2);
        fieldGame = new string[sizeFieldGame, sizeFieldGame];
    }

    public void SetCell(int cellX, int cellY)
    {
        fieldGame[cellX, cellY] = playerSign[playerTurn];
        if (CheckWin())
        {
            endGame = true;
            Settings.result = (!playerBot[playerTurn]) ? ResultGame.Victory : ResultGame.Defeat;
            Settings.rating += (!playerBot[playerTurn]) ? 100 : -100;
            Settings.SaveSettings();
        }
        else
        {
            int checkFreeCell = 0;
            for (int y = 0; y < 3; y++)
            {
                for (int x = 0; x < 3; x++)
                {
                    if (fieldGame[y, x] == null) checkFreeCell++;
                }
            }

            if (checkFreeCell <= 0)
            {
                endGame = true;
                Settings.result = ResultGame.Draw;
            }

            playerTurn++;
            if (playerTurn > 1) playerTurn = 0;
            NextPlayer();
        }

        if (endGame && windowEndGame == null)
        {
            Settings.gameLaunch++;
            windowEndGame = Instantiate(prefabEndGame);
        }
    }

    bool CheckWin()
    {
        int sizeXY = 0;//по первой диагонали
        int sizeYX = 0;//по второй диагонали

        for (int y = 0; y < 3; y++)
        {
            int sizeX = 0;//по горизонтали
            int sizeY = 0;//по вертикали         

            for (int x = 0; x < 3; x++)
            {
                if (fieldGame[x, y] == playerSign[playerTurn]) sizeX++;//вертикальный
                if (fieldGame[y, x] == playerSign[playerTurn]) sizeY++;//горизонтальный
            }

            if (fieldGame[y, y] == playerSign[playerTurn]) sizeXY++;
            if (fieldGame[y, 2 - y] == playerSign[playerTurn]) sizeYX++;

            if (sizeX == 3)
            {
                FinishLine(new Vector2(0, (1f - y) * 270f), 0f);
                return true;
            }

            if (sizeY == 3)
            {
                FinishLine(new Vector2((y - 1f) * 270f, 0), -90f);
                return true;
            }
        }

        if (sizeXY == 3)
        {
            FinishLine(new Vector2(0, 0), -45f);
            return true;
        }

        if (sizeYX == 3)
        {
            FinishLine(new Vector2(0, 0), 45f);
            return true;
        }

        return false;
    }

    void FinishLine(Vector2 pos, float rot)
    {
        finishLine.gameObject.SetActive(true);
        finishLine.gameObject.transform.localPosition = pos;
        finishLine.gameObject.transform.rotation = Quaternion.Euler(0, 0, rot);
        StartCoroutine(SetAmountLine());
    }

    IEnumerator SetAmountLine()
    {
        finishLine.fillAmount = 0;
        while (finishLine.fillAmount < 1)
        {
            finishLine.fillAmount += Time.deltaTime;
            yield return null;
        }
    }
}
