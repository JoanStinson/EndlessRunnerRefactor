using UnityEngine;
using System;

public class CoinMagnet : Consumable
{
    protected readonly Vector3 k_HalfExtentsBox = new Vector3(20.0f, 1.0f, 1.0f);
    protected const int k_LayerMask = 1 << 8;
    protected Collider[] returnColls = new Collider[20];

    public override string GetConsumableName()
    {
        return "Magnet";
    }

    public override ConsumableType GetConsumableType()
    {
        return ConsumableType.COIN_MAG;
    }

    public override int GetPrice()
    {
        return 750;
    }

    public override int GetPremiumCost()
    {
        return 0;
    }

    public override void Tick(CharacterInputController characterInputController)
    {
        base.Tick(characterInputController);

        var position = characterInputController.characterCollider.transform.position;
        var rotation = characterInputController.characterCollider.transform.rotation;
        int nb = Physics.OverlapBoxNonAlloc(position, k_HalfExtentsBox, returnColls, rotation, k_LayerMask);

        for (int i = 0; i < nb; i++)
        {
            Coin returnCoin = returnColls[i].GetComponent<Coin>();

            if (returnCoin != null && !returnCoin.isPremium && !characterInputController.characterCollider.magnetCoins.Contains(returnCoin.gameObject))
            {
                returnColls[i].transform.SetParent(characterInputController.transform);
                characterInputController.characterCollider.magnetCoins.Add(returnColls[i].gameObject);
            }
        }
    }
}
