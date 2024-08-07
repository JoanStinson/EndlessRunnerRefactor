﻿using UnityEngine;
using System.Collections;
using UnityEngine.AddressableAssets;

/// <summary>
/// Defines a consumable (called "power up" in game). Each consumable is derived from this and implements its functions.
/// </summary>
public abstract class Consumable : MonoBehaviour
{
    public enum ConsumableType
    {
        NONE,
        COIN_MAG,
        SCORE_MULTIPLAYER,
        INVINCIBILITY,
        EXTRALIFE,
        MAX_COUNT
    }

    public bool active { get { return m_Active; } }
    public float timeActive { get { return m_SinceStart; } }

    public float duration;
    public Sprite icon;
    public AudioClip activatedSound;
    public AssetReference ActivatedParticleReference;
    public bool canBeSpawned = true;

    protected bool m_Active = true;
    protected float m_SinceStart;
    protected ParticleSystem m_ParticleSpawned;

    // Here - for the sake of showing diverse way of doing things - we use abstract functions to get the data for each consumable.
    // Another way to do it would be to have public field, like the Character or Accesories use, and define all those on the prefabs instead of here.
    // This method allows information to be all in code (so no need for prefab etc.) the other make it easier to modify without recompiling/by non-programmer.
    public abstract ConsumableType GetConsumableType();
    public abstract string GetConsumableName();
    public abstract int GetPrice();
    public abstract int GetPremiumCost();

    public void ResetTime()
    {
        m_SinceStart = 0;
    }

    //override this to do test to make a consumable not usable (e.g. used by the ExtraLife to avoid using it when at full health)
    public virtual bool CanBeUsed(CharacterInputController characterInputController)
    {
        return true;
    }

    public virtual IEnumerator Started(CharacterInputController characterInputController)
    {
        m_SinceStart = 0;

        if (activatedSound != null)
        {
            characterInputController.powerupSource.clip = activatedSound;
            characterInputController.powerupSource.Play();
        }

        if (ActivatedParticleReference != null)
        {
            //Addressables 1.0.1-preview
            var op = ActivatedParticleReference.InstantiateAsync();
            yield return op;

            if (op.Result.TryGetComponent<ParticleSystem>(out var particleSystem))
            {
                m_ParticleSpawned = particleSystem;
                
                if (!m_ParticleSpawned.main.loop)
                {
                    StartCoroutine(TimedRelease(m_ParticleSpawned.gameObject, m_ParticleSpawned.main.duration));
                }

                m_ParticleSpawned.transform.SetParent(characterInputController.characterCollider.transform);
                m_ParticleSpawned.transform.localPosition = op.Result.transform.position;
            }
        }
    }

    private IEnumerator TimedRelease(GameObject obj, float time)
    {
        yield return new WaitForSeconds(time);
        Addressables.ReleaseInstance(obj);
    }

    public virtual void Tick(CharacterInputController characterInputController)
    {
        // By default do nothing, override to do per frame manipulation
        m_SinceStart += Time.deltaTime;
        if (m_SinceStart >= duration)
        {
            m_Active = false;
            return;
        }
    }

    public virtual void Ended(CharacterInputController characterInputController)
    {
        if (m_ParticleSpawned != null)
        {
            if (m_ParticleSpawned.main.loop)
            {
                Addressables.ReleaseInstance(m_ParticleSpawned.gameObject);
            }
        }

        if (activatedSound != null && characterInputController.powerupSource.clip == activatedSound)
        {
            characterInputController.powerupSource.Stop(); //if this one the one using the audio source stop it
        }

        for (int i = 0; i < characterInputController.consumables.Count; i++)
        {
            //if there is still an active consumable that have a sound, this is the one playing now
            if (characterInputController.consumables[i].active && characterInputController.consumables[i].activatedSound != null)
            {
                characterInputController.powerupSource.clip = characterInputController.consumables[i].activatedSound;
                characterInputController.powerupSource.Play();
            }
        }
    }
}
