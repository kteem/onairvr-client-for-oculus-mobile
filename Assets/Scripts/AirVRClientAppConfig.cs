/***********************************************************

  Copyright (c) 2017-2018 Clicked, Inc.

  Licensed under the MIT license found in the LICENSE file 
  in the root folder of the project.

 ***********************************************************/

using System;
using UnityEngine;

public class AirVRClientAppConfig
{
    private const string KeyServerAddress = "ServerAddress";
    private const string KeyServerPort = "ServerPort";
    private const string KeyServerUserID = "ServerUserID";
    private const string KeyAutoPlay = "Autoplay";
    private const string KeyFirstPlay = "FirstPlay";

    private const string DefaultServerHost = "192.168.0.1";
    private const int DefaultServerPort = 9090;
    private const int DefaultUserID = 0;
    private const bool DefaultAutoPlay = false;

    public string ServerAddress { get; private set; }
    public int ServerPort { get; private set; }
    public int ServerUserID { get; private set; }
    public bool AutoPlay { get; private set; }
    public bool FirstPlay { get; private set; }

    public AirVRClientAppConfig()
    {
        Load();        
    }

    public void Load()
    {
        ServerAddress = PlayerPrefs.GetString(KeyServerAddress, DefaultServerHost);
        ServerPort = PlayerPrefs.GetInt(KeyServerPort, DefaultServerPort);
        ServerUserID = PlayerPrefs.GetInt(KeyServerUserID, DefaultUserID);
        AutoPlay = PlayerPrefs.GetInt(KeyAutoPlay, 0) == 1;
        FirstPlay = PlayerPrefs.GetInt(KeyFirstPlay, 1) == 1;
    }

    public void Save(string address, int port, int userID, bool autoPlay, bool firstPlay)
    {
        ServerAddress = address;
        ServerPort = port;
        ServerUserID = userID;
        AutoPlay = autoPlay;
        FirstPlay = firstPlay;

        PlayerPrefs.SetString(KeyServerAddress, ServerAddress);
        PlayerPrefs.SetInt(KeyServerPort, ServerPort);
        PlayerPrefs.SetInt(KeyServerUserID, ServerUserID);
        PlayerPrefs.SetInt(KeyAutoPlay, AutoPlay ? 1:0);
        PlayerPrefs.SetInt(KeyFirstPlay, FirstPlay ? 1 : 0);

        PlayerPrefs.Save();        
    }

    public static bool ValidateIPv4(string ipString)
    {
        var quads = ipString.Split('.');

        if (quads.Length != 4) return false;

        foreach (var quad in quads)
        {
            int q;

            if (!Int32.TryParse(quad, out q)
                || !q.ToString().Length.Equals(quad.Length)
                || q < 0
                || q > 255) { return false; }
        }

        return true;
    }

    public static bool ValidatePort(string portString)
    {
        int port;

        if (!int.TryParse(portString, out port))
        {
            return false;
        }

        return port >= 0 && port <= 65535;
    }

    public static bool ValidateUserID(string userIDString)
    {
        int userID;

        if (!int.TryParse(userIDString, out userID))
        {
            return false;
        }

        return true;
    }
}
