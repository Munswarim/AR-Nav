using CesiumForUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeospatialOriginInitializer : MonoBehaviour
{

    private string origin;
    // Start is called before the first frame update
    void Start()
    {
        origin = PlayerPrefs.GetString("my origin");

        string[] parts = origin.Split(new string[] { ", " }, StringSplitOptions.None);

        CesiumGeoreference scriptObj = this.GetComponent<CesiumGeoreference>();
        if (double.TryParse(parts[0], out double lat))
        {
            scriptObj.latitude = lat;
        }
        if (double.TryParse(parts[1], out double lng))
        {
            scriptObj.longitude = lng;
        }
    }
}
