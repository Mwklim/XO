using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SetCell : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] Image sprite;
    [SerializeField] int CellY;
    [SerializeField] int CellX;

    [SerializeField] string cell = "";

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!Game.game.playerBot[Game.game.playerTurn])
            SelectionCell();
    }

    public void SelectionCell()
    {
        if (!Game.game.endGame && cell == "")
        {
            cell = Game.game.playerSign[Game.game.playerTurn];
            if (sprite != null) sprite.sprite = Game.game.spriteCell[Game.game.playerTurn];
            Game.game.SetCell(CellX, CellY);
        }
    }
}
