using UnityEngine;
using System.IO;
using System.Collections.Generic;
#if UNITY_ANALYTICS
using UnityEngine.Analytics;
#endif
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Save data for the game. This is stored locally in this case, but a "better" way to do it would be to store it on a server
/// somewhere to avoid player tampering with it. Here potentially a player could modify the binary file to add premium currency.
/// </summary>
public class PlayerData : IPlayerData
{
    public int UsedAccessory { get; set; } = -1;
    public string PreviousName { get; set; } = "Trash Cat";
    public int Premium { get; set; }
    public int Coins { get; set; }
    public bool TutorialDone { get; set; } = false;
    public Dictionary<Consumable.ConsumableType, int> Consumables { get; set; } = new Dictionary<Consumable.ConsumableType, int>();
    public List<string> Characters { get; set; } = new List<string>();
    public List<string> Themes { get; set; } = new List<string>();
    public int UsedCharacter { get; set; }
    public int UsedTheme { get; set; }
    public float MasterVolume { get; set; } = float.MinValue;
    public float MusicVolume { get; set; } = float.MinValue;
    public float MasterSFXVolume { get; set; } = float.MinValue;
    public List<MissionBase> Missions { get; set; } = new List<MissionBase>();

    // List of owned accessories, in the form "charName:accessoryName".
    public List<string> CharacterAccessories { get; set; } = new List<string>();
    
    //ftue = First Time User Expeerience. This var is used to track thing a player do for the first time. It increment everytime the user do one of the step
    //e.g. it will increment to 1 when they click Start, to 2 when doing the first run, 3 when running at least 300m etc.
    public int FtueLevel { get; set; } = 0;

    //Player win a rank ever 300m (e.g. a player having reached 1200m at least once will be rank 4)
    public int Rank { get; set; } = 0;
    public bool LicenceAccepted { get; set; }
    public List<HighscoreEntry> Highscores { get; set; } = new List<HighscoreEntry>();

    protected string saveFile = "";

    // This will allow us to add data even after production, and so keep all existing save STILL valid. See loading & saving for how it work.
    // Note in a real production it would probably reset that to 1 before release (as all dev save don't have to be compatible w/ final product)
    // Then would increment again with every subsequent patches. We kept it to its dev value here for teaching purpose. 
    private static int s_Version = 12;

    public void Create()
    {
        if (saveFile == "")
        {
            //if we create the PlayerData, mean it's the very first call, so we use that to init the database
            //this allow to always init the database at the earlier we can, i.e. the start screen if started normally on device
            //or the Loadout screen if testing in editor
            CoroutineHandler.StartStaticCoroutine(CharacterDatabase.LoadDatabase());
            CoroutineHandler.StartStaticCoroutine(ThemeDatabase.LoadDatabase());
        }

        saveFile = Application.persistentDataPath + "/save.bin";

        if (File.Exists(saveFile))
        {
            // If we have a save, we read it.
            Read();
        }
        else
        {
            // If not we create one with default data.
            NewSave();
        }

        CheckMissionsCount();
    }

    public void Consume(Consumable.ConsumableType type)
    {
        if (!Consumables.ContainsKey(type))
        {
            return;
        }

        Consumables[type] -= 1;

        if (Consumables[type] == 0)
        {
            Consumables.Remove(type);
        }

        Save();
    }

    public void Add(Consumable.ConsumableType type)
    {
        if (!Consumables.ContainsKey(type))
        {
            Consumables[type] = 0;
        }

        Consumables[type] += 1;
        Save();
    }

    public void AddCharacter(string name)
    {
        Characters.Add(name);
    }

    public void AddTheme(string theme)
    {
        Themes.Add(theme);
    }

    public void AddAccessory(string name)
    {
        CharacterAccessories.Add(name);
    }

    // Mission management
    // Will add missions until we reach 2 missions.
    public void CheckMissionsCount()
    {
        while (Missions.Count < 2)
        {
            AddMission();
        }
    }

    public void AddMission()
    {
        int val = Random.Range(0, (int)MissionBase.MissionType.MAX);
        MissionBase newMission = MissionBase.GetNewMissionFromType((MissionBase.MissionType)val);
        newMission.Created();
        Missions.Add(newMission);
    }

