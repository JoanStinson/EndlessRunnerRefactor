using UnityEngine;
using UnityEngine.UI;
#if UNITY_ANALYTICS
using UnityEngine.Analytics;
#endif
using System.Collections.Generic;

/// <summary>
/// state pushed on top of the GameManager when the player dies.
/// </summary>
public class GameOverState : AState
{
    public Canvas canvas;
    public MissionUI missionPopup;
    public AudioClip gameOverTheme;
    public Leaderboard miniLeaderboard;
    public Leaderboard fullLeaderboard;
    public GameObject addButton;
    private IPlayerData m_playerData;

    public override void Enter(AState from)
    {
        m_playerData = ServiceLocator.Instance.GetService<IPlayerData>();
        trackManager = ServiceLocator.Instance.GetService<ITrackManager>();
        canvas.gameObject.SetActive(true);
        miniLeaderboard.playerEntry.inputName.text = m_playerData.PreviousName;
        miniLeaderboard.playerEntry.score.text = trackManager.score.ToString();
        miniLeaderboard.Populate();

        if (m_playerData.AnyMissionComplete())
        {
            StartCoroutine(missionPopup.Open());
        }
        else
        {
            missionPopup.gameObject.SetActive(false);
        }

        CreditCoins();

        var musicPlayer = ServiceLocator.Instance.GetService<IMusicPlayer>();
        if (musicPlayer.GetStem(0) != gameOverTheme)
        {
            musicPlayer.SetStem(0, gameOverTheme);
            StartCoroutine(musicPlayer.RestartAllStems());
        }
    }

    public override void Exit(AState to)
    {
        canvas.gameObject.SetActive(false);
        FinishRun();
    }

    public override string GetName()
    {
        return "GameOver";
    }

    public override void Tick()
    {

    }

    public void OpenLeaderboard()
    {
        fullLeaderboard.forcePlayerDisplay = false;
        fullLeaderboard.displayPlayer = true;
        fullLeaderboard.playerEntry.playerName.text = miniLeaderboard.playerEntry.inputName.text;
        fullLeaderboard.playerEntry.score.text = trackManager.score.ToString();
        fullLeaderboard.Open();
    }

    public void GoToStore()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("shop", UnityEngine.SceneManagement.LoadSceneMode.Additive);
    }

    public void GoToLoadout()
    {
        trackManager.isRerun = false;
        gameManager.SwitchState("Loadout");
    }

    public void RunAgain()
    {
        trackManager.isRerun = false;
        gameManager.SwitchState("Game");
    }

    protected void CreditCoins()
    {
        m_playerData.Save();

#if UNITY_ANALYTICS // Using Analytics Standard Events v0.3.0
        var transactionId = System.Guid.NewGuid().ToString();
        var transactionContext = "gameplay";
        var level = PlayerData.instance.rank.ToString();
        var itemType = "consumable";
        
        if (trackManager.characterController.coins > 0)
        {
            AnalyticsEvent.ItemAcquired(
                AcquisitionType.Soft, // Currency type
                transactionContext,
                trackManager.characterController.coins,
                "fishbone",
                PlayerData.instance.coins,
                itemType,
                level,
                transactionId
            );
        }

        if (trackManager.characterController.premium > 0)
        {
            AnalyticsEvent.ItemAcquired(
                AcquisitionType.Premium, // Currency type
                transactionContext,
                trackManager.characterController.premium,
                "anchovies",
                PlayerData.instance.premium,
                itemType,
                level,
                transactionId
            );
        }
#endif
    }

    protected void FinishRun()
    {
        if (miniLeaderboard.playerEntry.inputName.text == "")
        {
            miniLeaderboard.playerEntry.inputName.text = "Trash Cat";
        }
        else
        {
            m_playerData.PreviousName = miniLeaderboard.playerEntry.inputName.text;
        }

        m_playerData.InsertScore(trackManager.score, miniLeaderboard.playerEntry.inputName.text);

        var de = trackManager.CharacterController.characterCollider.deathData;
        //register data to analytics
#if UNITY_ANALYTICS
        AnalyticsEvent.GameOver(null, new Dictionary<string, object> {
            { "coins", de.coins },
            { "premium", de.premium },
            { "score", de.score },
            { "distance", de.worldDistance },
            { "obstacle",  de.obstacleType },
            { "theme", de.themeUsed },
            { "character", de.character },
        });
#endif

        m_playerData.Save();
        trackManager.End();
    }
}
