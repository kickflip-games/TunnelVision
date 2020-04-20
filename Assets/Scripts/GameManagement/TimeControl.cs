/*
 *Written by Avi Vajpeyi
*/

using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Assets.Generation;
using Assets;
using TMPro;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


public class TimeControl : MonoBehaviour
{
    public RectTransform TimeBar;
    public float EnergyLeft = 100;
    public float EnergyUsage = 8;
    public bool Using;
    private bool WasPressed;
    public Camera View;

    public TMP_Text Score, ScoreCenter;
    private float _score;
    public bool Lost = true; //To simulate the start menu
    public TMP_Text GameOver;
    public TMP_Text Title;
    public TMP_Text RestartBtn, StartBtn;


    public GameObject PlayerPrefab;
    public AudioSource Sound;
    public TMP_Text InvertTxt;
    public Image InvertCheck;
    public Toggle Invert;
    private AircraftMovement _aircraftMovement;
    private float _targetGameOver;
    private float _targetRestart;
    private float _targetScore;
    private float _targetStart;
    private float _targetTitle;
    private float _targetPitch = 1;
    private float _targetInvert;


    private Vignette _vignette;
    private Bloom _bloom;
    private ChromaticAberration _chromaticAberration;

    private Volume _postProcVolume;

    void SetPostProcessingEffects()
    {
        Vignette vTmp;
        Bloom bTemp;
        ChromaticAberration cTemp;
        if( _postProcVolume.profile.TryGet<Vignette>( out vTmp) )
        {
            _vignette = vTmp;
        }
        if( _postProcVolume.profile.TryGet<Bloom>( out bTemp) )
        {
            _bloom = bTemp;
        }
        if( _postProcVolume.profile.TryGet<ChromaticAberration>( out cTemp) )
        {
            _chromaticAberration = cTemp;
        }
        
    }

    void Start()
    {
        View = FindObjectOfType<Camera>();
        _postProcVolume = FindObjectOfType<Volume>();
        SetPostProcessingEffects();

        Lost = true;
        Time.timeScale = .25f;
        _targetStart = 1;
        _targetTitle = 1;
        StartCoroutine(PlayAnim());
    }

    public void Lose()
    {
        Lost = true;
        Time.timeScale = .25f;
        _targetGameOver = 1f;
        _targetScore = 1f;
        StartCoroutine(LostCoroutine());
    }

    IEnumerator LostCoroutine()
    {
        yield return new WaitForSeconds(3 / (1 / Time.timeScale));
        _targetGameOver = 0;
        while (Lost)
        {
            yield return new WaitForSeconds(1 / (1 / Time.timeScale));
            _targetRestart = 1;
            yield return new WaitForSeconds(1 / (1 / Time.timeScale));
            _targetRestart = 0;
        }
    }

    IEnumerator PlayAnim()
    {
        while (Lost)
        {
            yield return new WaitForSeconds(.5f / (1 / Time.timeScale));
            _targetStart = 1;
            yield return new WaitForSeconds(.5f / (1 / Time.timeScale));
            _targetStart = 0;
        }
    }

    public void StartGame()
    {
        Restart();
    }

    public void Restart()
    {
        Debug.Log("Start Game");
        if (!Lost)
            return;
        StartCoroutine(RestartCoroutine());
    }

    IEnumerator RestartCoroutine()
    {
        _targetStart = 0;
        _targetRestart = 0;
        _targetTitle = 0;
        Lost = false;
        Time.timeScale = 1f;
        _score = 0;
        _targetScore = 0;
        EnergyLeft = 100;
        Destroy(GameObject.FindGameObjectWithTag("Player"));

        OpenSimplexNoise.Load(Random.Range(int.MinValue, int.MaxValue));

        World world = GameObject.FindGameObjectWithTag("World").GetComponent<World>();
        Chunk[] chunks = null;
        lock (world.Chunks)
            chunks = world.Chunks.Values.ToList().ToArray();

        for (int i = 0; i < chunks.Length; i++)
            world.RemoveChunk(chunks[i]);

        GameObject go =
            Instantiate<GameObject>(PlayerPrefab, Vector3.zero, Quaternion.identity);
        world.Player = go;
        _aircraftMovement = go.GetComponentInChildren<AircraftMovement>();
        go.GetComponent<AircraftMaster>().timeControl = this.GetComponent<TimeControl>();
        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<FollowShip>()
            .PlayerGameObject = go;

        yield return null;
    }

    void UpdateScoreUI()
    {
        Score.text = ((int) _score).ToString();
        ScoreCenter.text = Score.text;
    }


