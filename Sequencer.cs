using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;


/// <summary>
/// Plays audio samples according to a program.
/// Totally untested & unsupported. ;)
/// </summary>
[ExecuteInEditMode]
public class Sequencer : MonoBehaviour
{
    // Assigned in the Unity Editor
    public float BeatsPerMinute = 120;
    public AudioClip[] Samples;
    public AudioMixer Mixer;
    public int[][] Program = new int[16][];

    /// <summary>
    /// True if playing, otherwise false.
    /// </summary>
    private bool _playing;
    /// <summary>
    /// A number between 0 and 1 indicating how far along the sequencer is through its steps.
    /// </summary>
    private int _currentStep = -1;
    /// <summary>
    /// The time when the next set of samples will be queued.
    /// </summary>
    private double _nextStepTime;

    public int CurrentStep => _currentStep % Program.Length;
    public void Reset() => _currentStep = -1;
    public void Play() => _playing = true;
    public void Pause() => _playing = false;

    public bool IsStopped => !_playing && _currentStep == -1;
    public bool IsPaused => !_playing && _currentStep > -1;
    public bool IsPlaying => _playing;
    
    public void Stop()
    {
        var sources = AllocateAudioSources();
        for (var i = 0; i < sources.Count; i++)
            sources[i].SetScheduledEndTime(AudioSettings.dspTime + 0.0001);
        Pause();
        Reset();
    }

    IList<AudioSource> AllocateAudioSources(int count = 0)
    {
        var audioSources = GetComponents<AudioSource>().ToList();
        var needed = count - audioSources.Count;
        if (needed <= 0) return audioSources;

        for (var i = 0; i < needed; i++)
        {
            var audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.loop = false;
            audioSources.Add(audioSource);
        }

        return audioSources;
    }

    void Update()
    {
        if (!_playing
            || AudioSettings.dspTime < _nextStepTime
            || Program == null
            || Program.Length == 0
            || Samples == null
            || Samples.Length == 0) return;
        var sources = AllocateAudioSources(Samples.Length);
        _currentStep++;
        _nextStepTime = AudioSettings.dspTime + 60 / BeatsPerMinute;
        var step = Program[_currentStep % Program.Length];
        for (var i = 0; i < step.Length; i++)
        {
            var sIndex = step[i] % Samples.Length;
            var sample = Samples[sIndex];
            var source = sources[sIndex];
            source.clip = sample;
            source.PlayScheduled(_nextStepTime);
        }
    }
}
