using UnityEngine;

[CreateAssetMenu (menuName = "Spells/Heal Spell")]
public class HealSpellSO : SpellSO
{
    [Header("Heal Settings")]
    public int healAmount = 10;

    public override void Cast(Player player)
    {

        player.health.ChangeHealth(healAmount, player.transform.position);
    }
}
