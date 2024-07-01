using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This allows us to store a database of all characters currently in the bundles, indexed by name.
/// </summary>
public class CharacterDatabase
{
    public static Dictionary<string, Character> dictionary { get { return m_CharactersDict; } }
    public static bool loaded { get { return m_Loaded; } }

    protected static Dictionary<string, Character> m_CharactersDict;
    protected static bool m_Loaded = false;

    public static Character GetCharacter(string type)
    {
        Character character;

        if (m_CharactersDict == null || !m_CharactersDict.TryGetValue(type, out character))
        {
            return null;
        }

        return character;
    }

    public static IEnumerator LoadDatabase()
    {
        if (m_CharactersDict == null)
        {
            m_CharactersDict = new Dictionary<string, Character>();

            yield return Addressables.LoadAssetsAsync<GameObject>("characters", op =>
            {
                if (op.TryGetComponent<Character>(out var character))
                {
                    m_CharactersDict.Add(character.characterName, character);
                }
            });

            m_Loaded = true;
        }
    }
}