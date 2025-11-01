using UnityEngine;

public class Presenter_PlayerStat
{
    private Player_Controller player;
    private UI_PlayerStat uiPlayerStat;

    public Presenter_PlayerStat(Player_Controller player, UI_PlayerStat uiPlayerStat)
    {
        this.player = player;
        this.uiPlayerStat = uiPlayerStat;

        player.onPlayerStatChanged += OnTakeDamage;
        player.onPlayerStatChanged += OnShowStat;
    }

    private void OnTakeDamage()
    {
        if (player == null || uiPlayerStat == null) return;

        uiPlayerStat.SetHpBar(player.GetPlayerStat().GetStat(StatType.HP), player.GetPlayerStat().GetBaseStat(StatType.HP));
    }

    private void OnShowStat()
    {
        if (player == null || uiPlayerStat == null) return;

        uiPlayerStat.SetStatUI(player.GetPlayerStat());
    }
}
