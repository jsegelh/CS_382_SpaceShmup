using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoundsCheck))]
public class Enemy : MonoBehaviour{
    [Header("Inscribed")]
    public float speed = 10f;       // The movemnt speed is 10m/s
    public float fireRate = 0.3f;   // Seconds/shot 
    public float health = 10;       // Damaage needed to destroy this enemy
    public int score = 100;         // Points earned for destorying this

    public Vector3 pos{
        get{
            return this.transform.position;
        }

        set{
            this.transform.position = value;
        }
    }

    private BoundsCheck bndCheck;

    void Awake(){
        bndCheck = GetComponent<BoundsCheck>();
    }

    // Update is called once per frame
    void Update(){
        Move();

        // Check whether the Enemy has gone off the bottom of the screen
        if(bndCheck.LocIs(BoundsCheck.eScreenLocs.offDown)){
            Destroy( gameObject);
        }
        // if(!bndCheck.isOnScreen){
        //     // We're off the bottom, so destroy this GameObject
        //     Destroy(gameObject);
        // }
    }

    public virtual void Move(){
        Vector3 tempPos = pos;
        tempPos.y -= speed * Time.deltaTime;
        pos = tempPos;
    }
}