    public void StartRunMissions(ITrackManager manager)
    {
        for (int i = 0; i < Missions.Count; i++)
        {
            Missions[i].RunStart(manager);
        }
    }

    public void UpdateMissions(ITrackManager manager)
    {
        for (int i = 0; i < Missions.Count; i++)
        {
            Missions[i].Update(manager);
        }
    }

    public bool AnyMissionComplete()
    {
        for (int i = 0; i < Missions.Count; i++)
        {
            if (Missions[i].isComplete)
            {
                return true;
            }
        }

        return false;
    }

    public void ClaimMission(MissionBase mission)
    {
        Premium += mission.reward;

#if UNITY_ANALYTICS // Using Analytics Standard Events v0.3.0
        AnalyticsEvent.ItemAcquired(
            AcquisitionType.Premium, // Currency type
            "mission",               // Context
            mission.reward,          // Amount
            "anchovies",             // Item ID
            premium,                 // Item balance
            "consumable",            // Item type
            rank.ToString()          // Level
        );
#endif

        Missions.Remove(mission);
        CheckMissionsCount();
        Save();
    }

    // High Score management
    public int GetScorePlace(int score)
    {
        HighscoreEntry entry = new HighscoreEntry();
        entry.score = score;
        entry.name = "";
        int index = Highscores.BinarySearch(entry);
        return index < 0 ? (~index) : index;
    }

    public void InsertScore(int score, string name)
    {
        HighscoreEntry entry = new HighscoreEntry();
        entry.score = score;
        entry.name = name;
        Highscores.Insert(GetScorePlace(score), entry);

        // Keep only the 10 best scores.
        while (Highscores.Count > 10)
        {
            Highscores.RemoveAt(Highscores.Count - 1);
        }
    }

    public void NewSave()
    {
        Characters.Clear();
        Themes.Clear();
        Missions.Clear();
        CharacterAccessories.Clear();
        Consumables.Clear();
        UsedCharacter = 0;
        UsedTheme = 0;
        UsedAccessory = -1;
        Coins = 0;
        Premium = 0;
        Characters.Add("Trash Cat");
        Themes.Add("Day");
        FtueLevel = 0;
        Rank = 0;
        TutorialDone = false;
        CheckMissionsCount();
        Save();
    }

    public void Read()
    {
        BinaryReader r = new BinaryReader(new FileStream(saveFile, FileMode.Open));

        int ver = r.ReadInt32();

        if (ver < 6)
        {
            r.Close();
            NewSave();
            r = new BinaryReader(new FileStream(saveFile, FileMode.Open));
            ver = r.ReadInt32();
        }

        Coins = r.ReadInt32();
        Consumables.Clear();
        int consumableCount = r.ReadInt32();

        for (int i = 0; i < consumableCount; i++)
        {
            Consumables.Add((Consumable.ConsumableType)r.ReadInt32(), r.ReadInt32());
        }

        // Read character.
        Characters.Clear();
        int charCount = r.ReadInt32();
        for (int i = 0; i < charCount; i++)
        {
            string charName = r.ReadString();

            if (charName.Contains("Raccoon") && ver < 11)
            {//in 11 version, we renamed Raccoon (fixing spelling) so we need to patch the save to give the character if player had it already
                charName = charName.Replace("Racoon", "Raccoon");
            }

            Characters.Add(charName);
        }

        UsedCharacter = r.ReadInt32();

        // Read character accesories.
        CharacterAccessories.Clear();
        int accCount = r.ReadInt32();
        for (int i = 0; i < accCount; i++)
        {
            CharacterAccessories.Add(r.ReadString());
        }

        // Read Themes.
        Themes.Clear();
        int themeCount = r.ReadInt32();
        for (int i = 0; i < themeCount; i++)
        {
            Themes.Add(r.ReadString());
        }

        UsedTheme = r.ReadInt32();

        // Save contains the version they were written with. If data are added bump the version & test for that version before loading that data.
        if (ver >= 2)
        {
            Premium = r.ReadInt32();
        }

        // Added highscores.
        if (ver >= 3)
        {
            Highscores.Clear();
            int count = r.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                HighscoreEntry entry = new HighscoreEntry();
                entry.name = r.ReadString();
                entry.score = r.ReadInt32();
                Highscores.Add(entry);
            }
        }

