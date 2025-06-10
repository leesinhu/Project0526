using TMPro;
using UnityEngine;

public class TextUI : MonoBehaviour
{
    public TextMeshProUGUI messageText;  // ���� ������ �ؽ�Ʈ
    public RectTransform background;     // ��� (Panel)

    [SerializeField] float maxWidth = 300f;  // �ִ� �ʺ� ����

    private Vector2 padding = new Vector2(5f, 5f); // �¿� ���� (X: �¿�, Y: ����)

    void Start()
    {
        maxWidth = messageText.gameObject.GetComponent<RectTransform>().sizeDelta.x;
    }

    private void Update()
    {
        // �ؽ�Ʈ ũ�� ��������
        Vector2 textSize = messageText.GetPreferredValues();

        // ��� ũ�� ���� (�ؽ�Ʈ ũ�� + �е� ����)
        float width = Mathf.Min(textSize.x, maxWidth) + 50f;
        background.sizeDelta = new Vector2(width, textSize.y + padding.y);
    }
}
