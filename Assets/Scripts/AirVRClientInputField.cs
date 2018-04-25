/***********************************************************

  Copyright (c) 2017-2018 Clicked, Inc.

  Licensed under the MIT license found in the LICENSE file 
  in the root folder of the project.

 ***********************************************************/

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]

public class AirVRClientInputField : MonoBehaviour, IPointerClickHandler {
    private Button _thisButton;
    private Image _thisButtonImage;
    private Text _thisText;
    private string _text;
    private bool _focused;
    private AirVRClientPanelSetting _owner;

    private string stringOfKey(AirVRClientNumberPad.Key key) {
        if (0 <= (int)key && (int)key <= 9) {
            return ((int)key).ToString();
        }
        else if (key == AirVRClientNumberPad.Key.Dot) {
            return ".";
        }
        return "";
    }

    private Sprite _focusedSprite;
    private Sprite _unfocusedSprite;
	
    private void Awake() {
        _thisButton = GetComponent<Button>();
        _thisButtonImage = _thisButton.GetComponent<Image>();
        _thisText = transform.Find("Text").GetComponent<Text>();

        _unfocusedSprite = _thisButtonImage.sprite;
        _focusedSprite = _thisButton.spriteState.highlightedSprite;

        focused = false;
    }

    private void Update()
    {
        if (visible) {
            if (_thisText.text != _text)
            {
                _thisText.text = _text;
            }
        }
    }

    public AirVRClientPanelSetting owner {
        set {
            _owner = value;
        }
    }

    public bool visible {
        get {
            return gameObject.activeInHierarchy;
        }
        set {
            gameObject.SetActive(value);
        }
    }

    public bool focused {
        get {
            return _focused;
        }
        set {
            _focused = value;
            _thisButtonImage.sprite = _focused ? _focusedSprite : _unfocusedSprite;
        }
    }

    public string text {
        get {
            return _text;
        }
        set {
            _text = value;
        }
    }

    public void Input(AirVRClientNumberPad.Key key) {
        if (key == AirVRClientNumberPad.Key.Del) {
            if (string.IsNullOrEmpty(_text) == false) {
                _text = _text.Substring(0, _thisText.text.Length - 1);
            }
        }
        else {
            _text = _text + stringOfKey(key);
        }
    }

    // handle events from children
    public void OnClearClicked() {
        text = "";        
    }

    // implements IPointerClickHandler
    public void OnPointerClick(PointerEventData eventData) {
        if (_owner != null && _owner.CanvasGroup.interactable) {
            _owner.OnInputFieldClick(this);
        }
    }
}