        // Added missions.
        if (ver >= 4)
        {
            Missions.Clear();

            int count = r.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                MissionBase.MissionType type = (MissionBase.MissionType)r.ReadInt32();
                MissionBase tempMission = MissionBase.GetNewMissionFromType(type);
                tempMission.Deserialize(r);

                if (tempMission != null)
                {
                    Missions.Add(tempMission);
                }
            }
        }

        // Added highscore previous name used.
        if (ver >= 7)
        {
            PreviousName = r.ReadString();
        }

        if (ver >= 8)
        {
            LicenceAccepted = r.ReadBoolean();
        }

        if (ver >= 9)
        {
            MasterVolume = r.ReadSingle();
            MusicVolume = r.ReadSingle();
            MasterSFXVolume = r.ReadSingle();
        }

        if (ver >= 10)
        {
            FtueLevel = r.ReadInt32();
            Rank = r.ReadInt32();
        }

        if (ver >= 12)
        {
            TutorialDone = r.ReadBoolean();
        }

        r.Close();
    }

    public void Save()
    {
        BinaryWriter w = new BinaryWriter(new FileStream(saveFile, FileMode.OpenOrCreate));

        w.Write(s_Version);
        w.Write(Coins);

        w.Write(Consumables.Count);
        foreach (KeyValuePair<Consumable.ConsumableType, int> p in Consumables)
        {
            w.Write((int)p.Key);
            w.Write(p.Value);
        }

        // Write characters.
        w.Write(Characters.Count);
        foreach (string c in Characters)
        {
            w.Write(c);
        }

        w.Write(UsedCharacter);

        w.Write(CharacterAccessories.Count);
        foreach (string a in CharacterAccessories)
        {
            w.Write(a);
        }

        // Write themes.
        w.Write(Themes.Count);
        foreach (string t in Themes)
        {
            w.Write(t);
        }

        w.Write(UsedTheme);
        w.Write(Premium);

        // Write highscores.
        w.Write(Highscores.Count);
        for (int i = 0; i < Highscores.Count; i++)
        {
            w.Write(Highscores[i].name);
            w.Write(Highscores[i].score);
        }

        // Write missions.
        w.Write(Missions.Count);
        for (int i = 0; i < Missions.Count; i++)
        {
            w.Write((int)Missions[i].GetMissionType());
            Missions[i].Serialize(w);
        }

        // Write name.
        w.Write(PreviousName);
        w.Write(LicenceAccepted);
        w.Write(MasterVolume);
        w.Write(MusicVolume);
        w.Write(MasterSFXVolume);
        w.Write(FtueLevel);
        w.Write(Rank);
        w.Write(TutorialDone);
        w.Close();
    }
}

// Helper class to cheat in the editor for test purpose
#if UNITY_EDITOR
public class PlayerDataEditor : Editor
{
    [MenuItem("Trash Dash Debug/Clear Save")]
    public static void ClearSave()
    {
        File.Delete(Application.persistentDataPath + "/save.bin");
    }

    [MenuItem("Trash Dash Debug/Give 1000000 fishbones and 1000 premium")]
    public static void GiveCoins()
    {
        var playerData = ServiceLocator.Instance.GetService<IPlayerData>();
        playerData.Coins += 1000000;
        playerData.Premium += 1000;
        playerData.Save();
    }

    [MenuItem("Trash Dash Debug/Give 10 Consumables of each types")]
    public static void AddConsumables()
    {
        var playerData = ServiceLocator.Instance.GetService<IPlayerData>();

        for (int i = 0; i < ShopItemList.s_ConsumablesTypes.Length; i++)
        {
            Consumable c = ConsumableDatabase.GetConsumbale(ShopItemList.s_ConsumablesTypes[i]);
            if (c != null)
            {
                playerData.Consumables[c.GetConsumableType()] = 10;
            }
        }

        playerData.Save();
    }
}
#endif