    void AdjustCanvasColours()
    {
        _targetInvert = Mathf.Min(1, _targetScore + _targetTitle);
        Sound.pitch = Mathf.Lerp(Sound.pitch, _targetPitch, Time.deltaTime * 8f);
        Title.color = new Color(Title.color.r, Title.color.g, Title.color.b,
            Mathf.Lerp(Title.color.a, _targetTitle,
                Time.deltaTime * 4f * (1 / Time.timeScale)));
        StartBtn.color = new Color(StartBtn.color.r, StartBtn.color.g, StartBtn.color.b,
            Mathf.Lerp(StartBtn.color.a, _targetStart,
                Time.deltaTime * 4f * (1 / Time.timeScale)));
        GameOver.color = new Color(GameOver.color.r, GameOver.color.g, GameOver.color.b,
            Mathf.Lerp(GameOver.color.a, _targetGameOver,
                Time.deltaTime * 4f * (1 / Time.timeScale)));
        RestartBtn.color = new Color(RestartBtn.color.r, RestartBtn.color.g,
            RestartBtn.color.b,
            Mathf.Lerp(RestartBtn.color.a, _targetRestart,
                Time.deltaTime * 4f * (1 / Time.timeScale)));
        Invert.targetGraphic.color = new Color(Invert.targetGraphic.color.r,
            Invert.targetGraphic.color.g, Invert.targetGraphic.color.b,
            Mathf.Lerp(Invert.targetGraphic.color.a, _targetInvert,
                Time.deltaTime * 4f * (1 / Time.timeScale)));
        InvertTxt.color = new Color(InvertTxt.color.r, InvertTxt.color.g,
            InvertTxt.color.b,
            Mathf.Lerp(InvertTxt.color.a, _targetInvert,
                Time.deltaTime * 4f * (1 / Time.timeScale)));
        InvertCheck.color = new Color(InvertCheck.color.r, InvertCheck.color.g,
            InvertCheck.color.b,
            Mathf.Lerp(InvertCheck.color.a, _targetInvert,
                Time.deltaTime * 4f * (1 / Time.timeScale)));
        if (_targetTitle != 1)
        {
            Score.color = new Color(Score.color.r, Score.color.g, Score.color.b,
                Mathf.Lerp(Score.color.a, 1 - _targetScore,
                    Time.deltaTime * 2f * (1 / Time.timeScale)));
            ScoreCenter.color = new Color(ScoreCenter.color.r, ScoreCenter.color.g,
                ScoreCenter.color.b,
                Mathf.Lerp(ScoreCenter.color.a, _targetScore,
                    Time.deltaTime * 2f * (1 / Time.timeScale)));
        }
    }


    void ActivateDeactivateSlowMo()
    {
        if (Input.GetKey(KeyCode.Space) && EnergyLeft > 0 && !WasPressed)
        {
            EnergyLeft -= Time.deltaTime * EnergyUsage * (1 / Time.timeScale);
            EnergyLeft = Mathf.Clamp(EnergyLeft, 0, 100);
            Using = true;
        }
        else
        {
            EnergyLeft += Time.deltaTime * EnergyUsage * .5f;
            EnergyLeft = Mathf.Clamp(EnergyLeft, 0, 100);
            Using = false;
        }

        TimeBar.sizeDelta = Lerp(TimeBar.sizeDelta,
            new Vector2(EnergyLeft - .5f, TimeBar.sizeDelta.y), Time.deltaTime * 6f);
    }

    void AdjustSlowMoFX()
    {
        if (Using)
        {
            Time.timeScale = .35f;
            _targetPitch = .5f;
            _bloom.active = true;
            _vignette.active = false;
            _chromaticAberration.active = false;

        }
        else
        {
            Time.timeScale = 1f;
            _targetPitch = 1f;
            
            _bloom.active = false;
            _vignette.active = true;
            _chromaticAberration.active = true;

        }
    }


    void AdjustDifficultyFromScore()
    {
        if (_score < 125)
            _aircraftMovement.Speed = 12;
        else if (_score < 275)
            _aircraftMovement.Speed = 14;
        else if (_score < 500)
            _aircraftMovement.Speed = 16;
        else if (_score < 1000)
            _aircraftMovement.Speed = 18;
    }

    void UpdateScore()
    {
        if (!_aircraftMovement.IsInSpawn)
            _score += Time.deltaTime * 8;
    }

    void Update()
    {
        if (Lost && Input.GetKeyDown(KeyCode.Space))
            Restart();

        UpdateScoreUI();
        AdjustCanvasColours();


        if (Lost)
            return;
        
        ActivateDeactivateSlowMo();
        AdjustSlowMoFX();


        if (!Using)
            WasPressed = Input.GetKey(KeyCode.Space);

        UpdateScore();
        AdjustDifficultyFromScore();
    }

    public void InvertControls()
    {
        Options.Invert = !Options.Invert;
        Invert.isOn = Options.Invert;
    }

    Vector2 Lerp(Vector2 a, Vector2 b, float d)
    {
        return new Vector2(Mathf.Lerp(a.x, b.x, d), Mathf.Lerp(b.x, b.y, d));
    }

    public void QuitGame()
    {
        Debug.Log("QUIT");
        Application.Quit();
    }

}