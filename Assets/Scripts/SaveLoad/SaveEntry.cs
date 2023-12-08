using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveEntry : MonoBehaviour
{
    private SaveController _saveController;
    private SaveFileDataType _saveData;

    [Tooltip("The name component of the save entry.")]
    public TMP_Text saveName;
    [Tooltip("The thumbnail component of the save entry.")]
    public RawImage thumb;

    public void Start()
    {
        _saveController = PlayerSingleton.Instance.saveController;
    }

    public void Init(SaveFileDataType data, string name, Texture2D thumbTexture = null)
    {
        _saveData = data;
        saveName.SetText(name);
        if (thumbTexture != null)
            thumb.texture = thumbTexture;
    }

    public void Load()
    {
        _saveController.LoadSave(_saveData);
    }

    public void Delete()
    {
        _saveController.DeleteSave(_saveData);
    }
}
