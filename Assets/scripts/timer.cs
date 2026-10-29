
using UnityEngine;
using TMPro;

public class timer : MonoBehaviour
{
    public int time = 0;
    public TextMeshProUGUI _text;

    void Start()
    {
        _text.text = time.ToString();
        InvokeRepeating(nameof(AddTime), 1f, 1f);
    }

    void AddTime()
    {
        time += 1;
        _text.text = time.ToString();
    }
}
