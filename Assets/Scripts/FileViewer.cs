using UnityEngine;
using TMPro;

public class FileViewer : MonoBehaviour
{
    public string fileName;
    public string description;
    public string fileType;

    public TextMeshProUGUI fileNameText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI fileTypeText;

    public void ViewFile()
    {
        EventManager.current.ViewButtonClicked(fileName);
    }

    public void LoadFileText()
    {
        fileNameText.text = fileName;
        descriptionText.text = description;
        fileTypeText.text = fileType;
    }
}
