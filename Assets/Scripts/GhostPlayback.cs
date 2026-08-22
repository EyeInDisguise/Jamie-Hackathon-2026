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
        if (!isPlaying || frames == null || frames.Count < 2)
        {
            return;
        }

        playbackTime += Time.deltaTime;

        // Frame times are treated relative to the first recorded frame
        float firstFrameTime = frames[0].time;

        // Find the two frames surrounding the current playback time
        while (
            frameIndex < frames.Count - 2 &&
            frames[frameIndex + 1].time - firstFrameTime <= playbackTime
        )
        {
            frameIndex++;
        }

        GhostRecorder.GhostFrame currentFrame = frames[frameIndex];
        GhostRecorder.GhostFrame nextFrame = frames[frameIndex + 1];

        float currentTime = currentFrame.time - firstFrameTime;
        float nextTime = nextFrame.time - firstFrameTime;

        // Work out how far we are between the two recorded frames
        float frameDuration = nextTime - currentTime;

        float t = 0f;

        if (frameDuration > 0f)
        {
            t = (playbackTime - currentTime) / frameDuration;
        }

        t = Mathf.Clamp01(t);

        // Smoothly move between recorded positions
        transform.position = Vector3.Lerp(
            currentFrame.position,
            nextFrame.position,
            t
        );

        // Stop when the recorded run finishes
        float finalTime =
            frames[frames.Count - 1].time - firstFrameTime;

        if (playbackTime >= finalTime)
        {
            transform.position = frames[frames.Count - 1].position;
            isPlaying = false;
        }
    }

    public void StartPlayback()
    {
        // No #1 ghost has been saved yet
        if (bestFrames == null || bestFrames.Count < 2)
        {
            Debug.Log("No best ghost available");
            return;
        }

        // Copy the saved best run
        frames = new List<GhostRecorder.GhostFrame>(bestFrames);

        // Always start from the beginning
        playbackTime = 0f;
        frameIndex = 0;
        isPlaying = true;

        transform.position = frames[0].position;

        float ghostDuration =
            frames[frames.Count - 1].time - frames[0].time;

        Debug.Log(
            "Playing best ghost: " +
            frames.Count +
            " frames, duration: " +
            ghostDuration +
            " seconds"
        );
    }

    public void SaveBestGhost(List<GhostRecorder.GhostFrame> newFrames)
    {
        if (newFrames == null || newFrames.Count < 2)
        {
            Debug.LogWarning("Tried to save an empty ghost");
            return;
        }

        // Copy the finished run so the next recording can't erase it
        bestFrames =
            new List<GhostRecorder.GhostFrame>(newFrames);

        float ghostDuration =
            bestFrames[bestFrames.Count - 1].time -
            bestFrames[0].time;

        Debug.Log(
            "New #1 ghost saved: " +
            bestFrames.Count +
            " frames, duration: " +
            ghostDuration +
            " seconds"
        );
    }

    public void StopPlayback()
    {
        isPlaying = false;
        playbackTime = 0f;
        frameIndex = 0;

        // Put ghost back at the beginning
        if (bestFrames != null && bestFrames.Count > 0)
        {
            transform.position = bestFrames[0].position;
        }
    }
}