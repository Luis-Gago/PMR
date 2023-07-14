using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PipeSpawnScript : MonoBehaviour
{
    public GameObject pipe;
    public float initialSpawnRate = 2;
    private float spawnRate = 2;
    private float timer = 0;
    public float heightOffset = 2;
    public float minScale = 1f;
    public float maxScale = 10f;
    public Slider widthSlider; // Reference to the WidthSlider in the Unity UI

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
        
        maxScale = widthSlider.value; // Update maxScale based on the WidthSlider value
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



