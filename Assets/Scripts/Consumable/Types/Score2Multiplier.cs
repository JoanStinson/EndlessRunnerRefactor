using System.Collections;

public class Score2Multiplier : Consumable
{
    public override string GetConsumableName()
    {
        return "x2";
    }

    public override ConsumableType GetConsumableType()
    {
        return ConsumableType.SCORE_MULTIPLAYER;
    }

    public override int GetPrice()
    {
        return 750;
    }

	public override int GetPremiumCost()
	{
		return 0;
	}

	public override IEnumerator Started(CharacterInputController characterInputController)
    {
        yield return base.Started(characterInputController);
        m_SinceStart = 0;
        characterInputController.trackManager.modifyMultiply += MultiplyModify;
    }

    public override void Ended(CharacterInputController characterInputController)
    {
        base.Ended(characterInputController);
        characterInputController.trackManager.modifyMultiply -= MultiplyModify;
    }

    protected int MultiplyModify(int multi)
    {
        return multi * 2;
    }
}
