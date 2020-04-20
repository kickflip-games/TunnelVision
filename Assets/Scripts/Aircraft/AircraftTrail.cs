using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AircraftTrail : MonoBehaviour
{

    
    private AircraftTextureManager _textureManager;
    public TrailRenderer _trail;
    private bool IsOn = false;


    public void Start()
    {
        _textureManager = transform.root.GetComponent<AircraftTextureManager>();
        if (_trail == null)
        {
            _trail = gameObject.AddComponent<TrailRenderer>();
            _trail.widthMultiplier = .25f;
            _trail.material = _textureManager.TrailMaterial;
            _trail.time = 0.3f;
            _trail.endColor = new Color(0, 0, 0, 0);
            _trail.startColor = _textureManager.TrailColor;
        }

    }

    public void ToggleTrail(bool turnOn)
    {
        if (turnOn)
            StartTrail();
        else
            StopTrail();
    }
    

    private void StartTrail()
    {
        if (!IsOn)
        {
            IsOn = true;
            _trail.startColor = _textureManager.TrailColor;
        }

    }

    private IEnumerator DestroyTrail(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(_trail);
        _trail = null;
    }
    


    private void StopTrail()
    {
        if (_trail == null)
            return;
        
//        StartCoroutine(DestroyTrail(_trail.time + 1));
        _trail.startColor = _trail.endColor;

        IsOn = false;

        // trail.transform.parent = (_debris != null) ? _debris.transform : null;
        // Destroy(_trail.gameObject, _trail.time + 1);
        // FIXME : Destroy trail renderer as well? 
//        _trail = null;
    }
}
