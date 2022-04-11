using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkullAnimator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Sprite hoverSprite;
    [SerializeField] private Sprite mouseDownSprite;

    private Image _image;
    private bool _over;
    private bool _down;

    // Start is called before the first frame update
    void Start()
    {
        var instanceId = gameObject.GetInstanceID();
        _image = GetComponentsInChildren<Image>().First(i => i.gameObject.GetInstanceID() != instanceId);
        _image.sprite = defaultSprite;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _image.sprite = _down ? mouseDownSprite : hoverSprite;
        _over = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _image.sprite = defaultSprite;
        _over = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _image.sprite = _over ? mouseDownSprite : defaultSprite;
        _down = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _image.sprite = _over ? hoverSprite : defaultSprite;
        _down = false;
    }
}
