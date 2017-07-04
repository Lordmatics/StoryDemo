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

    public bool bCanInteractWithVideo = false;
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
        DisableVideo();
    }

    void Start ()
    {
        Application.runInBackground = true;
        image = GetComponent<RawImage>();
        audioSource = GetComponent<AudioSource>();
        videoPlayer = GetComponent<VideoPlayer>();
        videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
        videoPlayer.EnableAudioTrack(0, true);
        videoPlayer.SetTargetAudioSource(0, audioSource);

        DisableVideo();
        //StartCoroutine(PlayVideoAt("Videos/Test/TestA", OnVideoEnd));

        //double length = videoPlayer.clip.length;
        //Debug.Log("Length Of Video: " + length + "s");
    }

    public void EnableVideo()
    {
        image.enabled = true;
    }

    public void DisableVideo()
    {
        image.enabled = false;
    }

    public IEnumerator PlayVideoAt(string filePath, Action Callback, bool bAutoClose = false)
    {
        if(bAutoClose)
            Callback += OnVideoEnd;

        //"Videos/Test/Video"
        videoPlayer.Prepare();

        WaitForSeconds waitTime = new WaitForSeconds(2); // Might need to make this 5. Video can get stuck bufferring
        while (!videoPlayer.isPrepared)
        {
            Debug.Log("Preparing Video");
            yield return waitTime;
            break;
        }
        Debug.Log("Finished Preparations");

        EnableVideo();
        videoPlayer.clip = (VideoClip)Resources.Load(filePath);
        videoPlayer.Play();
        StartCoroutine(OnVideoFinished(Callback, bAutoClose));
        audioSource.Play();
        RenderTexture texture = (RenderTexture)Resources.Load("Videos/Test/VideoRT");
        //UnityEngine.RenderTexture.active = texture;
        //GL.Clear(true, true, Color.clear);
        //RenderTexture.active = rt;
        SetRenderTexture(texture);
        SetAlpha(1.0f);
        yield break;
    }

    private IEnumerator OnVideoFinished(Action Callback, bool bAutoClose)
    {
        while(videoPlayer.isPlaying || bIsPaused)
        {
            yield return 0;
        }
        if(Callback != null)
        {
            Callback();
            if(bAutoClose)
                Callback -= OnVideoEnd;
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
        // Warning - > Edit -> Graphics Emulation -> No Emulation isntead of OpenGL3
        //Camera camera = Camera.main;
        //int width = camera.pixelWidth;
        //int height = camera.pixelHeight;
        //RenderTexture rt = new RenderTexture(width, height, 32);
        //videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        //videoPlayer.targetTexture = texture;
        image.texture = texture;
        //texture.DiscardContents();
    }

    public void SetAlpha(float alpha)
    {
        Color curColor = image.color;
        curColor.a = alpha;
        image.color = curColor;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(bCanInteractWithVideo)
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


