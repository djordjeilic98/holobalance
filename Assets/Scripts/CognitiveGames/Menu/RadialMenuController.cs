using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadialMenuController : MonoBehaviour {


    
    public GameObject[] MenuItems;
    public bool HasBackButton;// first item is back button if is checked
    public float AngleOffset = 45f;
    public float CenterOffset = 0.8f;

    public bool OpenOnStart;

    public GameObject centralObject;

    // Use this for initialization
    void Start () {
		if (OpenOnStart)
        {
            OpenMenu(centralObject);
        } else
        {
            foreach (GameObject menuItem in MenuItems)
            {
                menuItem.GetComponent<RadialMenuItem>().Hide();
            }

            if (centralObject)
            {
                //centralObject.GetComponent<RadialMenuItem>().Show();
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OpenMenu(GameObject newCentralObject)
    {
        //if we are in play session mode ignore opening of the menu and wait for the next instruction
        if (!ConnectionManager.Instance().Standalone)
        {
            return;
        }

        float currentAngle = ((MenuItems.Length - (HasBackButton ? 2 : 1)) * AngleOffset) / 2;
        for (int i = 0; i < MenuItems.Length; i++)
        {
            GameObject menuItem = MenuItems[i];

            if (HasBackButton && i == 0)
            {
                menuItem.transform.position = transform.position + new Vector3(0, -CenterOffset);
            }
            else
            {
                menuItem.transform.position = transform.position + new Vector3(0, CenterOffset);
                menuItem.transform.RotateAround(transform.position, transform.forward, currentAngle);
                menuItem.transform.localRotation = Quaternion.identity;
                
                //menuItem.transform.rotation = Quaternion.Euler(0, 0, currentAngle);
                currentAngle -= AngleOffset;
            }
            menuItem.GetComponent<RadialMenuItem>().Show();

            menuItem.GetComponent<TransitionController>().startScale = Vector3.zero;
            menuItem.GetComponent<TransitionController>().endScale = Vector3.one;
            menuItem.GetComponent<TransitionController>().StartTransition(transform.position, menuItem.transform.position);
        }

        centralObject = newCentralObject;
        if (centralObject)
        {
            centralObject.GetComponent<RadialMenuItem>().Show();

            centralObject.GetComponent<TransitionController>().startScale = Vector3.one;
            centralObject.GetComponent<TransitionController>().endScale = new Vector3(0.5f, 0.5f, 0.5f);

            centralObject.GetComponent<TransitionController>().startColor = Color.white;
            centralObject.GetComponent<TransitionController>().endColor = new Color(0.7f, 0.7f, 0.7f);

            centralObject.GetComponent<TransitionController>().StartTransition(centralObject.transform.position, transform.position + transform.forward.normalized * 0.2f, true);

            //centralObject.GetComponent<GazeButton>().enabled = false;

            //Debug.Log("OpenMenu: " + centralObject);
        }

        GameManager.Instance.lastMenu = this.gameObject;
        GameManager.Instance.lastCentralButton = newCentralObject;
    }

    public void CloseMenu()
    {
        foreach (GameObject menuItem in MenuItems)
        {
            menuItem.GetComponent<TransitionController>().startScale = Vector3.one;
            menuItem.GetComponent<TransitionController>().endScale = Vector3.zero;
            menuItem.GetComponent<TransitionController>().StartTransition(menuItem.transform.position, transform.position);

            //menuItem.GetComponent<RadialMenuItem>().Hide();
        }

        if (centralObject)
        {
            centralObject.GetComponent<GazeButton>().enabled = true;

            centralObject.GetComponent<TransitionController>().startColor = Color.white;
            centralObject.GetComponent<TransitionController>().endColor = Color.white;

            centralObject.GetComponent<RadialMenuItem>().Hide();

            //Debug.Log("CloseMenu: " + centralObject);
        }
    }
}
