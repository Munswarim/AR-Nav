using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class LocationSearchHandler : MonoBehaviour
{
    public string header;
    public TMP_InputField searchInputField;
    public TMP_Text addressText;
    public TMP_Text latLngText;
    public string dummyLatLng;


    // Start is called before the first frame update
    void Start()
    {
        // Debug.Log($"{searchInputField.text}\n{addressText.text}\n{latLngText.text}");
        searchInputField.onValueChanged.AddListener(FetchData);
        FetchData("");
    }

    void FetchData(string keyword)
    {
        // Debug.Log(keyword);
        //string formatted_address;
        //double lat, lng;

        if (keyword == "")
        {
            addressText.text = $"Awaiting {header} input...";
            // latLngText.text = "";
            latLngText.text = dummyLatLng;
        }
        else if (keyword.ToLower() == "current")
        {
            double lat, lng;
            (lat, lng) = this.GetComponent<CurrentLatLngFinder>().Track();
            addressText.text = "Current address";
            latLngText.text = $"{lat}, {lng}";
        }
        else
        {
            this.GetComponent<PlaceFinderAPI>().Fetch(keyword, (formatted_address, lat, lng) =>
            {
                if (formatted_address != "Place not found.")
                {
                    addressText.text = formatted_address;
                    latLngText.text = $"{lat}, {lng}";
                }
                else
                {
                    addressText.text = "No place of such name...";
                    latLngText.text = "Unavailable...";
                }
            });
        }
        
    }
}
