using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DebugRespawnUI : MonoBehaviour
{
    public TMP_Dropdown respawnDropdown;
    private GameManager gameManager;


    void Start()
    {
        gameManager = GameManager.Instance;
        respawnDropdown.ClearOptions();

        var options = new List<string>();
        foreach (var t in gameManager.spawnPoints)
            options.Add(t.name);
        respawnDropdown.AddOptions(options);

        respawnDropdown.onValueChanged.AddListener(OnDropdownChanged);
    }

    void OnDropdownChanged(int index)
    {
        gameManager.RespawnAtIndex(index);
    }
}
