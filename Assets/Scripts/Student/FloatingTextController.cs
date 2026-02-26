using TMPro;
using UnityEngine;
using System.Collections;

public class FloatingTextController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textAmount;
    [SerializeField] private float _lifeTime;

    public void Setup(int amount)
    {
        _textAmount.text = "+" + amount.ToString();
        Destroy(gameObject, _lifeTime);
        StartCoroutine(PopUpAnimation());
    }
    private IEnumerator PopUpAnimation()
    {
        Vector3 originalScale = transform.localScale;
        transform.localScale = Vector3.zero; 

        float timer = 0;
        while (timer < 0.25f)
        {
            timer += Time.deltaTime;
            transform.localScale = Vector3.Lerp(Vector3.zero, originalScale, timer / 0.25f);
            yield return null;
        }
        transform.localScale = originalScale;
    }
}
