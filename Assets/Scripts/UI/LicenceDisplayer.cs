using UnityEngine;

public class LicenceDisplayer : MonoBehaviour
{
    private void Start()
    {
        var playerData = ServiceLocator.Instance.GetService<IPlayerData>();
        playerData.Create();

        // If we have already accepted the licence, we close the popup, no need for it.
        if (playerData.LicenceAccepted)
        {
            Close();
        }
    }

    public void Accepted()
    {
        var playerData = ServiceLocator.Instance.GetService<IPlayerData>();
        playerData.LicenceAccepted = true;
        playerData.Save();
        Close();
    }

    public void Refuse()
    {
        Application.Quit();
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
