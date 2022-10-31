using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.IO;
using TMPro;

public class DownloadFile : MonoBehaviour
{
    /// <summary>
    /// Some sample url to try.
    /// Video = http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4
    /// Image = https://static.wixstatic.com/media/c837a6_e65ba8980e144620b0606bf2c77e5abaf000.jpg/v1/fill/w_980,h_958,al_r,q_85,usm_0.33_1.00_0.00,enc_auto/c837a6_e65ba8980e144620b0606bf2c77e5abaf000.jpg
    /// PDF = https://file-examples.com/storage/feb1825f1e635ae95f6f16d/2017/10/file-sample_150kB.pdf
    /// Audio = https://download.samplelib.com/mp3/sample-3s.mp3
    /// </summary>
    public string url;
    public Image textureImage;
    public Image progressBar;
    public GameObject progressPanel;
    public GameObject downloadPanel;
    public GameObject mainMenu;
    public TextMeshProUGUI progressText;
    public SQLiteDB database;
    public TMP_InputField urlInput;
    public TMP_InputField descInput;
    public TextMeshProUGUI debug;

    [Tooltip("0 = Image, 1 = Video, 2 = PDF, 3 = Others")]
    public int fileType = 0;

    private void Start()
    {
        EventManager.current.OnDownloadButtonClicked += Download;
        fileType = 0;
    }

    private void OnDestroy()
    {
        EventManager.current.OnDownloadButtonClicked -= Download;
    }

    public void ChangeFileType(int value)
    {
        fileType = value;
    }

    private void CompleteDownload()
    {
        string[] splitUrl = url.Split('/');
        database.AddFile(splitUrl[splitUrl.Length - 1], fileType, descInput.text);
        progressText.text = "Complete";
        ResetInput();
        progressPanel.SetActive(false);
        downloadPanel.SetActive(false);
        mainMenu.SetActive(true);
    }

    private void ResetInput()
    {
        url = "";
        urlInput.text = "";
        descInput.text = "";
    }

    private void Download()
    {
        if (string.IsNullOrEmpty(urlInput.text) || string.IsNullOrEmpty(descInput.text))
        {
            debug.text = "One of the field is empty! Please fill out all fields";
            return;
        }
        else
        {
            string[] temp = urlInput.text.Split('/');
            string[] tempEnd = temp[temp.Length - 1].Split('.');

            if (fileType == 0) //Image
            {
                if (tempEnd[tempEnd.Length - 1] != "jpg" && tempEnd[tempEnd.Length - 1] != "png")
                {
                    debug.text = "Wrong file format!";
                    return;
                }
            }
            else if (fileType == 1) //Video
            {
                if (tempEnd[tempEnd.Length - 1] != "mp4")
                {
                    debug.text = "Wrong file format!";
                    return;
                }
            }
            else if (fileType == 2) //Pdf
            {
                if (tempEnd[tempEnd.Length - 1] != "pdf")
                {
                    debug.text = "Wrong file format!";
                    return;
                }
            }
            else if (fileType == 3) //Others
            {
                if (tempEnd[tempEnd.Length - 1] != "mp3" && tempEnd[tempEnd.Length - 1] != "wav")
                {
                    debug.text = "Wrong file format!";
                    return;
                }
            }

        }

        url = urlInput.text;
        debug.text = "";
        progressText.text = "Downloading...";
        Debug.Log("Downloading");
        progressPanel.SetActive(true);
        StartCoroutine(IEDownloadFile());
    }

    private IEnumerator IEDownloadFile()
    {
        if (fileType == 0)
        {
            using UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);

            UnityWebRequestAsyncOperation operation = www.SendWebRequest();
            StartCoroutine(IEHandleProgress(operation));

            yield return new WaitUntil(() => www.isDone);

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Texture2D loadedTexture = DownloadHandlerTexture.GetContent(www);
                textureImage.sprite = Sprite.Create(loadedTexture, new Rect(0f, 0f, loadedTexture.width, loadedTexture.height), Vector2.zero);
                textureImage.SetNativeSize();
                string[] temp = url.Split('/');
                WriteImageOnDisk(temp[temp.Length - 1]);
            }
        }
        else if (fileType == 1)
        {
            using UnityWebRequest www = UnityWebRequest.Get(url);

            UnityWebRequestAsyncOperation operation = www.SendWebRequest();
            StartCoroutine(IEHandleProgress(operation));

            yield return new WaitUntil(() => www.isDone);

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(www.error);
            }
            else
            {
                string[] temp = url.Split('/');
                string path = Path.Combine(Application.persistentDataPath + "/", temp[temp.Length - 1]);
                File.WriteAllBytes(path, www.downloadHandler.data);
                Debug.Log("File written to disk!");
                EventManager.current.DownloadComplete();
                CompleteDownload();
            }
        }
        else if (fileType == 2)
        {
            using UnityWebRequest www = UnityWebRequest.Get(url);

            UnityWebRequestAsyncOperation operation = www.SendWebRequest();
            StartCoroutine(IEHandleProgress(operation));

            yield return new WaitUntil(() => www.isDone);

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(www.error);
            }
            else
            {
                string[] temp = url.Split('/');
                string path = Path.Combine(Application.persistentDataPath + "/", temp[temp.Length - 1]);
                File.WriteAllBytes(path, www.downloadHandler.data);
                Debug.Log("File written to disk!");
                EventManager.current.DownloadComplete();
                CompleteDownload();
            }
        }
        else if (fileType == 3)
        {
            using UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.MPEG);

            UnityWebRequestAsyncOperation operation = www.SendWebRequest();
            StartCoroutine(IEHandleProgress(operation));

            yield return new WaitUntil(() => www.isDone);

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(www.error);
            }
            else
            {
                string[] temp = url.Split('/');
                string path = Path.Combine(Application.persistentDataPath + "/", temp[temp.Length - 1]);
                File.WriteAllBytes(path, www.downloadHandler.data);
                Debug.Log("File written to disk!");
                EventManager.current.DownloadComplete();
                CompleteDownload();
            }
        }
    }

    private void WriteImageOnDisk(string fileName)
    {
        try
        {
            byte[] textureBytes = textureImage.sprite.texture.EncodeToPNG();
            File.WriteAllBytes(Application.persistentDataPath + "/" + fileName, textureBytes);
            Debug.Log("File written to disk!");
            EventManager.current.DownloadComplete();
            CompleteDownload();
        }
        catch (UnassignedReferenceException)
        {
            Debug.Log("Not an image file!");
            progressText.text = "Not an image file!";
        }
    }

    private IEnumerator IEHandleProgress(UnityWebRequestAsyncOperation www)
    {
        while (!www.isDone)
        {
            progressBar.fillAmount = www.progress;

            yield return null;
        }
    }
}
