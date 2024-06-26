using System.Collections;

public class ExtraLife : Consumable
{
    protected const int k_MaxLives = 3;
    protected const int k_CoinValue = 10;

    public override string GetConsumableName()
    {
        return "Life";
    }

    public override ConsumableType GetConsumableType()
    {
        return ConsumableType.EXTRALIFE;
    }

    public override int GetPrice()
    {
        return 2000;
    }

	public override int GetPremiumCost()
	{
		return 5;
	}

    public override bool CanBeUsed(CharacterInputController characterInputController)
    {
        if (characterInputController.currentLife == characterInputController.maxLife)
        {
            return false;
        }

        return true;
    }

    public override IEnumerator Started(CharacterInputController characterInputController)
    {
        yield return base.Started(characterInputController);

        if (characterInputController.currentLife < k_MaxLives)
        {
            characterInputController.currentLife += 1;
        }
        else
        {
            characterInputController.coins += k_CoinValue;
        }
    }
}
