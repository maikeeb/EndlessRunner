using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour {
    GameObject[] LoadedObstacles;

    float leftover = 0f;

    [SerializeField]
    float distInterval = 30f;
	// Use this for initialization
	void Awake() {
		//load obstacles prefab
        LoadedObstacles = Resources.LoadAll<GameObject>("Obstacles");

        RoadManager.Instance.OnAddPiece += PlaceObstacles;
	}
	
	
	void PlaceObstacles (GameObject piece) {

        Transform BeginLeft = piece.transform.Find("BeginLeft");
        Transform EndLeft = piece.transform.Find("EndLeft");
        Transform BeginRight = piece.transform.Find("BeginRight");
        Transform EndRight = piece.transform.Find("EndRight");

        float length;

        // curved place variables
        Vector3 RotationPoint = Vector3.zero;
        float radius = 0f;

        if (piece.tag == Tags.StraightPiece)
        {
            length = Vector3.Distance(BeginLeft.position, EndLeft.position);
        }
        else
        {
            // get radius
            RotationPoint = RoadManager.Instance.GetRotationPoint(BeginLeft, BeginRight, EndLeft, EndRight);
            radius = Vector3.Distance(piece.transform.position, RotationPoint);

            // get angle
            float angle = Vector3.Angle(BeginLeft.position - BeginRight.position, EndLeft.position - EndRight.position);

            length = radius * angle * Mathf.Deg2Rad;

        }

       
        float halflength = length / 2f;
        float currDist = distInterval - halflength - leftover;
        if (currDist >= halflength)
        {
            leftover += halflength * 2f;
        }
        for (; currDist < halflength; currDist += distInterval )
        {
            //obstacle container
            GameObject ObstacleRow = new GameObject("ObstacleRow");
            ObstacleRow.transform.position = piece.transform.position;
            ObstacleRow.transform.rotation = piece.transform.rotation;
            ObstacleRow.transform.Rotate(90f, 0f, 0f); // compensate for road piece roation
            ObstacleRow.transform.parent = piece.transform;


            bool consecutive = false;
            int prevIndex = -1;

            for (int i = PlayerController.Instance.NumLanes / -2; i <= PlayerController.Instance.NumLanes /2; i++)
            {
                // prevent 3 of the same obstacles is in a row
                int randomObstacle = Random.Range(0, LoadedObstacles.Length);
                if (randomObstacle == prevIndex)
                {
                    if (!consecutive)
                    {
                        consecutive = true;
                    }
                    else
                    {
                        randomObstacle = ++randomObstacle % LoadedObstacles.Length;
                    }
                }
                else
                {
                    consecutive = false;
                }
                GameObject obstacle = Instantiate(LoadedObstacles[randomObstacle], ObstacleRow.transform.position,
                ObstacleRow.transform.rotation, ObstacleRow.transform);
                obstacle.transform.Translate(Vector3.right * i * PlayerController.Instance.laneWidth, Space.Self);
            }
            //instatiate obstacle prefab
            

            if (piece.tag == Tags.StraightPiece)
            {
                ObstacleRow.transform.Translate(0f, 0f, currDist);
            }
            else
            {
                float angle = currDist / radius;
                ObstacleRow.transform.RotateAround(RotationPoint, Vector3.up, angle * Mathf.Rad2Deg * -Mathf.Sign(piece.transform.localScale.x));
            }
            leftover = halflength - currDist;
        }
	
	}
}
