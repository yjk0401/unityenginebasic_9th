using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using System.Linq; //collection 의 다양한 질의(Query) 기능들을 포함하는 네임스페이스

[RequireComponent(typeof(VideoPlayer))]

public class MusicPlayManager : MonoBehaviour
{
    public static MusicPlayManager instance;

    public float noteFallingDistance => _spawnerCenter.position.y - _hitterCenter.position.y;
    public float noteFallingTime => noteFallingDistance / speedGain;

    public float speedGain = 1.0f;

    public bool isPlaying;
    private VideoPlayer _videoPlayer;
    private Queue<NoteData> _queue;
    private float _timeMark;

    private Dictionary<KeyCode, NoteSpawner> _spawners;
    [SerializeField] private List<NoteSpawner> _spawnerList;
    [SerializeField] private Transform _spawnerCenter;
    [SerializeField] private Transform _hitterCenter;

    public const int POINT_COOL = 500;
    public const int POINT_GREAT = 300;
    public const int POINT_GOOD = 100;
    public const int POINT_MISS = 0;
    public const int POINT_BAD = -100;

    public string rank 
    {
        get 
        {
            if ((float)score / scoreMax >= 1.0f) return "PERPECT";
            else if ((float)score / scoreMax >= 0.95) return "S";
            else if ((float)score / scoreMax >= 0.85) return "A";
            else if ((float)score / scoreMax >= 0.75) return "B";
            else if ((float)score / scoreMax >= 0.65) return "D";
            else if ((float)score / scoreMax >= 0.55) return "E";
            else return "Fail";
        }
    }

    public int score 
    {
        get => _score;
        set 
        {
            _score = value;
            _scoreText.score = value;
        }
    }
    private int _score;
    [SerializeField] private ScoringText _scoreText;
    public int scoreMax;
    

    public int combo
    {
        get => _combo;
        set 
        {
            if (highestCombo < value)
                highestCombo = value;

             _combo = value;
            _popUpTextManager.PopUpComboText(value);
        }
    }
    private int _combo;
    public int highestCombo;
    public int coolCount 
    {
        get => _coolCount;
        set 
        {
            score += (value - _coolCount) * POINT_COOL;
            combo += (value - _coolCount);
            _coolCount = value;
            _popUpTextManager.PopUpHitJudgeText(HitJudge.Cool);
        }
    }
    public int greatCount
    {
        get => _greatCount;
        set
        {
            score += (value - _greatCount) * POINT_GREAT;
            combo += (value - _greatCount);
            _greatCount = value;
            _popUpTextManager.PopUpHitJudgeText(HitJudge.Great);
        }
    }
    public int goodCount
    {
        get => _goodCount;
        set
        {
            score += (value - _goodCount) * POINT_GOOD;
            combo += (value - _goodCount);
            _goodCount = value;
            _popUpTextManager.PopUpHitJudgeText(HitJudge.Good);
        }
    }
    public int missCount
    {
        get => _missCount;
        set
        {
            score += (value - _missCount) * POINT_MISS;
            combo = 0;
            _missCount = value;
            _popUpTextManager.PopUpHitJudgeText(HitJudge.Miss);
        }
    }
    public int badCount
    {
        get => _badCount;
        set
        {
            score += (value - _badCount) * POINT_BAD;
            combo = 0;
            _badCount = value;
            _popUpTextManager.PopUpHitJudgeText(HitJudge.Bad);
        }
    }
    private int _coolCount;
    private int _greatCount;
    private int _goodCount;
    private int _missCount;
    private int _badCount;

    [SerializeField] private PopUpTextManager _popUpTextManager;
    [SerializeField] private ResultUi _resultUi;

    private void Awake()
    {
        instance = this;
        _videoPlayer = GetComponent<VideoPlayer>();
        _spawners = new Dictionary<KeyCode, NoteSpawner>();
        foreach (var spawner in _spawnerList)
        {
            _spawners.Add(spawner.key , spawner);
        }
    }

    public void StartMusicPlay()
    {
        _queue = new Queue<NoteData>(SongDataLoader.dataLoaded.noteDatum.OrderBy(x => x.time));
        scoreMax = _queue.Count * POINT_COOL;
        highestCombo = 0;
        combo = 0;
        score = 0;
        coolCount = 0;
        greatCount = 0;
        goodCount = 0;
        missCount = 0;
        badCount = 0;

        _videoPlayer.clip = SongDataLoader.clipLoaded;
        Invoke("PlayVideo", noteFallingTime);
        _timeMark = Time.time;
        isPlaying = true;   
    }

    private void Update()
    {
        if (isPlaying == false)
            return;

        while (_queue.Count > 0) 
        {
            if (_queue.Peek().time <= Time.time - _timeMark)
            {
                _spawners[_queue.Dequeue().key].Spawn();
            }
            else
            {
                break;
            }
        }


        if (_queue.Count <= 0) 
        {
            isPlaying = false;
            Invoke("Finish", noteFallingTime + 2.0f);
        }
    }

    private void PlayVideo() 
    {
        _videoPlayer.Play();
    }

    private void Finish() 
    {
        _videoPlayer.Stop();
        _resultUi.gameObject.SetActive(true);
    }
}
