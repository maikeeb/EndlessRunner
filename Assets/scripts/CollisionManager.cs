using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionManager : MonoBehaviour {

    int playerMask;
    GameObject Player;
    CollisionSphere[] collisionSpheres;

    CollisionSphere feet;
    CollisionSphere head;

    Vector3[] collisionSphereSlidePositions;

    bool invincible = false;

    [SerializeField]
    float blinkRate = 0.2f;
    [SerializeField]
    float blinkTime = 3f;

    SkinnedMeshRenderer rend;
    Animator PlayerAnim;
    int SlideParam;
    
    class CollisionSphere
    {
        public Vector3 offset;
        public float radius;

        public CollisionSphere(string name, Vector3 offset, float radius)
        {
            this.offset = offset;
            this.radius = radius;
        }

        public static bool operator >(CollisionSphere lhs, CollisionSphere rhs)
        {
            return lhs.offset.y > rhs.offset.y;
        }
        public static bool operator <(CollisionSphere lhs, CollisionSphere rhs)
        {
            return lhs.offset.y < rhs.offset.y;
        }
        

    }

    class CollisionSphereComparer : IComparer
    {
        public int Compare(object a,object b)
        {
            if(!(a is CollisionSphere) || !(b is CollisionSphere))
            {
                Debug.LogError(Environment.StackTrace);
                throw new ArgumentException("Cannot compare CollisionShperes to non-CollisionSpheres");
            }

            CollisionSphere lhs = (CollisionSphere)a;
            CollisionSphere rhs = (CollisionSphere)b;

            if (lhs < rhs)
            {
                return -1;
            }
            else if (lhs > rhs)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }
	// Use this for initialization
	void Start () {
        Player = GameObject.Find("Robot");
        if (!Player)
        {
            Debug.LogError("Could not find the Player GameObject (serched \"robot\")");
            Destroy(this);
        }

        PlayerAnim = Player.GetComponent<Animator>();
        if (!PlayerAnim)
        {
            Debug.LogError("Animator Component not found on Player");
            Destroy(this);
        }

        SlideParam = Animator.StringToHash("SlideCurve");
        rend = Player.GetComponentInChildren<SkinnedMeshRenderer>();

        if (!rend)
        {
            Debug.LogError("Could not find SkinMeshRenderer component in Player children");
            Destroy(this);
        }
        playerMask = GetLayerMask((int)Enums.Layer.Obstacle);
        //import SphereCollider components into CollisionSphere objects
        SphereCollider[] colliders = Player.GetComponents<SphereCollider>();

        collisionSpheres = new CollisionSphere[colliders.Length];

        for (int i = 0; i < colliders.Length; i++)
        {
            collisionSpheres[i] = new CollisionSphere("Collider "+ i, colliders[i].center, colliders[i].radius);
        }

        Array.Sort(collisionSpheres, new CollisionSphereComparer());

        feet = collisionSpheres[0];
        head = collisionSpheres[collisionSpheres.Length - 1];

        //poistion of collisionspheres mid slide 

        collisionSphereSlidePositions = new Vector3[4];
        collisionSphereSlidePositions[0] = new Vector3(0f, 0.2f, 0.75f);
        collisionSphereSlidePositions[1] = new Vector3(0f, 0.25f, 0.25f);
        collisionSphereSlidePositions[2] = new Vector3(0f, 0.55f, -0.15f);
        collisionSphereSlidePositions[3] = new Vector3(0.4f, 0.7f, -0.28f);


    }
	
	// Update is called once per frame
	void LateUpdate () {
        List<Collider> collisions = new List<Collider>();
        for (int i = 0; i < collisionSpheres.Length; i++)
        {

            Vector3 SlideDisplacement = collisionSphereSlidePositions[i] - collisionSpheres[i].offset;

            SlideDisplacement *= PlayerAnim.GetFloat(SlideParam);

            Vector3 offset = collisionSpheres[i].offset + SlideDisplacement;
                
            foreach (Collider c in Physics.OverlapSphere(Player.transform.position + offset, collisionSpheres[i].radius, playerMask))
            {
                collisions.Add(c);
            }

        }
        {
            

        }

        if (collisions.Count > 0)
        {

            ObstacleCollision();
        }
	}

    private void ObstacleCollision()
    {
        if (!invincible)
        {
            invincible = true;
            StartCoroutine(BlinkPlayer());
        }
            

        
    }

    private IEnumerator BlinkPlayer()
    {
        float startTime = Time.time;
        while (invincible)
        {
            rend.enabled = !rend.enabled;

            if (Time.time >= startTime + blinkTime)
            {
                rend.enabled = true;
                invincible = false;
                
            }
            yield return new WaitForSecondsRealtime(blinkRate);
        }
    }

    int GetLayerMask(params int[] indices)
    {
        int mask = 0;

        for (int i = 0; i < indices.Length; i++)
        {
            mask |= 1 << indices[i];
        }

        return mask;
    }

    int GetLayerIgnoreMask(params int[] indices)
    {
        return ~GetLayerMask(indices);
    }

    void AddLayers(ref int mask, params int[] indices)
    {
        mask |= GetLayerMask(indices);
    }

    void RemoveLayers(ref int mask, params int[] indices)
    {
        mask &= -GetLayerMask(indices);
    }
}
