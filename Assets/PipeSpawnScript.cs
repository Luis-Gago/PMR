// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class PipeSpawnScript : MonoBehaviour
// {
//     public GameObject pipe;
//     public float spawnRate = 2;
//     private float timer = 0;
//     public float heightOffset = 3;
//     public float minScale = 1f;
//     public float maxScale = 10f;

//     // Start is called before the first frame update
//     void Start()
//     {
//         spawnPipe();
//     }

//     // Update is called once per frame
//     void Update()
//     {
//         if (timer < spawnRate)
//         {
//             timer += Time.deltaTime;
//         }
//         else
//         {
//             spawnPipe();
//             timer = 0;
//             spawnRate = 2;

//         }
//     }

//     void spawnPipe()
//     {
//         float highestPoint = transform.position.y + heightOffset;
//         float lowestPoint = transform.position.y - heightOffset;
//         float randomWidth = Random.Range(minScale, maxScale);
//         spawnRate += (randomWidth * .01f);

//         GameObject newPipe = Instantiate(pipe, new Vector3(transform.position.x, Random.Range(lowestPoint, highestPoint), 0), transform.rotation);
        
//         Vector3 currentScale = newPipe.transform.localScale;
//         newPipe.transform.localScale = new Vector3(randomWidth, currentScale.y, currentScale.z);
//     }
// }

// SECOND ITERATION:
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class PipeSpawnScript : MonoBehaviour
// {
//     public GameObject pipe;
//     public float initialSpawnRate = 2;
//     private float spawnRate = 2;
//     private float timer = 0;
//     public float heightOffset = 3;
//     public float minScale = 1f;
//     public float maxScale = 10f;

//     // Start is called before the first frame update
//     void Start()
//     {
//         spawnPipe();
//     }

//     // Update is called once per frame
//     void Update()
//     {
//         if (timer < spawnRate)
//         {
//             timer += Time.deltaTime;
//         }
//         else
//         {
//             spawnPipe();
//             timer = 0;
//             spawnRate += 0.1f * (maxScale - minScale) / spawnRate; // Adjust the spawn rate based on the width of the pipe
//         }
//     }

//     void spawnPipe()
//     {
//         float highestPoint = transform.position.y + heightOffset;
//         float lowestPoint = transform.position.y - heightOffset;
//         float randomWidth = Random.Range(minScale, maxScale);

//         GameObject newPipe = Instantiate(pipe, new Vector3(transform.position.x, Random.Range(lowestPoint, highestPoint), 0), transform.rotation);
        
//         Vector3 currentScale = newPipe.transform.localScale;
//         newPipe.transform.localScale = new Vector3(randomWidth, currentScale.y, currentScale.z);
//     }
// }

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeSpawnScript : MonoBehaviour
{
    public GameObject pipe;
    public float initialSpawnRate = 2;
    private float spawnRate = 2;
    private float timer = 0;
    public float heightOffset = 3;
    public float minScale = 1f;
    public float maxScale = 10f;

    // Start is called before the first frame update
    void Start()
    {
        spawnPipe();
    }

    // Update is called once per frame
    void Update()
    {
        if (timer < spawnRate)
        {
            timer += Time.deltaTime;
        }
        else
        {
            spawnPipe();
            timer = 0;
            Debug.Log("Spawn Rate: " + spawnRate); // Print spawnRate to console
        }
    }

    void spawnPipe()
    {
        float highestPoint = transform.position.y + heightOffset;
        float lowestPoint = transform.position.y - heightOffset;
        float randomWidth = Random.Range(minScale, maxScale);

        GameObject newPipe = Instantiate(pipe, new Vector3(transform.position.x, Random.Range(lowestPoint, highestPoint), 0), transform.rotation);
        
        Vector3 currentScale = newPipe.transform.localScale;
        newPipe.transform.localScale = new Vector3(randomWidth, currentScale.y, currentScale.z);
        
        spawnRate = initialSpawnRate + randomWidth;
    }
}




