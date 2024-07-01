using UnityEngine;
using System.Collections.Generic;
#if UNITY_ANALYTICS
using UnityEngine.Analytics;
#endif

public class ShopCharacterList : ShopList
{
    public override void Populate()
    {
        m_RefreshCallback = null;

        foreach (Transform t in listRoot)
        {
            Destroy(t.gameObject);
        }

        foreach (KeyValuePair<string, Character> pair in CharacterDatabase.dictionary)
        {
            Character c = pair.Value;

            if (c != null)
            {
                prefabItem.InstantiateAsync().Completed += (op) =>
                {
                    if (op.Result == null || !(op.Result is GameObject))
                    {
                        Debug.LogWarning(string.Format("Unable to load character shop list {0}.", prefabItem.Asset.name));
                        return;
                    }

                    GameObject newEntry = op.Result;
                    newEntry.transform.SetParent(listRoot, false);
                    ShopItemListItem itm = newEntry.GetComponent<ShopItemListItem>();
                    itm.icon.sprite = c.icon;
                    itm.nameText.text = c.characterName;
                    itm.pricetext.text = c.cost.ToString();
                    itm.buyButton.image.sprite = itm.buyButtonSprite;

                    if (c.premiumCost > 0)
                    {
                        itm.premiumText.transform.parent.gameObject.SetActive(true);
                        itm.premiumText.text = c.premiumCost.ToString();
                    }
                    else
                    {
                        itm.premiumText.transform.parent.gameObject.SetActive(false);
                    }

                    itm.buyButton.onClick.AddListener(delegate () { Buy(c); });
                    m_RefreshCallback += delegate () { RefreshButton(itm, c); };
                    RefreshButton(itm, c);
                };
            }
        }
    }

    protected void RefreshButton(ShopItemListItem itm, Character c)
    {
        var playerData = ServiceLocator.Instance.GetService<IPlayerData>();

        if (c.cost > playerData.Coins)
        {
            itm.buyButton.interactable = false;
            itm.pricetext.color = Color.red;
        }
        else
        {
            itm.pricetext.color = Color.black;
        }

        if (c.premiumCost > playerData.Premium)
        {
            itm.buyButton.interactable = false;
            itm.premiumText.color = Color.red;
        }
        else
        {
            itm.premiumText.color = Color.black;
        }

        if (playerData.Characters.Contains(c.characterName))
        {
            itm.buyButton.interactable = false;
            itm.buyButton.image.sprite = itm.disabledButtonSprite;
            itm.buyButtonText.text = "Owned";
        }
    }

    public void Buy(Character c)
    {
        var playerData = ServiceLocator.Instance.GetService<IPlayerData>();
        playerData.Coins -= c.cost;
        playerData.Premium -= c.premiumCost;
        playerData.AddCharacter(c.characterName);
        playerData.Save();

#if UNITY_ANALYTICS // Using Analytics Standard Events v0.3.0
        var transactionId = System.Guid.NewGuid().ToString();
        var transactionContext = "store";
        var level = PlayerData.instance.rank.ToString();
        var itemId = c.characterName;
        var itemType = "non_consumable";
        var itemQty = 1;

        AnalyticsEvent.ItemAcquired(
            AcquisitionType.Soft,
            transactionContext,
            itemQty,
            itemId,
            itemType,
            level,
            transactionId
        );
        
        if (c.cost > 0)
        {
            AnalyticsEvent.ItemSpent(
                AcquisitionType.Soft, // Currency type
                transactionContext,
                c.cost,
                itemId,
                PlayerData.instance.coins, // Balance
                itemType,
                level,
                transactionId
            );
        }

        if (c.premiumCost > 0)
        {
            AnalyticsEvent.ItemSpent(
                AcquisitionType.Premium, // Currency type
                transactionContext,
                c.premiumCost,
                itemId,
                PlayerData.instance.premium, // Balance
                itemType,
                level,
                transactionId
            );
        }
#endif

        // Repopulate to change button accordingly.
        Populate();
    }
}
