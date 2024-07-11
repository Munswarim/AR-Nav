using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GeocodeAddressAPI : MonoBehaviour
{
    public string apiKey;
    public double latitude;
    public double longitude;

    private void Start()
    {
        StartCoroutine(GetPlaceName());
    }

    IEnumerator GetPlaceName()
    {
        // Construct the URL for the Reverse Geocoding API request.
        string baseUrl = "https://maps.googleapis.com/maps/api/geocode/json?";
        string latlng = $"{latitude},{longitude}";
        string url = $"{baseUrl}latlng={latlng}&key={apiKey}";

        // Send the request.
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError)
        {
            Debug.LogError("Error: " + request.error);
        }
        else
        {
            // Parse the JSON response.
            string jsonResponse = request.downloadHandler.text;
            GeocodeNamespace.GeocodingResponse geocodingResponse = JsonUtility.FromJson<GeocodeNamespace.GeocodingResponse>(jsonResponse);

            if (geocodingResponse != null && geocodingResponse.results.Length > 0)
            {
                // Extract the place name (formatted address) from the response.
                string address = geocodingResponse.results[0].formatted_address;

                Debug.Log($"Address: {address}");
            }
            else
            {
                Debug.LogError("Place not found.");
            }
        }
    }
}
namespace GeocodeNamespace
{
    [System.Serializable]
    public class GeocodingResponse
    {
        public GeocodingResult[] results;
    }

    [System.Serializable]
    public class GeocodingResult
    {
        public string formatted_address;
    }
}
