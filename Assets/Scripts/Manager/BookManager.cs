using System.Collections.Generic;
using UnityEngine;
[DefaultExecutionOrder(-95)] // Chạy sau SaveManager nhưng trước ClassBookController
public class BookManager : MonoBehaviour
{

    public static BookManager Instance;

    private List<BookData> BookListData;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void Initialize(List<BookData> BooksData)
    {

        if (BooksData == null) return;

        BookListData = BooksData;

    }
    public static List<BookData> GenerateDefaultBookData()
    {
        List<BookData> defaultBooks = new List<BookData>();
        for(int i =0;i<6;i++)
        {
            var newData = new BookData(
                i,
                0
            );
            defaultBooks.Add(newData);
        }
        Debug.Log($"[BookManager] Tạo dữ liệu sách mặc định, tổng số sách: {defaultBooks.Count}");
        return defaultBooks;
    }

    public List<BookData> GetBook()
    {
        return BookListData;
    }

    public void UpdateBookData(int id, int amount)
    {
        foreach(var book in BookListData)
        {
            if(book.Level == id)
            {
                book.Count += amount;
                return;
            }
        }
    }

    public void SaveChange()
    {
        SaveManager.Instance.SaveGame();
    }
}

