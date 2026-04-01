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

    protected BoundsCheck bndCheck;

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

    // void OnCollisionEnter(Collision coll){
    //     GameObject otherGO = coll.gameObject;
    //     if(otherGO.GetComponent<ProjectileHero>() != null){
    //         Destroy(otherGO);       // Destroy the Projectile
    //         Destroy(gameObject);    // Destroy this Enemy GameObject
    //     }
    //     else{
    //         Debug.Log( "Enemy hit by non-ProjectileHero: " + otherGO.name);
    //     }
    // }

    void OnCollisionEnter( Collision coll ){
        GameObject otherGO = coll.gameObject;
        // Check for collisions with ProjectileHero
        ProjectileHero p = otherGO.GetComponent<ProjectileHero>();
        if( p != null ){
            // Only damage this Enemy if it's on screen
            if(bndCheck.isOnScreen){
                // Get the damage amount from the Main WEAP_DICT.
                health -= Main.GET_WEAPON_DEFINITION(p.type).damageOnHit;
                if( health <= 0){
                    // Destroy this Enemy
                    Destroy(this.gameObject);
                }
            }

            // Destroy the ProjecitleHero regardless
            Destroy(otherGO);
        }
        else{
            print("Enemy hit by non-ProjectileHero: " + otherGO.name);
        }
    }
}   
