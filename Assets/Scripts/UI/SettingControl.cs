using UnityEngine;
using UnityEngine.UI;

public class SettingControl : MonoBehaviour
{
    [Header("Âm nhạc (Music)")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Toggle musicToggle;

    [Header("Âm thanh (SFX)")]
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Toggle sfxToggle;

    [Header("Nút chức năng")]
    [SerializeField] private Button closeButton;

    private void Start()
    {
        // 1. Tải giá trị đã lưu từ PlayerPrefs (mặc định là 1 nếu chưa có)
        float savedMusicVol = PlayerPrefs.GetFloat("MusicVolume", 1f);
        float savedSFXVol = PlayerPrefs.GetFloat("SFXVolume", 1f);
        bool isMusicOn = PlayerPrefs.GetInt("MusicMute", 1) == 1;
        bool isSFXOn = PlayerPrefs.GetInt("SFXMute", 1) == 1;

        // 2. Cập nhật giao diện UI
        if (musicSlider != null) musicSlider.value = savedMusicVol;
        if (sfxSlider != null) sfxSlider.value = savedSFXVol;
        if (musicToggle != null) musicToggle.isOn = isMusicOn;
        if (sfxToggle != null) sfxToggle.isOn = isSFXOn;

        // 3. Gán sự kiện cho các thanh trượt
        if (musicSlider != null) musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        if (sfxSlider != null) sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);

        // 4. Gán sự kiện cho các nút Toggle (Bật/Tắt nhanh)
        if (musicToggle != null) musicToggle.onValueChanged.AddListener(OnMusicToggleChanged);
        if (sfxToggle != null) sfxToggle.onValueChanged.AddListener(OnSFXToggleChanged);

        // 5. Nút đóng
        if (closeButton != null) closeButton.onClick.AddListener(OnCloseClick);

        // Cập nhật âm lượng ban đầu cho AudioManager
        UpdateAudioManager();
    }

    private void OnMusicVolumeChanged(float value)
    {
        PlayerPrefs.SetFloat("MusicVolume", value);
        UpdateAudioManager();
    }

    private void OnSFXVolumeChanged(float value)
    {
        PlayerPrefs.SetFloat("SFXVolume", value);
        UpdateAudioManager();
    }

    private void OnMusicToggleChanged(bool isOn)
    {
        PlayerPrefs.SetInt("MusicMute", isOn ? 1 : 0);
        UpdateAudioManager();
    }

    private void OnSFXToggleChanged(bool isOn)
    {
        PlayerPrefs.SetInt("SFXMute", isOn ? 1 : 0);
        UpdateAudioManager();
    }

    private void UpdateAudioManager()
    {
        if (AudioManager.Instance == null) return;

        // Lấy giá trị hiện tại
        float mVol = PlayerPrefs.GetFloat("MusicVolume", 1f);
        float sVol = PlayerPrefs.GetFloat("SFXVolume", 1f);
        bool mOn = PlayerPrefs.GetInt("MusicMute", 1) == 1;
        bool sOn = PlayerPrefs.GetInt("SFXMute", 1) == 1;

        // Điều khiển AudioManager (Giả sử bạn đã có các hàm này trong AudioManager)
        AudioManager.Instance.SetMusicVolume(mOn ? mVol : 0);
        AudioManager.Instance.SetSFXVolume(sOn ? sVol : 0);
    }

    private void OnCloseClick()
    {
        PlayerPrefs.Save();
        AudioManager.Instance.PlaySFX("click1"); // Phát tiếng click khi đóng
        UIManager.Instance.CloseSettingPanel();
    }
}