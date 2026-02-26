using System;
using UnityEngine;

public class CoinParticleSystem : MonoBehaviour
{
    [Header("Effect Settings")]
    [SerializeField] private ParticleSystem _particleSystem;
    [SerializeField] private FloatingTextController _floatingTextPrefab;
    [SerializeField] private float _heightOffset;

    public void OnFinishStudying(int amount)
    {
        PlayCoinEffect(amount);
    }

    private void PlayCoinEffect(int amount)
    {
        Vector3 spawnPos = transform.position + Vector3.up * _heightOffset;
        if (_particleSystem != null)
        {
            {
                ParticleSystem newEffect = Instantiate(_particleSystem, spawnPos, Quaternion.identity);
                newEffect.Play();
                Destroy(newEffect.gameObject, 3f);
            }
        }
        if (_floatingTextPrefab != null)
        {
            var textObj = Instantiate(_floatingTextPrefab, spawnPos, Quaternion.identity);
            textObj.Setup(amount);
        }
        else
        {
            Debug.LogWarning("Chua gan particle system");
        }
    }
}
