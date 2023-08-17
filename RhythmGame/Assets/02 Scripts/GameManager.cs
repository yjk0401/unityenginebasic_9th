using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public enum State 
    {
        Idle,
        LoadSongData,
        LoadSongDataLoaded,
        StartPlay,
        WaitUntilPlayFinished,
        DisplayScore,
        WaitForUser,
    }

    public State state;

    private void Awake()
    {
        if (Instance != null) 
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        switch (state)
        {
            case State.Idle:
                break;
            case State.LoadSongData:
                {
                    SongDataLoader.Load(SongSelection.s_selected);
                    state = State.LoadSongDataLoaded;
                }
                break;
            case State.LoadSongDataLoaded:
                {
                    if (SongDataLoader.isLoaded) 
                    {
                        SceneManager.LoadScene("MusicPlay");
                        state = State.StartPlay;
                    }
                }
                break;
            case State.StartPlay:
                {
                    if (MusicPlayManager.instance != null) 
                    {
                        MusicPlayManager.instance.StartMusicPlay();
                        state = State.WaitUntilPlayFinished;
                    }
                }
                break;
            case State.WaitUntilPlayFinished:
                break;
            case State.DisplayScore:
                break;
            case State.WaitForUser:
                break;
            default:
                break;
        }
    }

}
