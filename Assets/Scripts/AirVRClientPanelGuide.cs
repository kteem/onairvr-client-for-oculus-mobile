/***********************************************************

  Copyright (c) 2017-2018 Clicked, Inc.

  Licensed under the MIT license found in the LICENSE file 
  in the root folder of the project.

 ***********************************************************/

using UnityEngine;
using UnityEngine.UI;

public class AirVRClientPanelGuide : AirVRClientPanel
{
    public Image[] cards;
    public Button guideButton;
    public AirVRClientPageControl PageControl;

    private int currentPage;

    private void Start()
    {
        PageControl.SetNumberOfPages(cards.Length);
    }

	private void Update ()
    {
        if (OVRInput.GetDown(OVRInput.Button.One) || OVRInput.GetDown(OVRInput.Button.DpadRight) || Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.LeftControl))
        {
            NextPage();
        }

        if(OVRInput.GetDown(OVRInput.Button.DpadLeft) || Input.GetKeyDown(KeyCode.Z))
        {
            PreviousPage();
        }

        if (OVRInput.GetDown(OVRInput.Button.Back) || Input.GetKeyDown(KeyCode.Escape))
        {
            EndGuide();
        }
	}

    private void NextPage()
    {
        if (currentPage >= cards.Length - 1)
        {
            EndGuide();
            return;                
        }

        cards[currentPage].gameObject.SetActive(false);
        currentPage++;
        cards[currentPage].gameObject.SetActive(true);
        PageControl.SetCurrentPage(currentPage);
    }

    private void PreviousPage()
    {
        if (currentPage < 1)
            return;

        cards[currentPage].gameObject.SetActive(false);
        currentPage--;
        cards[currentPage].gameObject.SetActive(true);
        PageControl.SetCurrentPage(currentPage);
    }

    public void StartGuide()
    {
        AirVRClientUIManager.Instance.SettingPanel.CanvasGroup.interactable = false;       
        AirVRClientUIManager.Instance.SettingPanel.CanvasGroupFader.FadeOut(0.3f);
        AirVRClientUIManager.Instance.DisableAllButton();
        gameObject.SetActive(true);

        foreach (var card in cards)
        {
            card.gameObject.SetActive(false);
        }

        currentPage = 0;
        PageControl.SetCurrentPage(currentPage);
        cards[currentPage].gameObject.SetActive(true);
    }

    public void EndGuide()
    {
        if (AirVRClientAppManager.Instance.Config.FirstPlay)
        {
            AirVRClientAppManager.Instance.Config.Save(AirVRClientAppManager.Instance.Config.ServerAddress, AirVRClientAppManager.Instance.Config.ServerPort, AirVRClientAppManager.Instance.Config.ServerUserID, AirVRClientAppManager.Instance.Config.AutoPlay, false);
        }

        currentPage = 0;
        PageControl.SetCurrentPage(currentPage);

        gameObject.SetActive(false);
        guideButton.interactable = true;

        AirVRClientUIManager.Instance.SettingPanel.CanvasGroupFader.FadeIn(0.3f);
        AirVRClientUIManager.Instance.SettingPanel.CanvasGroup.interactable = true;
        AirVRClientUIManager.Instance.EnableAllButton();
    }    
}
