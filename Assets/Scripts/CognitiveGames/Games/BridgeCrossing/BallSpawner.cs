using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallSpawner : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject objectToSpawn;
    public float spawnInterval;

    void Start()
    {
        Invoke("SpawnBall", spawnInterval);
    }

    public void SpawnBall()
    {
        GameObject ball = Instantiate(objectToSpawn, transform.position, transform.rotation, transform);
        ball.GetComponent<Animator>().enabled = false;
        Invoke("SpawnBall", spawnInterval);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
