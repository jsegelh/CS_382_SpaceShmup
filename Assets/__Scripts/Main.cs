using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Enables the loading & reloading of scenes

public class Main : MonoBehaviour{
    static private Main S;
    static private Dictionary<eWeaponType, WeaponDefinition> WEAP_DICT;

    [Header("Inscribed")]
    public bool spawnEnemies = true;
    public GameObject[] prefabEnemies;          // Array of Enemy prefabs
    public float enemySpawnPerSecond = 0.5f;    // # of Enemies spawned/second
    public float enemyInsetDefault = 1.5f;      // Inset from the sides
    public float gameRestartDelay = 2;
    public WeaponDefinition[] weaponDefinitions;
    private BoundsCheck bndCheck;

    void Awake(){
        S = this;
        // Set bndCkeck to reference teh BoundsCheck component on this GameObject
        bndCheck = GetComponent<BoundsCheck>();

        // Invoke SpawnEnemy() once (in 2 seconds, based on default values)
        Invoke( nameof(SpawnEnemy), 1f/enemySpawnPerSecond);

        // A generic Dicitonary with eWeaponType as the key
        WEAP_DICT = new Dictionary<eWeaponType, WeaponDefinition>();
        foreach( WeaponDefinition def in weaponDefinitions ){
            WEAP_DICT[def.type] = def;
        }
    }
    
    public void SpawnEnemy(){
        // If spawnEnemies is false, skip to the next invoke of SpawnEnemy()
        if(!spawnEnemies){
            Invoke(nameof(SpawnEnemy), 1f / enemySpawnPerSecond);
            return;
        }

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

    void DelayedRestart(){
        // Inovoke the Restart() method in gameRestartDelay seconds
        Invoke(nameof(Restart),gameRestartDelay);
    }

    void Restart(){
        // Reload __Scene_0 to restart the game
        SceneManager.LoadScene("__Scene_0");
    }

    static public void HERO_DIED(){
        S.DelayedRestart();
    }

    /// <summary>
    /// Static function that gets a WeaponDefinition from the WEAP_DICT static
    /// protected field of the Main class.
    /// </summary>
    /// <returns>The WeaponDefinition, or if there is no WeaponDefinition with
    /// the eWeaponType passed in, returns a new WeaponDefinition with a
    /// eWeaponType of eWeaponType.none.</returns>
    static public WeaponDefinition GET_WEAPON_DEFINITION(eWeaponType wt){
        if(WEAP_DICT.ContainsKey(wt)){
            return( WEAP_DICT[wt]);
        }
        // If no entry of the correct type exists in WEAP_DICT, return a new
        // WeaponDefinition with a type of eWeaponType.none (the default value)
        return( new WeaponDefinition() );
    }
}
