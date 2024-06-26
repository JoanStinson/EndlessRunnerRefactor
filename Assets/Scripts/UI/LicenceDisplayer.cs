using UnityEngine;

public class LicenceDisplayer : MonoBehaviour
{
    private void Start()
    {
        PlayerData.Create();

        // If we have already accepted the licence, we close the popup, no need for it.
        if (PlayerData.instance.licenceAccepted)
        {
            Close();
        }
    }

    public void Accepted()
    {
        PlayerData.instance.licenceAccepted = true;
        PlayerData.instance.Save();
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
