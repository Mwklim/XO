using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bot : MonoBehaviour
{
    [SerializeField] int numberPlayer;

    void Start()
    {
        Game_NextPlayer();
        Game.game.NextPlayer += Game_NextPlayer;
    }

    void OnDestroy()
    {
        Game.game.NextPlayer -= Game_NextPlayer;
    }


    private void Game_NextPlayer()
    {
        if (Game.game.playerBot[numberPlayer] && numberPlayer == Game.game.playerTurn && !Game.game.endGame)
        {
            StartCoroutine(GoBotCalculation());
        }
    }



    IEnumerator GoBotCalculation()
    {
        yield return PauseTime(0.5f);

        float bestScore = float.MinValue;
        Vector2 bestPos = new Vector2(0, 0);

        for (int i = 0; i < 3; i++)
        {
            for (int f = 0; f < 3; f++)
            {
                if (Game.game.fieldGame[i, f] == null)
                {
                    Game.game.fieldGame[i, f] = Game.game.playerSign[numberPlayer];
                    float score = CheckNextCell(Game.game.fieldGame, 0, numberPlayer);

                    Game.game.fieldGame[i, f] = null;
                    if (score > bestScore || (score == bestScore && Random.Range(0, 3) < 1))
                    {
                        bestScore = score;
                        bestPos = new Vector2(i, f);
                    }
                }
            }
        }
        Game.game.ImageBoard[(int)bestPos.y * 3 + (int)bestPos.x].SelectionCell();
    }

    IEnumerator<WaitForEndOfFrame> PauseTime(float time)
    {
        while (time > 0)
        {
            time -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

    float CheckNextCell(string[,] fieldGame, int depth, int nowNumber)
    {
        if (CheckWin(fieldGame, numberPlayer))
            return 1;

        int checkNextPlayer = numberPlayer + 1; if (checkNextPlayer > 1) checkNextPlayer = 0;
        if (CheckWin(fieldGame, checkNextPlayer))
            return -1;

        if (CheckFullCell(fieldGame))
            return 0;

        float bestScore = (nowNumber == numberPlayer ? float.MaxValue : float.MinValue);
        for (int i = 0; i < 3; i++)
        {
            for (int f = 0; f < 3; f++)
            {
                if (fieldGame[i, f] == null)
                {
                    int playerNext = nowNumber + 1; if (playerNext > 1) playerNext = 0;

                    fieldGame[i, f] = Game.game.playerSign[playerNext];
                    float score = CheckNextCell(fieldGame, depth + 1, playerNext);
                    fieldGame[i, f] = null;
                    bestScore = (playerNext == numberPlayer) ? Mathf.Max(score, bestScore) : Mathf.Min(score, bestScore);
                }
            }
        }
        return bestScore;
    }

    bool CheckWin(string[,] fieldGame, int nowNumber)
    {
        int sizeXY = 0;//по первой диагонали
        int sizeYX = 0;//по второй диагонали

        for (int y = 0; y < 3; y++)
        {
            int sizeX = 0;//по горизонтали
            int sizeY = 0;//по вертикали         

            for (int x = 0; x < 3; x++)
            {
                if (fieldGame[x, y] == Game.game.playerSign[nowNumber]) sizeX++;//вертикальный
                if (fieldGame[y, x] == Game.game.playerSign[nowNumber]) sizeY++;//горизонтальный
            }

            if (fieldGame[y, y] == Game.game.playerSign[nowNumber]) sizeXY++;
            if (fieldGame[y, 2 - y] == Game.game.playerSign[nowNumber]) sizeYX++;

            if (sizeX == 3) return true;
            if (sizeY == 3) return true;
        }

        if (sizeXY == 3) return true;
        if (sizeYX == 3) return true;

        return false;
    }

    bool CheckFullCell(string[,] fieldGame)
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
            return true;

        return false;
    }
}