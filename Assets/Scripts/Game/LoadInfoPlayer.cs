using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadInfoPlayer : MonoBehaviour
{
    [SerializeField] int numberPlayer;
    [SerializeField] Text NamePlayer;
    [SerializeField] Image BackgroundPlayer;

    float setAlpha = 1f;

    void Start()
    {
        if (numberPlayer == 0)
            NamePlayer.text = Settings.namePlayer;

        Game_NextPlayer();
        Game.game.NextPlayer += Game_NextPlayer;
    }

    void OnDestroy()
    {
        Game.game.NextPlayer -= Game_NextPlayer;
    }

    private void Update()
    {
        BackgroundPlayer.color = new Color(BackgroundPlayer.color.r, BackgroundPlayer.color.g, BackgroundPlayer.color.b,
          CustomMethods.AnimParameter(BackgroundPlayer.color.a, setAlpha, 3f));
    }

    private void Game_NextPlayer()
    {
        setAlpha = (numberPlayer == Game.game.playerTurn && !Game.game.endGame) ? 1f : 0;
    }
}
