using CesiumForUnity;
using Google.XR.ARCoreExtensions.GeospatialCreator.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkerSpawner : MonoBehaviour
{
    public GameObject markerPrefab;
    // public GameObject markerPrefab2;

    private void Start()
    {
        string origin = PlayerPrefs.GetString("my origin");

        ARGeospatialCreatorAnchor scriptObj = markerPrefab.GetComponent<ARGeospatialCreatorAnchor>();
        
        string[] parts = origin.Split(new string[] { ", " }, StringSplitOptions.None);

        if (double.TryParse(parts[0], out double lat))
        {
            scriptObj.Latitude = lat;
        }
        if (double.TryParse(parts[1], out double lng))
        {
            scriptObj.Longitude = lng;
        }

    }

    public IEnumerator Spawn(string instruction, double lat, double lng, long angleY=0, long anglePlus=0)
    {
        if (anglePlus != 0)
        {
            angleY += anglePlus;
        }
        GameObject obj = Instantiate(this.markerPrefab);
        obj.transform.eulerAngles += new Vector3(0f, angleY, 0f);

        yield return new WaitForSeconds(0.5f);

        ARGeospatialCreatorAnchor scriptObj = obj.GetComponent<ARGeospatialCreatorAnchor>();

        scriptObj.Latitude = lat;
        scriptObj.Longitude = lng;

        Debug.Log($"{instruction} \n" +
                  $"Expected\t: {lat}, {lng} \n" +
                  $"Actual\t: {scriptObj.Latitude}, {scriptObj.Longitude}");
    }


    /*
    public void Spawn2(double lat, double lng)
    {
        GameObject obj = Instantiate(this.markerPrefab2);
        obj.GetComponent<ARGeospatialCreatorAnchor>().Latitude = lat;
        obj.GetComponent<ARGeospatialCreatorAnchor>().Longitude = lng;
    }
    */
}
