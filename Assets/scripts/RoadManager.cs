using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadManager : Singleton<RoadManager>{


    //EVENTS
    public delegate void AddPieceHandler(GameObject Piece);
    public event AddPieceHandler OnAddPiece;


    [SerializeField]
    GameObject[] LoadedPieces;
    [SerializeField]
    List<GameObject> RoadPieces;
    [SerializeField]
    int numPieces = 10;
    [SerializeField]
    int firstPieceIndex = 4;
    public Transform BeginLeft, BeginRight, EndLeft, EndRight;
    [SerializeField]
    float speed = 20f;
    Vector3 RotationPoint = Vector3.zero;
    public Vector3 GetRotationPoint(Transform BeginLeft, Transform BeginRight, Transform EndLeft, Transform EndRight)
    {


        //computing edges from the corners
        Vector3 beginEdge = BeginLeft.position - BeginRight.position;
        Vector3 endEdge = EndLeft.position - EndRight.position;

        float a = Vector3.Dot(beginEdge, beginEdge);
        float b = Vector3.Dot(beginEdge, endEdge);
        float e = Vector3.Dot(endEdge, endEdge);
        float d = a * e - b * b;

        Vector3 r = BeginLeft.position - EndLeft.position;
        float c = Vector3.Dot(beginEdge, r);
        float f = Vector3.Dot(endEdge, r);
        float s = (b * f - c * e) / d;
        float t = (a * f - c * b) / d;

        Vector3 RotationPointBegin = BeginLeft.position + beginEdge * s;
        Vector3 RotationPointEnd = EndLeft.position + endEdge * t;
        // return midpoint between two closest points
        return (RotationPointBegin + RotationPointEnd) / 2f;

    }
    float GetResetDistance()
    {
        if(RoadPieces[1].tag == Tags.StraightPiece)
        {
            return -EndLeft.transform.position.z;
        } else
        {
            Vector3 endEdge = EndRight.position - EndLeft.position;
            float angle = Vector3.Angle(Vector3.right, endEdge);
            float radius = Mathf.Abs(RotationPoint.x);
            return angle * Mathf.Deg2Rad * radius;
        }
    }

    // Use this for initialization
    void Start () {

        OnAddPiece += x => { };
        LoadedPieces = Resources.LoadAll<GameObject>("RoadPieces");
        RoadPieces = new List<GameObject>();

        RoadPieces.Add(Instantiate(LoadedPieces[firstPieceIndex]) as GameObject);
        RoadPieces.Add(Instantiate(LoadedPieces[firstPieceIndex]) as GameObject);

        RoadPieces[0].AddComponent<MeshCollider>();
        RoadPieces[1].AddComponent<MeshCollider>();

        Vector3 Displacement = RoadPieces[0].transform.Find("EndLeft").position - RoadPieces[1].transform.Find("BeginLeft").position;
        RoadPieces[1].transform.Translate(Displacement, Space.World);
        for (int i = 2; i < numPieces; i++)
        {
            AddPiece();
        }

        RoadPieces[0].transform.parent = RoadPieces[1].transform;
        float halfLength = (RoadPieces[0].transform.Find("BeginLeft").position - RoadPieces[0].transform.Find("EndLeft").position).magnitude / 2;
        RoadPieces[1].transform.Translate(0f, 0f, - halfLength, Space.World);

        SetCurrentPiece();
    }
	void AddPiece()
    {
        int RandomIndex = Random.Range(0, LoadedPieces.Length);
        RoadPieces.Add(Instantiate(LoadedPieces[RandomIndex], RoadPieces[RoadPieces.Count - 1].transform.position, RoadPieces[RoadPieces.Count - 1].transform.rotation));
        //RoadPieces[RoadPieces.Count - 1].AddComponent<MeshCollider>();

        Transform NewPiece = RoadPieces[RoadPieces.Count - 1].transform;
        Transform PrevPiece = RoadPieces[RoadPieces.Count - 2].transform;

        BeginLeft = NewPiece.Find("BeginLeft");
        EndLeft = PrevPiece.Find("EndLeft");
        BeginRight = NewPiece.Find("BeginRight");
        EndRight = PrevPiece.Find("EndRight");

        Vector3 BeginEdge = BeginRight.position - BeginLeft.position;
        Vector3 EndEdge = EndRight.position - EndLeft.position;

        float angle = Vector3.Angle(BeginEdge, EndEdge) * Mathf.Sign(Vector3.Cross(BeginEdge, EndEdge).y);

        NewPiece.Rotate(0, angle, 0, Space.World);

        Vector3 Displacement = EndLeft.position - BeginLeft.position;
        NewPiece.Translate(Displacement, Space.World);

        NewPiece.parent = RoadPieces[1].transform;

        OnAddPiece(NewPiece.gameObject);
    }
    void SetCurrentPiece()
    {

        BeginLeft = RoadPieces[1].transform.Find("BeginLeft");
        EndLeft = RoadPieces[1].transform.Find("EndLeft");
        BeginRight = RoadPieces[1].transform.Find("BeginRight");
        EndRight = RoadPieces[1].transform.Find("EndRight");

        RotationPoint = GetRotationPoint(BeginLeft, BeginRight, EndLeft, EndRight);

    }
    // Update is called once per frame
    void MovePiece(float distance)
    {
        if (RoadPieces[1].tag == Tags.StraightPiece)
        {
            RoadPieces[1].transform.Translate(0f, 0f, -speed * Time.deltaTime, Space.World);
        }
        else
        {
            float radius = Mathf.Abs(RotationPoint.x);
            float angle = ((speed * Time.deltaTime) / radius) * Mathf.Sign(RoadPieces[1].transform.localScale.x) * Mathf.Rad2Deg;
            RoadPieces[1].transform.RotateAround(RotationPoint, Vector3.up, angle);
        }

    }
    void Update () {
        MovePiece(speed * Time.deltaTime);

        if (EndLeft.position.z < 0f || EndRight.position.z < 0f )
        {
            float resetDistance = GetResetDistance();
            MovePiece(-resetDistance);
            CyclePieces();
            MovePiece(resetDistance);
            if(RoadPieces[1].tag == Tags.StraightPiece)
            {
                RoadPieces[1].transform.rotation = new Quaternion(RoadPieces[1].transform.rotation.x,
                                                                  0f, 
                                                                  0f,
                                                                  RoadPieces[1].transform.rotation.w);
                RoadPieces[1].transform.position = new Vector3(0f, 0f, RoadPieces[1].transform.position.z);
            }
        }
    }
    void CyclePieces()
    {
        Destroy(RoadPieces[0]);
        RoadPieces.RemoveAt(0);

        AddPiece();

        for (int i = RoadPieces.Count - 1; i >= 0; i--)
        {
            RoadPieces[i].transform.parent = null;
            RoadPieces[i].transform.parent = RoadPieces[1].transform;

        }
        SetCurrentPiece();
    }
}
