using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppManager : MonoBehaviour
{
    public GameObject homePanel;
    public GameObject downloadPanel;
    public GameObject viewPanel;

    void Start()
    {
        EventManager.current.OnDownloadFileButtonClicked += EnableDownloadPanel;
        EventManager.current.OnViewListButtonClicked += EnableViewPanel;
    }

    void OnDestroy()
    {
        EventManager.current.OnDownloadFileButtonClicked -= EnableDownloadPanel;
        EventManager.current.OnViewListButtonClicked -= EnableViewPanel;
    }

    private void EnableDownloadPanel()
    {
        downloadPanel.SetActive(true);
        viewPanel.SetActive(false);
        homePanel.SetActive(false);
    }

    private void EnableViewPanel()
    {
        downloadPanel.SetActive(false);
        viewPanel.SetActive(true);
        homePanel.SetActive(false);
    }
}
