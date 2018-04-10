/***********************************************************

  Copyright (c) 2017-2018 Clicked, Inc.

  Licensed under the MIT license found in the LICENSE file 
  in the root folder of the project.

 ***********************************************************/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AirVRClientPageControl : MonoBehaviour
{
    [SerializeField] private Toggle indicatorBase;
    private List<Toggle> indicators = new List<Toggle>();

    void Awake()
    {
        indicators.Add(indicatorBase);   
    }

    public void SetNumberOfPages(int number)
    {
        if (indicators.Count < number)
        {
            for (int i = indicators.Count; i < number; i++)
            {
                Toggle indicator = Instantiate(indicatorBase, indicatorBase.transform.parent);
                indicator.transform.localScale = indicatorBase.transform.localScale;
                indicator.isOn = false;
                indicators.Add(indicator);
            }
        }
        else if(indicators.Count > number)
        {
            for (int i = indicators.Count - 1; i >= number; i--)
            {
                Destroy(indicators[i].gameObject);
                indicators.RemoveAt(i);
            }
        }
    }

    public void SetCurrentPage(int index)
    {
        if (index >= 0 && index <= indicators.Count - 1)
        {
            indicators[index].isOn = true;
        }
    }
}
