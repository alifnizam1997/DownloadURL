using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Video;
using System.Collections;
using UnityEngine.Networking;

public class ListFiles : MonoBehaviour
{
    public SQLiteDB database;
    public GameObject item;
    public GameObject previewPanel;
    public GameObject audioText;
    public GameObject pdfText;
    public Transform contentHolder;
    public ScrollRect scroller;
    public Image imagePreview;
    public VideoPlayer vplayer;
    public AudioSource audioSource;

    private Sprite imageSaved;
    private List<string> data = new();
    private int fileCount;

    private void OnEnable()
    {
        data = database.RetrieveAllFiles();
        ListAllFiles();
        scroller.verticalNormalizedPosition = 1;
    }

    private void Start()
    {
        EventManager.current.OnViewButtonClicked += ViewFile;
    }

    private void OnDestroy()
    {
        EventManager.current.OnViewButtonClicked -= ViewFile;
    }

    public void ListAllFiles()
    {
        if (fileCount == data.Count)
        {
            return;
        }

        int counter = contentHolder.childCount - 1;

        for (int i = counter; i >= 0; i--)
        {
            Destroy(contentHolder.GetChild(i).gameObject);
        }

        fileCount = data.Count;

        for (int i = 0; i < data.Count; i += 3)
        {
            FileViewer readFile = Instantiate(item, contentHolder).GetComponent<FileViewer>();
            readFile.fileName = data[i + 0];
            readFile.fileType = data[i + 1];
            readFile.description = data[i + 2];
            readFile.LoadFileText();
        }

        scroller.verticalNormalizedPosition = 1;
    }

    public void ViewFile(string fileName)
    {
        imagePreview.gameObject.SetActive(false);
        vplayer.gameObject.SetActive(false);
        audioText.SetActive(false);
        pdfText.SetActive(false);

        string[] temp = fileName.Split(".");

        previewPanel.SetActive(true);

        if (temp[temp.Length - 1] == "png" || temp[temp.Length - 1] == "jpg")
        {
            ViewImage(fileName);
        }
        else if (temp[temp.Length - 1] == "mp4")
        {
            StartCoroutine(ViewVideo(fileName));
        }
        else if (temp[temp.Length - 1] == "mp3" || temp[temp.Length - 1] == "wav")
        {
            StartCoroutine(ListenAudio(fileName));
        }
        else if (temp[temp.Length - 1] == "pdf")
        {
            pdfText.SetActive(true);
        }
    }

    public void ViewImage(string fileName)
    {
        imagePreview.gameObject.SetActive(true);
        string path = Application.persistentDataPath + "/" + fileName;
        imageSaved = LoadSprite(path);
        imagePreview.sprite = imageSaved;
    }

    private Sprite LoadSprite(string path)
    {
        if (string.IsNullOrEmpty(path))
        { 
            return null;
        }

        if (File.Exists(path))
        {
            byte[] bytes = File.ReadAllBytes(path);
            Texture2D texture = new(1, 1);
            texture.LoadImage(bytes);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            return sprite;
        }

        return null;
    }

    private IEnumerator ViewVideo(string fileName)
    {
        vplayer.gameObject.SetActive(true);
        vplayer.url = Application.persistentDataPath + "/" + fileName;
        vplayer.Prepare();
        Debug.Log("IsPreparing");
        yield return new WaitUntil(() => vplayer.isPrepared);
        Debug.Log("IsComplete");
        vplayer.Play();
    }

    private IEnumerator ListenAudio(string fileName)
    {
        audioText.SetActive(true);
        UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + Application.persistentDataPath + "/" + fileName, AudioType.MPEG);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(www.error);
        }
        else
        {
            audioSource.clip = DownloadHandlerAudioClip.GetContent(www);
            audioSource.Play();
        }
    }
}
