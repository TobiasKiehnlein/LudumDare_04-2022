using System;
using System.Globalization;
using RayWallSystem;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemHandler : MonoBehaviour, IHandleInstantiation
{
    [SerializeField] private bool isPreselected;
    [SerializeField] private Sprite sprite;
    [SerializeField] private float reloadTimeInSeconds;
    [SerializeField] private int maxAmount;
    [SerializeField] [Range(0, 99)] private int currentAmount;
    [SerializeField] private KeyCode associatedKey;

    [SerializeField] private GameObject spawnPrefab;

    private TMP_Text _text;
    private Image _image;
    private Slider _slider;
    private float _timeUntilIncrement;
    private Button _button;
    private Image _activeIndicator;

    private void Start()
    {
        _button = GetComponentInChildren<Button>();
        _image = _button.gameObject.GetComponent<Image>();
        _text = GetComponentInChildren<TMP_Text>();
        _slider = GetComponentInChildren<Slider>();

        _image.sprite = sprite;
        _activeIndicator = GetComponentInChildren<ActiveIndicator>().GetComponent<Image>();

        if (isPreselected)
        {
            HandleClick();
        }
    }

    private void Update()
    {
        _timeUntilIncrement -= Time.deltaTime;
        if (_timeUntilIncrement < 0)
        {
            currentAmount++;
            _timeUntilIncrement = maxAmount == currentAmount ? float.MaxValue : reloadTimeInSeconds;
        }

        if (Input.GetKeyDown(associatedKey))
        {
            ExecuteEvents.Execute(_button.gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);
        }

        _text.text = currentAmount.ToString();
        _slider.value = 1 - _timeUntilIncrement / reloadTimeInSeconds;
        _slider.gameObject.SetActive(currentAmount != maxAmount);
    }

    public void HandleClick()
    {
        GlobalItemHandler.Instance.prefab = spawnPrefab;
        GlobalItemHandler.Instance.HandleInstantiation = this;
    }

    public void HandleInstantiation()
    {
        if (currentAmount <= 0) return;

        currentAmount--;
        _timeUntilIncrement = Math.Min(_timeUntilIncrement, reloadTimeInSeconds);
    }

    public bool ClickAllowed()
    {
        return currentAmount > 0;
    }

    public void Deactivate()
    {
        _activeIndicator.enabled = false;
    }

    public void Activate()
    {
        _activeIndicator.enabled = true;
    }
}