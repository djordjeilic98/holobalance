using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingAppleController : MonoBehaviour {

    public CatchingGameController game;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Fall()
    {
        //Debug.Log("Fall: " + Time.time);

        GameObject fallingApple = Instantiate(game.fallingPrefab, transform.position, Quaternion.identity);
        fallingApple.GetComponent<Rigidbody>().velocity = new Vector3(0, -game.parameters.fallingSpeed, 0);
        fallingApple.GetComponent<Animator>().Play("AppleRed");

        game.spawnedObjects.Add(fallingApple);
    }
}
