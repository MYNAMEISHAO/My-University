using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResearchBookController : MonoBehaviour
{
    private GameObject Carbinet;
    [SerializeField] private GameObject PointTag;
    [SerializeField] private GameObject EnergyTag;
    [SerializeField] private GameObject TaoButton1;
    [SerializeField] private GameObject TaoButton2;
    [SerializeField] private GameObject TaoButton3;

    private Dictionary<int, int> bookRate1;
    private Dictionary<int, int> bookRate2;
    private Dictionary<int, int> bookRate3;

    private int shelfCount;
    private int slotPerShelf;
    private float timer = 0f;

    // Dữ liệu tham chiếu
    private List<SubjectData> subjectDatas;
    private List<BookData> bookData = new List<BookData>();

    private void OnEnable()
    {
        GameEvents.OnResearchPointChanged += ShowPoint;
        GameEvents.OnEnergyChanged += ShowEnergy;
    }

    private void OnDisable()
    {
        GameEvents.OnResearchPointChanged -= ShowPoint;
        GameEvents.OnEnergyChanged -= ShowEnergy;
    }

    void Start()
    {
        // Khởi tạo References
        subjectDatas = SubjectManager.Instance.GetAllSubjects();
        bookData = BookManager.Instance.GetBook();
        int totalPoints = PlayerManager.Instance.GetResearchPoint();
        int totalEnergy = PlayerManager.Instance.GetEnergy();
        timer = PlayerManager.Instance.timer;

        Carbinet = transform.GetChild(0).GetChild(0).gameObject;
        shelfCount = Carbinet.transform.childCount;
        slotPerShelf = Carbinet.transform.GetChild(0).childCount;

        InitializeRates();
        DisplayCarbinet();
        ShowEnergy(totalEnergy);
        ShowPoint(totalPoints);
    }

    void Update()
    {
        timer = PlayerManager.Instance.timer;
        int minutes = Mathf.FloorToInt(timer / 60);
        int seconds = Mathf.FloorToInt(timer % 60);
        EnergyTag.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void InitializeRates()
    {
        bookRate1 = new Dictionary<int, int> { { 1, 50 }, { 2, 30 }, { 3, 15 }, { 4, 5 } };
        bookRate2 = new Dictionary<int, int> { { 2, 50 }, { 3, 30 }, { 4, 15 }, { 5, 5 } };
        bookRate3 = new Dictionary<int, int> { { 3, 50 }, { 4, 30 }, { 5, 15 }, { 6, 5 } };
    }

    // --- LOGIC THÊM SÁCH (Được gọi từ Button UI) ---

    public void AddBookFromSource(int sourceIndex)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX("click1");
        }
        Dictionary<int, int> selectedRate = sourceIndex switch
        {
            1 => bookRate1,
            2 => bookRate2,
            3 => bookRate3
        };
        int costEnergy = sourceIndex switch
        {
            1 => 1,
            2 => 5,
            3 => 10
        };
        PlayerManager.Instance.SpendEnergy(costEnergy);
        int bookLevel = GetRandomBook(selectedRate);
        int shelfIndex = bookLevel - 1;

        if (shelfIndex < shelfCount)
        {
            HandleAddBook(shelfIndex);
        }
        Debug.Log("[ResearchBookController] Thêm sách cấp " + bookLevel);
    }

    private void HandleAddBook(int shelfIndex)
    {
        Transform nganTu = Carbinet.transform.GetChild(shelfIndex);
        int emptySlot = FindEmptySlotIndex(nganTu);

        if (emptySlot < slotPerShelf)
        {
            // Hiển thị sách và cập nhật data
            nganTu.GetChild(emptySlot).gameObject.SetActive(true);
            UpdateBookData(shelfIndex, 1);

            // KIỂM TRA GỘP SÁCH NGAY TẠI ĐÂY (Thay vì chạy trong Update)
            CheckAndMerge(shelfIndex);
        }
    }

    private void CheckAndMerge(int shelfIndex)
    {
        Transform nganTu = Carbinet.transform.GetChild(shelfIndex);
        int activeBooks = 0;
        foreach (Transform slot in nganTu) if (slot.gameObject.activeSelf) activeBooks++;

        if (activeBooks >= slotPerShelf)
        {
            // 1. Reset ngăn tủ hiện tại
            foreach (Transform slot in nganTu) slot.gameObject.SetActive(false);
            UpdateBookData(shelfIndex, -slotPerShelf);

            // 2. Gộp lên cấp tiếp theo hoặc mở môn học
            if (shelfIndex < shelfCount - 1)
            {
                HandleAddBook(shelfIndex + 1); // Đệ quy để gộp liên hoàn
            }
            else
            {
                HandleAddSubject();
            }

            // 3. Hiệu ứng (Bạn có thể Instantiate Particle ở đây)
            Debug.Log($"Merge thành công tại kệ {shelfIndex}!");
        }
    }

    private void HandleAddSubject()
    {
        UIManager.Instance.ShowNewSubjectPanel();
        
    }

    // --- HELPERS ---

    public void DisplayCarbinet()
    {
        for (int i = 0; i < shelfCount; i++)
        {
            if (i >= bookData.Count) break;

            int count = bookData[i].Count;
            Transform shelf = Carbinet.transform.GetChild(i);

            for (int j = 0; j < slotPerShelf; j++)
            {
                shelf.GetChild(j).gameObject.SetActive(j < count);
            }
        }
    }

    private int FindEmptySlotIndex(Transform nganTu)
    {
        for (int i = 0; i < nganTu.childCount; i++)
        {
            if (!nganTu.GetChild(i).gameObject.activeSelf) return i;
        }
        return slotPerShelf;
    }

    private int GetRandomBook(Dictionary<int, int> d)
    {
        int random = Random.Range(1, 101);
        int val = 0;
        foreach (var entry in d)
        {
            val += entry.Value;
            if (random <= val) return entry.Key;
        }
        return 1;
    }

    public void UpdateBookData(int shelfIndex, int amount)
    {
        if (shelfIndex < bookData.Count)
        {
            BookManager.Instance.UpdateBookData(shelfIndex, amount);
            SaveManager.Instance.SaveGame();
        }
    }

    public void ShowPoint(int obj)
    {
        PointTag.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = obj.ToString();
    }

    private void ShowEnergy(int obj)
    {
        EnergyTag.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = obj.ToString() + " / " + PlayerManager.Instance.maxEnergy;
        if (obj <= 0)
        {
            TaoButton1.GetComponent<Button>().interactable = false;
            TaoButton2.GetComponent<Button>().interactable = false;
            TaoButton3.GetComponent<Button>().interactable = false;
        }
        else if (obj < 5)
        {
            TaoButton1.GetComponent<Button>().interactable = true;
            TaoButton2.GetComponent<Button>().interactable = false;
            TaoButton3.GetComponent<Button>().interactable = false;
        }
        else if (obj < 10)
        {
            TaoButton1.GetComponent<Button>().interactable = true;
            TaoButton2.GetComponent<Button>().interactable = true;
            TaoButton3.GetComponent<Button>().interactable = false;
        }
        else
        {
            TaoButton1.GetComponent<Button>().interactable = true;
            TaoButton2.GetComponent<Button>().interactable = true;
            TaoButton3.GetComponent<Button>().interactable = true;
        }
    }

    public void OnTradeClick()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX("click1");
        }
        Debug.Log("[ResearchBookController] Mở giao diện cửa hàng điểm nghiên cứu.");
        UIManager.Instance.ShowPointShopPanel();
    }

    public void OnCloseClick()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX("click1");
        }
        Debug.Log("[ResearchBookController] Đóng giao diện tủ sách nghiên cứu.");
        UIManager.Instance.CloseResearchUI();
    }

}