using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectElement : MonoBehaviour
{
    public GameObject interaction;

    public void onSelect()
    {
        GameObject selectedButton = EventSystem.current.currentSelectedGameObject;
        GameObject prefab = Resources.Load("ChristmasTreeCartoon") as GameObject;
        switch (selectedButton.name)
        {
            case "Tree 1 Image Button":
                prefab = Resources.Load("Tree") as GameObject;
                break;
            case "Tree 2 Image Button":
                prefab = Resources.Load("ChristmasTreeCartoon") as GameObject;
                break;
        }
        interaction.GetComponent<TapToPlace>().ChangePrefab(prefab);
    }
}
