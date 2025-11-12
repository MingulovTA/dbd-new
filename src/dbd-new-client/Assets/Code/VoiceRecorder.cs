using System;
using System.IO;
using UnityEngine;

public class VoiceRecorder : MonoBehaviour
{ 
    private AudioClip recordingClip;
    private string microphoneDevice;
    private bool isRecording;
    private int startPosition;
    private float recordStartTime;

    private string savePath;

    // Настройки записи (можно подогнать под нужды)
    private const int sampleRate = 16000; // 16 kHz — достаточно для голоса
    private const int maxDurationSec = 60; // запас, но реальная длина будет меньше

    private string _pref;

    private void Start()
    {
        var dt = DateTime.Now;
        _pref = $"{dt.DayOfYear}{dt.Hour}{dt.Minute}{dt.Second}{dt.Millisecond}";
        // Папка для сохранений
        savePath = Path.Combine(Application.dataPath, "Recordings");
        if (!Directory.Exists(savePath))
            Directory.CreateDirectory(savePath);

        if (Microphone.devices.Length > 0)
            microphoneDevice = Microphone.devices[0];
        else
            Debug.LogError("❌ Микрофон не найден!");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
            StartRecording();

        if (Input.GetKeyUp(KeyCode.C))
            StopRecordingAndSave();
    }

    private void StartRecording()
    {
        if (isRecording || microphoneDevice == null)
            return;

        Debug.Log("🎙 Начинаю запись...");
        isRecording = true;
        recordStartTime = Time.time;

        // Стартуем запись (mono, 16 kHz, до 60 сек)
        recordingClip = Microphone.Start(microphoneDevice, false, maxDurationSec, sampleRate);
        startPosition = Microphone.GetPosition(microphoneDevice);
    }

    private void StopRecordingAndSave()
    {
        if (!isRecording)
            return;

        int endPosition = Microphone.GetPosition(microphoneDevice);
        Microphone.End(microphoneDevice);
        isRecording = false;

        float recordLength = Time.time - recordStartTime;
        Debug.Log($"⏹ Запись остановлена. Длина: {recordLength:F2} сек.");

        // Получаем реальные данные записи
        int samplesRecorded = Mathf.FloorToInt(recordingClip.frequency * recordLength);
        float[] allData = new float[samplesRecorded];
        recordingClip.GetData(allData, 0);

        // Создаём короткий клип по фактической длине
        AudioClip trimmedClip = AudioClip.Create("TrimmedRecording", samplesRecorded, 1, sampleRate, false);
        trimmedClip.SetData(allData, 0);

        SaveWavFile(trimmedClip);
    }

    private string SaveWavFile(AudioClip clip)
    {
        float[] samples = new float[clip.samples];
        clip.GetData(samples, 0);

        byte[] wavBytes = ConvertToWav(clip, samples);

        var dt = DateTime.Now;
        string n = $"{dt.DayOfYear}{dt.Hour}{dt.Minute}{dt.Second}{dt.Millisecond}";
        
        
        string fileName = $"{_pref}_{n}.wav";
        string filePath = Path.Combine(savePath, fileName);
        File.WriteAllBytes(filePath, wavBytes);

        Debug.Log($"✅ Файл сохранён: {filePath}");
        return fileName;
    }

    private byte[] ConvertToWav(AudioClip clip, float[] samples)
    {
        MemoryStream stream = new MemoryStream();
        int sampleCount = samples.Length;
        int channels = 1;
        int frequency = clip.frequency;
        int byteRate = frequency * channels * 2;

        // Заголовок WAV
        stream.Write(System.Text.Encoding.ASCII.GetBytes("RIFF"), 0, 4);
        stream.Write(BitConverter.GetBytes(36 + sampleCount * 2), 0, 4);
        stream.Write(System.Text.Encoding.ASCII.GetBytes("WAVEfmt "), 0, 8);
        stream.Write(BitConverter.GetBytes(16), 0, 4);
        stream.Write(BitConverter.GetBytes((ushort)1), 0, 2);
        stream.Write(BitConverter.GetBytes((ushort)channels), 0, 2);
        stream.Write(BitConverter.GetBytes(frequency), 0, 4);
        stream.Write(BitConverter.GetBytes(byteRate), 0, 4);
        stream.Write(BitConverter.GetBytes((ushort)(channels * 2)), 0, 2);
        stream.Write(BitConverter.GetBytes((ushort)16), 0, 2);
        stream.Write(System.Text.Encoding.ASCII.GetBytes("data"), 0, 4);
        stream.Write(BitConverter.GetBytes(sampleCount * 2), 0, 4);

        // Данные
        foreach (float s in samples)
        {
            short val = (short)(Mathf.Clamp(s, -1f, 1f) * short.MaxValue);
            stream.Write(BitConverter.GetBytes(val), 0, 2);
        }

        return stream.ToArray();
    }
}