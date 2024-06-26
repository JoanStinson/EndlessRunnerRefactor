using System.Collections;

public class Invincibility : Consumable
{
    public override string GetConsumableName()
    {
        return "Invincible";
    }

    public override ConsumableType GetConsumableType()
    {
        return ConsumableType.INVINCIBILITY;
    }

    public override int GetPrice()
    {
        return 1500;
    }

	public override int GetPremiumCost()
	{
		return 5;
	}

	public override void Tick(CharacterInputController characterInputController)
    {
        base.Tick(characterInputController);
        characterInputController.characterCollider.SetInvincibleExplicit(true);
    }

    public override IEnumerator Started(CharacterInputController characterInputController)
    {
        yield return base.Started(characterInputController);
        characterInputController.characterCollider.SetInvincible(duration);
    }

    public override void Ended(CharacterInputController characterInputController)
    {
        base.Ended(characterInputController);
        characterInputController.characterCollider.SetInvincibleExplicit(false);
    }
}
