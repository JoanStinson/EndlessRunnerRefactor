using System.Collections.Generic;

public interface IPlayerData
{
    int UsedAccessory { get; set; }
    string PreviousName { get; set; }
    int Premium { get; set; }
    int Coins { get; set; }
    bool TutorialDone { get; set; }
    Dictionary<Consumable.ConsumableType, int> Consumables { get; set; }
    List<string> Characters { get; set; }
    List<string> Themes { get; set; }
    int UsedCharacter { get; set; }
    int UsedTheme { get; set; }
    float MasterVolume { get; set; }
    float MusicVolume { get; set; }
    float MasterSFXVolume { get; set; }
    List<string> CharacterAccessories { get; set; }
    List<MissionBase> Missions { get; set; }
    int FtueLevel { get; set; }
    int Rank { get; set; }
    bool LicenceAccepted { get; set; }
    List<HighscoreEntry> Highscores { get; set; }

    void Create();
    void Add(Consumable.ConsumableType type);
    void AddAccessory(string name);
    void AddCharacter(string name);
    void AddMission();
    void AddTheme(string theme);
    bool AnyMissionComplete();
    void CheckMissionsCount();
    void ClaimMission(MissionBase mission);
    void Consume(Consumable.ConsumableType type);
    int GetScorePlace(int score);
    void InsertScore(int score, string name);
    void Read();
    void Save();
    void StartRunMissions(ITrackManager manager);
    void UpdateMissions(ITrackManager manager);
    void NewSave();
}