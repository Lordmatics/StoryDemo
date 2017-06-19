using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.EventSystems;


// http://video.online-convert.com/convert-to-mp4
// Converter to MP4. .flv didn't work, but mp4 does

[AddComponentMenu("Scripts/Video/AssignRenderTexture")]
[RequireComponent(typeof(RawImage))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(VideoPlayer))]
public class AssignCurrentRenderTexture : MonoBehaviour, IPointerDownHandler
{
    private RawImage image;
    private AudioSource audioSource;
    private VideoPlayer videoPlayer;

    public static AssignCurrentRenderTexture instance;

    public Action OnVideoEnd;

    bool bIsPaused = false;
    bool bFinishedCurrentClip = false;

    private void Awake()
    {
        instance = this;
    }

    private void OnEnable()
    {
        OnVideoEnd += Finished;    
    }

    private void OnDisable()
    {
        OnVideoEnd -= Finished;   
    }

    void Finished()
    {
        Debug.Log("Finished");
        bFinishedCurrentClip = true;
        videoPlayer.Stop();
        audioSource.Stop();
        SetAlpha(0.0f);
    }

    void Start ()
    {
        image = GetComponent<RawImage>();
        audioSource = GetComponent<AudioSource>();
        videoPlayer = GetComponent<VideoPlayer>();
        videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
        videoPlayer.EnableAudioTrack(0, true);
        videoPlayer.SetTargetAudioSource(0, audioSource);


        PlayVideoAt("Videos/Test/TestA");
        //double length = videoPlayer.clip.length;
        //Debug.Log("Length Of Video: " + length + "s");
    }

    public void PlayVideoAt(string filePath)
    {
        //"Videos/Test/Video"
        videoPlayer.clip = (VideoClip)Resources.Load(filePath);
        videoPlayer.Play();
        StartCoroutine(OnVideoFinished(OnVideoEnd));
        audioSource.Play();
        RenderTexture texture = (RenderTexture)Resources.Load("Videos/Test/VideoRT");
        SetRenderTexture(texture);
        SetAlpha(1.0f);
    }

    private IEnumerator OnVideoFinished(Action Callback)
    {
        while(videoPlayer.isPlaying || bIsPaused)
        {
            yield return 0;
        }
        if(Callback != null)
        {
            Callback();
        }
        yield break;
    }
    void ChangeVid()
    {
        videoPlayer.Stop();
        audioSource.Stop();
        SetAlpha(0.0f);
        videoPlayer.clip = (VideoClip)Resources.Load("Videos/Test/Video2");
        videoPlayer.Play();
        audioSource.Play();
        SetAlpha(1.0f);
    }

    public void SetRenderTexture(RenderTexture texture)
    {
        image.texture = texture;
    }

    public void SetAlpha(float alpha)
    {
        Color curColor = image.color;
        curColor.a = alpha;
        image.color = curColor;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        PauseOrResume();
    }

    void PauseOrResume()
    {
        switch(bIsPaused)
        {
            case true:
                Resume();
                break;
            case false:
                Pause();
                break;
        }
    }

    void Resume()
    {
        bIsPaused = false;
        videoPlayer.Play();
    }

    void Pause()
    {
        // If you click on video, once its finished, it will play from beginning
        if (bFinishedCurrentClip)
        {
            SetAlpha(1.0f);
            Resume();
            return;
        }
        bIsPaused = true;
        videoPlayer.Pause();
    }
}


