/***********************************************************

  Copyright (c) 2017-2018 Clicked, Inc.

  Licensed under the MIT license found in the LICENSE file 
  in the root folder of the project.

 ***********************************************************/

using UnityEngine;
using UnityEngine.UI;

public class AirVRClientPanelSetting : AirVRClientPanel
{
    [SerializeField] private AirVRClientInputField _inputUserID;
    [SerializeField] private AirVRClientInputField _inputAddress;
    [SerializeField] private AirVRClientInputField _inputPort;
    [SerializeField] private AirVRClientNumberPad _numberPad;
    [SerializeField] private AirVRClientAutoPlay _autoPlay;
    [SerializeField] private Button _playButton;

    private PointerOver _userIDPointerOver;
    private PointerOver _addressPointerOver;
    private PointerOver _portPointerOver;
    private PointerOver _numberPadPointerOver;

    public AirVRClientAutoPlay AutoPlay { get { return _autoPlay; } }
    public Button PlayButton { get { return _playButton; } }

    private void SetInputFieldFocus(AirVRClientInputField inputField)
    {
        _inputUserID.focused = inputField == _inputUserID;
        _inputAddress.focused = inputField == _inputAddress;
        _inputPort.focused = inputField == _inputPort;
    }

    public void SetAllInputFieldUnfocus()
    {
        _inputUserID.focused = false;
        _inputAddress.focused = false;
        _inputPort.focused = false;
    }

    private AirVRClientInputField GetInputFieldFocused()
    {
        if (_inputUserID.focused)
            return _inputUserID;

        if (_inputAddress.focused)
            return _inputAddress;

        if (_inputPort.focused)
            return _inputPort;

        return null;
    }

    private void Awake()
    {
        _userIDPointerOver = _inputUserID.GetComponent<PointerOver>();
        _addressPointerOver = _inputAddress.GetComponent<PointerOver>();
        _portPointerOver = _inputPort.GetComponent<PointerOver>();
        _numberPadPointerOver = _numberPad.GetComponent<PointerOver>();

        AirVRClientInputField[] inputFields = GetComponentsInChildren<AirVRClientInputField>(true);

        foreach (AirVRClientInputField inputField in inputFields)
        {
            inputField.owner = this;
        }

        _numberPad.KeyClicked += onNumberPadKeyClicked;
    }

    private void Start()
    {
        if (_inputUserID.gameObject.activeSelf)
            _inputUserID.gameObject.SetActive(false);
        
        _inputUserID.text = AirVRClientAppManager.Instance.Config.ServerUserID.ToString();
        _inputAddress.text = AirVRClientAppManager.Instance.Config.ServerAddress;
        _inputPort.text = AirVRClientAppManager.Instance.Config.ServerPort.ToString();
        _autoPlay.Toggle.isOn = AirVRClientAppManager.Instance.Config.AutoPlay;
    }

    private void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.Up) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            _inputUserID.gameObject.SetActive(!_inputUserID.gameObject.activeSelf);
        }

        if (_numberPad.gameObject.activeSelf)
        {
            if (OVRInput.GetDown(OVRInput.Button.One) || OVRInput.GetDown(OVRInput.Button.Back) ||
                Input.GetKeyDown(KeyCode.LeftControl))
            {
                if (!_numberPadPointerOver.IsPointerOver)
                {
                    if (!_addressPointerOver.IsPointerOver && !_portPointerOver.IsPointerOver &&
                        !_userIDPointerOver.IsPointerOver)
                    {
                        _numberPad.Hide();
                        SetAllInputFieldUnfocus();
                    }
                }
            }
        }
    }

    // handle UI events
    public void OnPlayClicked()
    {
        AirVRClientAppManager.Instance.Connect(_inputAddress.text, _inputPort.text, _inputUserID.text);
    }

    // handle AirVRClientNumberPad events
    private void onNumberPadKeyClicked(AirVRClientNumberPad numberPad, AirVRClientNumberPad.Key key)
    {
        GetInputFieldFocused().Input(key);
    }

    // handle AirVRClientInputField events
    public void OnInputFieldClick(AirVRClientInputField inputField)
    {
        SetInputFieldFocus(inputField);

        _numberPad.Show();
    }
}
