using System.Collections.Generic;
using UnityEngine;

public class GhostPlayback : MonoBehaviour
{
    [SerializeField] private GhostRecorder ghostRecorder;

    private List<GhostRecorder.GhostFrame> frames;
    
    private List<GhostRecorder.GhostFrame> bestFrames =
        new List<GhostRecorder.GhostFrame>();

    private bool isPlaying;
    private float playbackTime;
    private int frameIndex;

    private void Update()
    {
        if (!isPlaying || frames == null || frames.Count == 0) return;

        playbackTime += Time.deltaTime;

        // Move forward through recorded frames
        while (frameIndex < frames.Count - 1 &&
               frames[frameIndex + 1].time <= playbackTime)
        {
            frameIndex++;
        }

        transform.position = frames[frameIndex].position;

        // Stop when we reach the end
        if (frameIndex >= frames.Count - 1)
        {
            isPlaying = false;
        }
    }

    public void StartPlayback()
    {
        // No #1 ghost has been saved yet
        if (bestFrames.Count == 0)
        {
            Debug.Log("No best ghost available");
            return;
        }

        // Use the saved #1 run
        frames = new List<GhostRecorder.GhostFrame>(bestFrames);

        playbackTime = 0f;
        frameIndex = 0;
        isPlaying = true;

        transform.position = frames[0].position;

        Debug.Log("Playing best ghost: " + frames.Count + " frames");
    }

    public void SaveBestGhost(List<GhostRecorder.GhostFrame> newFrames)
    {
        // Copy the finished run so the next recording can't erase it
        bestFrames = new List<GhostRecorder.GhostFrame>(newFrames);

        Debug.Log("New #1 ghost saved: " + bestFrames.Count + " frames");
    }
    
    public void StopPlayback()
    {
        isPlaying = false;
    }
}