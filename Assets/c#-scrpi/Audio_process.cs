using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class Audio_process : MonoBehaviour
{
    string audioFilePath; // 绝对路径或相对路径（相对于项目的Assets文件夹）
    public AudioSource audioSource播放器;

    // 如果音频文件位于外部存储，可以使用UnityWebRequest来加载

    public void PlayAudio(string path)
    {
        StartCoroutine(LoadAndPlayAudio(path));
    }

    IEnumerator LoadAndPlayAudio(string path)
    {
        using UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + path, AudioType.WAV);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
            audioSource播放器.clip = clip;
            audioSource播放器.Play();
        }
        else
        {
            Debug.LogError("Failed to load audio: " + www.error);
        }
    }

    //计算音频的长度
    #region
    public float Count_audio_lenght(string TTS_input_audio)
    {
        audioFilePath = TTS_input_audio;
        // 加载音频文件
        AudioClip audioClip = LoadAudioClip(audioFilePath);
        if (audioClip != null)
        {
            // 获取音频时长（以秒为单位）
            float duration = audioClip.length;
            Debug.Log("Audio duration: " + duration + " seconds");
            return duration;
        }
        return 0;
    }

    AudioClip LoadAudioClip(string path)
    {
        // 确保路径相对于Assets文件夹
        path = Path.Combine(Application.dataPath, path);

        AudioClip clip = null;
        if (File.Exists(path))
        {
            var data = File.ReadAllBytes(path);
            clip = AudioClip.Create(Path.GetFileName(path), data.Length / 2, 1, 44100, false, OnAudioRead, OnAudioSetPosition);
        }
        return clip;
    }

    void OnAudioRead(float[] data)
    {
        // 将byte[]转换为float[]
        var fileData = File.ReadAllBytes(audioFilePath);
        for (int i = 0; i < data.Length; i++)
        {
            data[i] = (float)BitConverter.ToInt16(fileData, i * 2) / short.MaxValue;
        }
    }

    void OnAudioSetPosition(int position)
    {
        // 这里可以处理音频位置的设置，如果需要的话
    }
#endregion
}
