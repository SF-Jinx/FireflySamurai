using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CameraController : MonoBehaviour {

    public float moveTime;
    public Vector3 moveSpeed;
    public float defaultSize;
    private GameObject[] players;
    private GameObject mainCamera;

	void Start () {
        players = GameObject.FindGameObjectsWithTag("Player");
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");

        transform.position = getCentroid(players);
	}
	
	void Update () {
        
        // POSITION
        Vector2 endPosition = getCentroid(players);
        transform.position = Vector3.SmoothDamp(mainCamera.transform.position, endPosition, ref moveSpeed, moveTime);

        //SIZE (ZOOM)
        float size = getPlayfieldSize(players);
        if (size > defaultSize)
        {
            Camera.main.orthographicSize = size;
        }
        else
            Camera.main.orthographicSize = defaultSize;
	}

    Vector2 getCentroid(GameObject[] players)
    {
        float xPosition = 0f;
        float yPosition = 0f;

        for (int i = 0; i < players.Length; i++)
        {
            xPosition += (players[i].transform.position.x);
            yPosition += (players[i].transform.position.y);
        }

        xPosition = xPosition / players.Length;
        yPosition = yPosition / players.Length;

        Vector2 centroid = new Vector2(xPosition, yPosition);
        return centroid;
    }

    float getPlayfieldSize(GameObject[] players)
    {
        float[] xPosition = new float[players.Length];
        float[] yPosition = new float[players.Length];

        for (int i = 0; i < players.Length; i++)
        {
            xPosition[i] = players[i].transform.position.x;
            yPosition[i] = players[i].transform.position.y;
        }

        float distanceX = xPosition.Max() - xPosition.Min();

        float distanceY = yPosition.Max() - yPosition.Min();

        float size = greater(distanceX, distanceY) / 2;
        return size;
    }

    float greater(float x, float y)
    {
        if (x > y) { return x; }
        else return y;
    }
}
