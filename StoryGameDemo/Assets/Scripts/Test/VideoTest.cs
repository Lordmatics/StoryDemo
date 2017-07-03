using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

[AddComponentMenu("Scripts/Test/VideoTest")]
[RequireComponent(typeof(AudioSource))]
public class VideoTest : MonoBehaviour
{

    [SerializeField]
    private RawImage image;

    [SerializeField]
    private VideoPlayer videoPlayer;

    [SerializeField]
    private AudioSource audioSource;

    //[SerializeField]
    //private VideoSource videoSource;

    //[SerializeField]
    //private string videoURL;

    [SerializeField]
    private VideoClip videoClip;


    //public MovieTexture movie;

    void Start()
    {
        //videoPlayer = gameObject.AddComponent<UnityEngine.Video.VideoPlayer>();
        videoPlayer.clip = videoClip;// AssetDatabase.LoadAssetAtPath<UnityEngine.Video.VideoClip>("Assets/BigBuckBunny.mp4");
        videoPlayer.isLooping = true;
        
        //videoPlayer.target = UnityEngine.Video.VideoTarget.CameraFrontPlane;
        //videoPlayer.alpha = 0.5f;
        videoPlayer.loopPointReached += EndReached;
        videoPlayer.Play();
    }
    void EndReached(UnityEngine.Video.VideoPlayer vPlayer)
    {
        Debug.Log("End reached!");
    }

    void Update()
    {
        Debug.Log("Frame " + videoPlayer.frame);
    }

 //   void Start ()
 //   {
 //       Application.runInBackground = true;
 //       videoPlayer.clip = videoClip;
 //       image.texture = videoPlayer.texture;
 //       audioSource = GetComponent<AudioSource>() == null ? gameObject.AddComponent<AudioSource>() : GetComponent<AudioSource>();
 //       audioSource.playOnAwake = false;
 //       videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
 //       videoPlayer.EnableAudioTrack(0, true);
 //       videoPlayer.SetTargetAudioSource(0, audioSource);

 //       videoPlayer.Play();
 //       audioSource.Play();
 //       //StartCoroutine(PlayVideo());
	//}
	
    //public IEnumerator PlayVideo()
    //{
        //WWW www = new WWW(videoURL);
        //videoPlayer = gameObject.AddComponent<VideoPlayer>();
        //audioSource = gameObject.AddComponent<AudioSource>();

        //videoPlayer.playOnAwake = false;
        //audioSource.playOnAwake = false;
        //audioSource.Pause();

        //videoPlayer.source = VideoSource.VideoClip;
        //videoPlayer.clip = videoClip;
        //videoPlayer.Play();
        //yield return null;
        //videoPlayer.url = videoURL;// www.url;

        //videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;

        //videoPlayer.EnableAudioTrack(0, true);
        //videoPlayer.SetTargetAudioSource(0, audioSource);

        //videoPlayer.Prepare();

        //WaitForSeconds waitTime = new WaitForSeconds(1);
        //while(!videoPlayer.isPrepared)
        //{
        //    Debug.Log("Preparing Video");
        //    yield return waitTime;
        //    break;
        //}
        //Debug.Log("Finished Preparations");

        //image.texture = videoPlayer.texture;

        //videoPlayer.Play();

        //audioSource.Play();
    //}

}
