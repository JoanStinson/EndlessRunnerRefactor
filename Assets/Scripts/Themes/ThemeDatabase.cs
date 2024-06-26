using System.Collections;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;

// Handles loading data from the Asset Bundle to handle different themes for the game
public class ThemeDatabase
{
    public static Dictionary<string, ThemeData> dictionnary { get { return themeDataList; } }
    public static bool loaded { get { return m_Loaded; } }

    protected static Dictionary<string, ThemeData> themeDataList;
    protected static bool m_Loaded = false;

    public static ThemeData GetThemeData(string type)
    {
        ThemeData list;
        if (themeDataList == null || !themeDataList.TryGetValue(type, out list))
        {
            return null;
        }

        return list;
    }

    public static IEnumerator LoadDatabase()
    {
        // If not null the dictionary was already loaded.
        if (themeDataList == null)
        {
            themeDataList = new Dictionary<string, ThemeData>();

            yield return Addressables.LoadAssetsAsync<ThemeData>("themeData", op =>
            {
                if (op != null)
                {
                    if (!themeDataList.ContainsKey(op.themeName))
                    {
                        themeDataList.Add(op.themeName, op);
                    }
                }
            });

            m_Loaded = true;
        }

    }
}
