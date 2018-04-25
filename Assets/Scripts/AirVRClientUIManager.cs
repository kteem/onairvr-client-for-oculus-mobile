/***********************************************************

  Copyright (c) 2017-2018 Clicked, Inc.

  Licensed under the MIT license found in the LICENSE file 
  in the root folder of the project.

 ***********************************************************/

using UnityEngine;
using UnityEngine.UI;

public class AirVRClientUIManager : Singleton<AirVRClientUIManager>
{
    [SerializeField] private AirVRClientPanelSetting _settingPanel;
    [SerializeField] private AirVRClientPanelGuide _guidePanel;

    private CanvasGroup _canvasGroup;
    private CanvasGroupFader _canvasGroupFader;

    private Button[] _buttons;

    public CanvasGroup CanvasGroup { get { return _canvasGroup; } }

    public AirVRClientPanelGuide GuidePanel
    {
        get { return _guidePanel; }
    }

    public AirVRClientPanelSetting SettingPanel
    {
        get { return _settingPanel; }
    }

    private void Awake()
    {
        _canvasGroupFader = GetComponent<CanvasGroupFader>();
        _canvasGroup = GetComponent<CanvasGroup>();

        _buttons = GetComponentsInChildren<Button>();

        // 패널 초기화
        _settingPanel.gameObject.SetActive(true);
        _guidePanel.gameObject.SetActive(false);
    }

    public void Show()
    {
        EnableAllButton();
        _canvasGroupFader.SetVisible();
    }

    public void Hide()
    {
        DisableAllButton();
        _canvasGroupFader.SetInvisible();
    }

    public void DisableAllButton()
    {
        foreach (var button in _buttons)
        {
            button.enabled = false;
        }
    }

    public void EnableAllButton()
    {
        foreach (var button in _buttons)
        {
            button.enabled = true;
        }
    }
}
