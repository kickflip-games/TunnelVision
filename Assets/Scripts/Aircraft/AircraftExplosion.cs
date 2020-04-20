/* Written by Avi Vajpeyi
 */


using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AircraftExplosion : MonoBehaviour
{
    private AudioClip _explosionAudioClip;
    private GameObject[] _renderedGameObjects;
    private AudioSource _audioSource;
    private GameObject _shardContainer;
    private List<GameObject> Shards = new List<GameObject>();

    public void SetReferences(
        AudioSource audioSource, 
        AudioClip explosionAudio,
        GameObject[] renderedGameObjects,
        GameObject shardContainer
        )
    {
        _audioSource = audioSource;
        _explosionAudioClip = explosionAudio;
        _renderedGameObjects = renderedGameObjects;
        _shardContainer = shardContainer;
        foreach (Transform shard in _shardContainer.transform)
        {
            Shards.Add(shard.gameObject);
        }
    }


    public void Explode()
    {
        DisableRenderedObjects();
        ActivateShards();
        PlayExplosionSound();
    }

    IEnumerator DestroyShardCoroutine(GameObject GO)
    {
        yield return new WaitForSeconds(2 + Random.Range(0.0f, 5.0f));
        Destroy(GO);
    }


    void PlayExplosionSound()
    {
        if (_audioSource != null)
        {
            Debug.Log("shatter sound");
            _audioSource.PlayOneShot(_explosionAudioClip, 0.7F);
        }
    }

    void DisableRenderedObjects()
    {
        foreach (GameObject g in _renderedGameObjects)
        {
            g.SetActive(false);
        }
    }

    void ActivateShards()
    {
        foreach (GameObject shard in Shards)
        {
            shard.SetActive(true);
            Rigidbody rb = shard.GetComponent<Rigidbody>();
            rb.isKinematic = false;
            rb.useGravity = false;
            rb.AddExplosionForce(50.0f, transform.position, 30);
            StartCoroutine(DestroyShardCoroutine(shard));
        }
    }
}