/***********************************************************

  Copyright (c) 2017-2018 Clicked, Inc.

  Licensed under the MIT license found in the LICENSE file 
  in the root folder of the project.

 ***********************************************************/

using UnityEngine;
using UnityEngine.VR;

public class AirVRClientAppManager : Singleton<AirVRClientAppManager>, AirVRClient.EventHandler
{    
    [SerializeField] private GameObject _room;
    [SerializeField][RangeAttribute(0.5f, 2.0f)] private float _renderScale = 1f;
    private bool _lastUserPresent = false;

    public bool IsConnecting { get; private set; }
    public AirVRClientNotification Notification { get; private set; }
    public AirVRClientAppConfig Config { get; private set; }
    public AirVRClientInputModule InputModule { get; private set; }

    private void Awake()
    {
        Notification = FindObjectOfType<AirVRClientNotification>();
        Config = new AirVRClientAppConfig();
        InputModule = FindObjectOfType<AirVRClientInputModule>();

        AirVRClient.Delegate = this;
    }

    private void Start()
    {
        UnityEngine.XR.XRSettings.eyeTextureResolutionScale = _renderScale;

        if (Config.FirstPlay)
        {            
            AirVRClientUIManager.Instance.GuidePanel.StartGuide();
        }
    }

    private void Update()
    {    
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(IsConnecting)
                OnDisconnected();
        }
    }

    public void Connect(string addressText, string portText, string userIDText)
    {   
        string message;

        if (!AirVRClientAppConfig.ValidateIPv4(addressText))
        {
            message = "Please enter the correct ip address.";

            Notification.DisplayError(message);
            Debug.Log(message);
            return;
        }

        if (!AirVRClientAppConfig.ValidatePort(portText))
        {
            message = "Please enter the correct port.";

            Notification.DisplayError(message);
            Debug.Log(message);
            return;
        }

        if (!AirVRClientAppConfig.ValidateUserID(userIDText))
        {
            message = "Please enter the correct User ID.";

            Notification.DisplayError(message);
            Debug.Log(message);
            return;
        }

        string address = addressText;
        int port = int.Parse(portText);
        int userID = int.Parse(userIDText);

        Config.Save(address, port, userID, AirVRClientUIManager.Instance.SettingPanel.AutoPlay.Toggle.isOn, Config.FirstPlay);
        AirVRClientUIManager.Instance.CanvasGroup.blocksRaycasts = false;        
        AirVRClientUIManager.Instance.CanvasGroup.interactable = false;

        AirVRClientUIManager.Instance.SettingPanel.PlayButton.enabled = false;

        IsConnecting = true;
        Notification.DisplayConnecting();

        #if UNITY_ANDROID && !UNITY_EDITOR
        AirVRClient.Connect(address, port, userID.ToString());
        #endif
    }

    private void OnDisconnected()
    {
        if (!_room.activeSelf)
            _room.SetActive(true);

        AirVRClientUIManager.Instance.Show();
        AirVRClientUIManager.Instance.SettingPanel.PlayButton.enabled = true;
        AirVRClientUIManager.Instance.CanvasGroup.blocksRaycasts = true;
        AirVRClientUIManager.Instance.CanvasGroup.interactable = true;

        if (IsConnecting)
        {
            string message = "Connection failed.";
            Notification.DisplayError(message);
            Debug.LogError(message);
        }

        IsConnecting = false;
    }

    // implements AirVRClient.EventHandler
    public void AirVRClientFailed(string reason) { }

    public void AirVRClientConnected()
    {
        _room.SetActive(false);
        IsConnecting = false;
        AirVRClientUIManager.Instance.Hide();
        AirVRClient.Play();
    }

    public void AirVRClientPlaybackStarted() { }
    public void AirVRClientPlaybackStopped() { }

    public void AirVRClientDisconnected()
    {
        OnDisconnected();
    }

    public void AirVRClientUserDataReceived(byte[] userData) { }
}
