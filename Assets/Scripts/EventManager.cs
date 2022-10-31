using System;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager current;

    void Awake()
    {
        current = this;
    }

    public event Action OnDownloadFileButtonClicked;

    public void DownloadFileButtonClicked()
    {
        OnDownloadFileButtonClicked?.Invoke();
    }

    public event Action OnViewListButtonClicked;

    public void ViewListButtonClicked()
    {
        OnViewListButtonClicked?.Invoke();
    }

    public event Action OnDownloadButtonClicked;

    public void DownloadButtonClicked()
    {
        OnDownloadButtonClicked?.Invoke();
    }

    public event Action OnDownloadComplete;

    public void DownloadComplete()
    {
        OnDownloadComplete?.Invoke();
    }

    public event Action<string> OnViewButtonClicked;

    public void ViewButtonClicked(string name)
    {
        OnViewButtonClicked?.Invoke(name);
    }

    public event Action OnCompleteRetrieveFiles;
    
    public void CompleteRetrieveFiles()
    {
        OnCompleteRetrieveFiles?.Invoke();
    }
}
