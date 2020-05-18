using SFB;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class IllegalFilenameSearcher : MonoBehaviour
{
    [SerializeField] protected char separator = ',';
    [SerializeField] protected InputField invalidCharactersInput;
    [SerializeField] protected Transform foundItemContainer;
    [SerializeField] protected GameObject foundItemPrefab;

    protected void Awake()
    {
        Debug.Assert(invalidCharactersInput != null, $"{nameof(invalidCharactersInput)}: unassigned");
        Debug.Assert(foundItemContainer != null, $"{nameof(foundItemContainer)}: unassigned");
        Debug.Assert(foundItemPrefab != null, $"{nameof(foundItemPrefab)}: unassigned");
    }

    public void Search()
    {
        // create invalid char list
        string[] invalidCharacters = invalidCharactersInput.text.Split(separator);

        // use StandaloneFileBrowser to get target directory
        string[] paths = StandaloneFileBrowser.OpenFolderPanel("Directory", string.Empty, false);

        // validate a valid directoy was selected
        if (paths == null || paths.Length < 1 || paths[0] == null) return;

        // delete old items
        foreach (Transform item in foundItemContainer) {
            Destroy(item.gameObject);
        }

        // deep search for all directories in path, then very simple Contains() check for invalid characters
        foreach (var directory in Directory.GetDirectories(paths[0], "*", SearchOption.AllDirectories)) {
            foreach (var character in invalidCharacters) {
                if (directory.ToLower().Contains(character.ToLower())) {
                    CreateInput(directory);
                    break;
                }
            }
        }

        // do the same for files
        foreach (var file in Directory.GetFiles(paths[0], "*", SearchOption.AllDirectories)) {
            foreach (var character in invalidCharacters) {
                if (file.ToLower().Contains(character.ToLower())) {
                    CreateInput(file);
                    break;
                }
            }
        }
    }

    protected void CreateInput(string path)
    {
        Debug.Log(path);
        InputField i = Instantiate(foundItemPrefab, foundItemContainer).GetComponentInChildren<InputField>();
        i.text = path;
        i.name = path;
    }
}