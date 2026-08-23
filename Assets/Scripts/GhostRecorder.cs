using System.Collections.Generic;
using UnityEngine;

public class GhostRecorder : MonoBehaviour
{
    [System.Serializable]
    public class GhostFrame
    {
        public float time;
        public Vector3 position;

        public GhostFrame(float time, Vector3 position)
        {
            this.time = time;
            this.position = position;
        }
    }

    [SerializeField] private Transform player;

    private List<GhostFrame> frames = new List<GhostFrame>();

    private bool isRecording;
    private float recordingTime;

    public List<GhostFrame> Frames => frames;

    private void FixedUpdate()
    {
        if (!isRecording) return;

        // FixedUpdate gives us evenly-spaced samples of the physics movement
        recordingTime += Time.fixedDeltaTime;

        frames.Add(new GhostFrame(
            recordingTime,
            player.position
        ));
    }

    public void StartRecording()
    {
        // New race = throw away the previous temporary recording
        frames.Clear();
        recordingTime = 0f;
        isRecording = true;
    }

    public void StopRecording()
    {
        isRecording = false;

       // Debug.Log("Ghost frames recorded: " + frames.Count);
    }
}