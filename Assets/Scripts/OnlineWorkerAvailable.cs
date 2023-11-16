using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class Worker
{
    public int id;
    public string first_name;
    public string middle_name;
    public string last_name;
    public string suffix;
    public string gender;
    public string work_category;
    public string latitude;
    public string longitude;
    public string file_path;
    public string file_name;
    public string image_url;
    public int is_online;
}

[System.Serializable]
public class WorkerData
{
    public Worker[] data;
    public int current_page;
    public int from;
    public int last_page;
    public string first_page_url;
    public string last_page_url;
    public string next_page_url;
    public string prev_page_url;
    public int per_page;
    public int to;
    public int total;
}

[System.Serializable]
public class WorkerResponseWrapper
{
    public WorkerData data;
}
public class OnlineWorkerAvailable : MonoBehaviour
{
    public string apiUrl = "http://127.0.0.1:8000/api/getOnlineWorkers";
    public RawImage workerImage;
    public TMP_Text workerFirstNameText;
    public TMP_Text workerMiddleNameText;
    public TMP_Text workerLastNameText;
    public TMP_Text workerStatusText;
    public TMP_Text workerSuffixText;

    public string bearerToken = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiJ9.eyJhdWQiOiIyIiwianRpIjoiMjRmYWU2MDYyYTU0M2MyZjdkMTEyNzkzZDJjZTI4MjI4YTYyMmZmMmVkMGM4MjRlMDgxMjk0MzJjOThiMmE4ZWQzNDg2MjBkZDFmODhlMzEiLCJpYXQiOjE3MDAwNjg5NTMuOTY4NjYzLCJuYmYiOjE3MDAwNjg5NTMuOTY4NjY2LCJleHAiOjE3MzE2OTEzNTMuNzgzNDYsInN1YiI6IjQiLCJzY29wZXMiOlsiKiJdfQ.IuF6rmjUVdH77buPryWRgiDmLUnNV8PaaIFxkbu5Ba8k29QzN76XDLqy5O-A1_sQl-Gt2lG7mSeFJcFugCxKvz3F9MION451gGzAQ3OnhTCp0TXJlu-awlnU9sNXYKu9f5JtJZ8AH9BD3z-xTv-kAvtd-Jestnq2iaYbm-HHw6wWM5U1AWsXqb1Hx8_iAKdwmP2X0BnT_boo8F0jsuFLH2szrkUjiwK20pu5KODvVYlpN__Boy-d1dVK5OBpLGeN6vhhkv2F9HVP2eqo7D8HykHDdUddmO9AjzXZiU7ktUie-v6WukZIyAR8_hl4CF28OEvQeApvh3tO471YcSNSQXWEqSoTEYV6_Y4GXTWFR73IA2RxRG-tJH0ZJQHq4C5gXJR-dFUHg0Okeypkxc_vhMKCO59AyHNPUQCVgyxRw7hmiEABxWKatl3FCc_cEF7VpVnwQefEbp8SYgkd3t95YnW6tIWE_EzEiVUBkXLdvktY8D45wjr4_A0g25A7jPkRXerwbt518b_uBVx-PX6RqCsMI-yn-4AhRVRE9KR4EfjUvzDyxG8s30_vFugEkUMgDXANCzlK-qNVGTF9bHrkRnpARsPHFlPnCCfwDPqcpwHy6A6ZhofzpntHgDprH_hnTiNXom3iyR6-Tw5f-t1zfd6YxolQu4xSWR3VI8H7gTw"; // Replace with your bearer token

    void Start()
    {
        StartCoroutine(GetOnlineWorkers());
    }

    IEnumerator GetOnlineWorkers()
    {
        string getUrl = apiUrl + "?id=2&itemsPerPage=10&page=1"; // Construct the URL with query parameters

        UnityWebRequest request = UnityWebRequest.Get(getUrl);
        request.SetRequestHeader("Authorization", "Bearer " + bearerToken);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string jsonResult = request.downloadHandler.text;
            Debug.Log("Raw API Response: " + jsonResult);

            try
            {
                WorkerResponseWrapper wrapper = JsonUtility.FromJson<WorkerResponseWrapper>(jsonResult);

                if (wrapper != null && wrapper.data != null && wrapper.data.data != null && wrapper.data.data.Length > 0)
                {
                    Debug.Log("Wrapper data data length: " + wrapper.data.data.Length);
                    Debug.Log("Worker data count: " + wrapper.data.data.Length);
                    DisplayWorker(wrapper.data.data[0]); // Display the first worker's information
                }
                else
                {
                    Debug.Log("No workers found in the API response");
                }

            }
            catch (System.Exception e)
            {
                Debug.LogError("Error parsing JSON: " + e.Message);
            }
        }
        else
        {
            Debug.LogError("Error fetching worker data. Response code: " + request.responseCode);
            Debug.LogError("Error details: " + request.error);
            Debug.LogError("Response content: " + request.downloadHandler.text);
        }
    }


    void DisplayWorker(Worker worker)
    {
        workerFirstNameText.text = $"First Name: {worker.first_name.Trim()}";
        workerMiddleNameText.text = $"Middle Name: {worker.middle_name.Trim()}";
        workerLastNameText.text = $"Last Name: {worker.last_name.Trim()}";

        if (!string.IsNullOrEmpty(worker.suffix))
        {
            workerSuffixText.text = $"Suffix: {worker.suffix}";
            workerSuffixText.gameObject.SetActive(true); // Show the suffix text element
        }
        else
        {
            workerSuffixText.gameObject.SetActive(false); // Hide the suffix text element
        }

        if (worker.is_online == 1)
        {
            workerStatusText.text = "Status: Online";
        }
        else
        {
            workerStatusText.text = "Status: Offline";
        }

        StartCoroutine(LoadImage(worker.image_url, workerImage));
    }

    IEnumerator LoadImage(string url, RawImage rawImage)
    {
        UnityWebRequest imageRequest = UnityWebRequestTexture.GetTexture(url);

        yield return imageRequest.SendWebRequest();

        if (imageRequest.result == UnityWebRequest.Result.Success)
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(imageRequest);
            rawImage.texture = texture;
        }
        else
        {
            Debug.LogError("Error loading image: " + imageRequest.error);
        }
    }
}
