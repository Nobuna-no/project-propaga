using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;

[RequireComponent(typeof(VideoPlayer))]
public class VideoController : MonoBehaviour
{
    public UnityEvent OnVideoLoadingCompleted;
    public UnityEvent OnVideoStarted;
    public UnityEvent OnVideoEndReached;
    private VideoPlayer m_videoPlayer;

    public void LoadVideo()
    {
        m_videoPlayer.Prepare();
        m_videoPlayer.prepareCompleted += VideoReadyToPlay;
    }

    public void PlayVideo()
    {
        if (!m_videoPlayer.isPrepared)
        {
            Debug.LogWarning("Video is not loaded yet, prefer calling LoadVideo first", this);
        }

        m_videoPlayer.Play();
        OnVideoStarted?.Invoke();
    }

    public void SkipVideo()
    {
        if (m_videoPlayer.isPlaying)
        {
            m_videoPlayer.Stop();
        }
    }

    void Start()
    {
        m_videoPlayer = GetComponent<VideoPlayer>();
        m_videoPlayer.loopPointReached += EndReached;
    }

    void VideoReadyToPlay(VideoPlayer source)
    {
        m_videoPlayer.prepareCompleted -= VideoReadyToPlay;
        OnVideoLoadingCompleted?.Invoke();
    }


    void EndReached(UnityEngine.Video.VideoPlayer vp)
    {
        OnVideoEndReached?.Invoke();
        m_videoPlayer.Stop();
    }
}
