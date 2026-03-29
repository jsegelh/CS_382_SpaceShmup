using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Enables the loading & reloading of scenes

public class Main : MonoBehaviour{
    static private Main S;

    [Header("Inscribed")]
    public GameObject[] prefabEnemies;          // Array of Enemy prefabs
    public float enemySpawnPerSecond = 0.5f;    // # of Enemies spawned/second
    public float enemyInsetDefault = 1.5f;      // Inset from the sides
    private BoundsCheck bndCheck;

    void Awake(){
        S = this;
        // Set bndCkeck to reference teh BoundsCheck component on this GameObject
        bndCheck = GetComponent<BoundsCheck>();

        // Invoke SpawnEnemy() once (in 2 seconds, based on default values)
        Invoke( nameof(SpawnEnemy), 1f/enemySpawnPerSecond);
    }
    
    public void SpawnEnemy(){
        // Pick a random Enemy prefab to insantiate
        int ndx = Random.Range(0, prefabEnemies.Length);
        GameObject go = Instantiate<GameObject>(prefabEnemies[ndx]);

        // Position the Enemy above the screen with a random x position
        float enemyInset = enemyInsetDefault;
        if(go.GetComponent<BoundsCheck>() != null){
            enemyInset = Mathf.Abs( go.GetComponent<BoundsCheck>().radius);
        }

        // Set the inital position for the spawned Enemy
        Vector3 pos = Vector3.zero;
        float xMin = -bndCheck.camWidth + enemyInset;
        float xMax = bndCheck.camWidth - enemyInset;
        pos.x = Random.Range(xMin,xMax);
        pos.y = bndCheck.camHeight + enemyInset;
        go.transform.position = pos;

        // Invoke SpawnEnemy() again
        Invoke( nameof(SpawnEnemy), 1f/enemySpawnPerSecond);
    }
}
