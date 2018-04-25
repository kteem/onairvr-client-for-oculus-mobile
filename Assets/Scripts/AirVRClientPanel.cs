/***********************************************************

  Copyright (c) 2017-2018 Clicked, Inc.

  Licensed under the MIT license found in the LICENSE file 
  in the root folder of the project.

 ***********************************************************/

using UnityEngine;

public class AirVRClientPanel : MonoBehaviour
{
    [SerializeField] protected CanvasGroup _canvasGroup;
    [SerializeField] protected CanvasGroupFader _canvasGroupFader;

    public CanvasGroup CanvasGroup { get { return _canvasGroup; } }
    public CanvasGroupFader CanvasGroupFader { get { return _canvasGroupFader; } }
}
