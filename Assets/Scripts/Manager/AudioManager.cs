using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Cấu hình AudioSource")]
    [SerializeField] private AudioSource musicSource; // Dành cho nhạc nền (loop)
    [SerializeField] private AudioSource sfxSource;   // Dành cho hiệu ứng (oneshot)

    [Header("Đường dẫn Resources")]
    // Theo hình ảnh của bạn: Assets/Resources/Audio -> chỉ cần điền "Audio"
    [SerializeField] private string audioFolderPath = "Audio";

    private Dictionary<string, AudioClip> audioClipDict = new Dictionary<string, AudioClip>();

    private void Awake()
    {
        // Thiết lập Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadResources();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Tự động phát file "music" khi bắt đầu game
        PlayMusic("music");
    }

    // Tải toàn bộ clip từ Assets/Resources/Audio
    private void LoadResources()
    {
        AudioClip[] clips = Resources.LoadAll<AudioClip>(audioFolderPath);
        foreach (var clip in clips)
        {
            if (!audioClipDict.ContainsKey(clip.name))
            {
                audioClipDict.Add(clip.name, clip);
                Debug.Log($"[AudioManager] Đã tải thành công: {clip.name}");
            }
        }
    }
    // Thêm vào trong class AudioManager
    public void SetMusicVolume(float volume)
    {
        if (musicSource != null) musicSource.volume = volume;
    }

    public void SetSFXVolume(float volume)
    {
        if (sfxSource != null) sfxSource.volume = volume;
    }
    /// <summary>
    /// Phát nhạc nền lặp lại (Ví dụ: PlayMusic("music"))
    /// </summary>
    public void PlayMusic(string fileName)
    {
        if (audioClipDict.TryGetValue(fileName, out AudioClip clip))
        {
            if (musicSource.clip == clip) return; // Đang phát rồi thì bỏ qua

            musicSource.clip = clip;
            musicSource.loop = true;
            musicSource.Play();
        }
        else
        {
            Debug.LogWarning($"[AudioManager] Không tìm thấy file nhạc: {fileName}");
        }
    }

    /// <summary>
    /// Phát hiệu ứng âm thanh một lần (Ví dụ: PlaySFX("click1"))
    /// </summary>
    public void PlaySFX(string fileName)
    {
        if (audioClipDict.TryGetValue(fileName, out AudioClip clip))
        {
            sfxSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"[AudioManager] Không tìm thấy SFX: {fileName}");
        }
    }

    // Các hàm hỗ trợ điều khiển âm lượng từ SettingControl
    public void SetMusicMute(bool isMuted) => musicSource.mute = isMuted;
    public void SetSFXMute(bool isMuted) => sfxSource.mute = isMuted;
}