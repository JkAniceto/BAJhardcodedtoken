using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;

public class WorkerSyncLocation : MonoBehaviour
{
    private string updateLocationURL = "http://127.0.0.1:8000/api/updateLongLat";
    public Button syncLocationButton;

    void Start()
    {
        syncLocationButton.onClick.AddListener(SyncLocation);
    }

    // Method to be called when the sync location button is clicked
    public void SyncLocation()
    {
        StartCoroutine(GetLocation()); // Just call GetLocation directly
    }

    IEnumerator GetLocation()
    {
        // Simulate location data on PC for testing
        float simulatedLatitude = Random.Range(-90f, 90f);
        float simulatedLongitude = Random.Range(-180f, 180f);

        Debug.Log("Simulated Latitude: " + simulatedLatitude + ", Longitude: " + simulatedLongitude);

        // Convert the simulated location data to a JSON string
        string locationJson = JsonUtility.ToJson(new LocationData(simulatedLatitude, simulatedLongitude));

        // Send the simulated location data to the server
        yield return StartCoroutine(UpdateLocation(locationJson));
    }

    IEnumerator UpdateLocation(string locationJson)
    {
        string accessToken = PlayerPrefs.GetString("AccessToken");

        UnityWebRequest request = UnityWebRequest.Post(updateLocationURL, locationJson);
        request.SetRequestHeader("Authorization", "Bearer " + accessToken); // Include the access token in the header
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error updating location: " + request.error);
        }
        else
        {
            if (request.responseCode == 200)
            {
                Debug.Log("Location updated successfully!");
            }
            else
            {
                Debug.LogError("Error updating location. Server response: " + request.responseCode);
            }
        }
    }


    [System.Serializable]
    private class LocationData
    {
        public float latitude;
        public float longitude;

        public LocationData(float lat, float lon)
        {
            latitude = lat;
            longitude = lon;
        }
    }
}
