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
        GameObject prefab = Resources.Load("ChristmasTree1") as GameObject;
        switch (selectedButton.name)
        {
            case "Tree 1 Image Button":
                prefab = Resources.Load("ChristmasTree1") as GameObject;
                break;
            case "Tree 2 Image Button":
                prefab = Resources.Load("ChristmasTree2") as GameObject;
                break;
            case "Tree 3 Image Button":
                prefab = Resources.Load("ChristmasTree3") as GameObject;
                break;
            case "Tree 4 Image Button":
                prefab = Resources.Load("ChristmasTree4") as GameObject;
                break;
        }
        interaction.GetComponent<TapToPlace>().ChangePrefab(prefab);
    }
}
