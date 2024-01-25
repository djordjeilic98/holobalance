using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoController : MonoBehaviour
{
    public GameObject avatarLocation;
    public GameObject assets;
    public string animationName;
    public string infoText;
    public bool hasChair;

    SceneControllerCTG sceneController;

    public void ShowInfo()
    {
        if (assets)
        {
            assets.SetActive(true);
        }
        if (!sceneController)
        {
            sceneController = GameManager.Instance.GetComponent<SceneControllerCTG>();
        }

        sceneController.ShowAvatar(true);
        sceneController.avatarCollection.transform.position = avatarLocation.transform.position;
        sceneController.avatarCollection.transform.rotation = avatarLocation.transform.rotation;

        sceneController.chair.SetActive(hasChair);

        sceneController.SendDone(sceneController.ExergameInfo(animationName, infoText));
        //sceneController.StartCoroutine(sceneController.ExergameInfo(animationName, infoText));
    }

    public void HideInfo()
    {
        if (assets)
        {
            assets.SetActive(false);
        }
        sceneController.ShowAvatar(false);
    }
}
