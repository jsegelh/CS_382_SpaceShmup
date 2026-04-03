using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is an enum of the various possible weapon types.
/// It also includes a "shield" type to allow a sheild PowerUp.
/// </summary>
public enum eWeaponType{
    none,       // The default / no weapon
    blaster,    // A simple blaster
    spread,     // Multiple shots simultaneously
    shield,     // Rase shieldLevel
}

/// <summary>
/// The WeaponDefinition calss allows you to set the properties
/// of a specific weapon in the Inspector. The Mian class has
/// an array of WeaponDefinitions that makes this possible.
/// </summary>
[System.Serializable]
public class WeaponDefinition{
    public eWeaponType type = eWeaponType.none;
    [Tooltip("Letter to show on the PowerUp Cube")]
    public string letter;
    [Tooltip("Color of PowerUp Cube")]
    public Color powerUpColor = Color.white;
    [Tooltip("Prefab of Weapon model that is attached to the Player Ship")]
    public GameObject weaponModelPrefab;
    [Tooltip("Prefab of projectile that is fired")]
    public GameObject projectilePrefab;
    [Tooltip("Color of the Projectile that is fired")]
    public Color projectileColor = Color.white;
    [Tooltip("Damage caused when a single Projectile hits an Enemy")]
    public float damageOnHit = 0;
    [Tooltip("Seconds to delay between shots")]
    public float delayBetweenShots = 0;
    [Tooltip("Velocity of individual Projectiles")]
    public float velocity = 50;
}

public class Weapon : MonoBehaviour{
    static public Transform PROJECTILE_ANCHOR;

    [Header("Inscribed")]
    public AudioClip[] fireSounds;
    public AudioClip startupFireSound;

    [Tooltip("The amount of time past the firing delay that causes the startup firing sound to play")]
    public float delayToPlayStartupFireSound = 0.5f;

    [Header("Dynamic")]
    [SerializeField]
    [Tooltip("Setting this manually while playing does not work properly.")]
    private eWeaponType _type = eWeaponType.none;
    public WeaponDefinition def;
    public float nextShotTime; // Time the Weapon will fire next
    // public float fireSoundDuration; // The duration of the longest fireing sound

    private GameObject weaponModel;
    private Transform shotPointTrans;
    private AudioSource audioSource; // This gameObject's audio source

    void Start(){
        // Set up PROJECTILE_ACHOR if it has not already been done
        if(PROJECTILE_ANCHOR == null){
            GameObject go = new GameObject("_ProjectileAnchor");
            PROJECTILE_ANCHOR = go.transform;
        }

        shotPointTrans = transform.GetChild(0);

        // Call SetType() for the default _type set in the Inspector
        SetType( _type );

        // Find the fireEvent of a Hero Component in the parent hierarchy
        Hero hero =  GetComponentInParent<Hero>();
        if (hero != null) hero.fireEvent += Fire;

        // Find the AudioSource on this gameObject
        audioSource = GetComponent<AudioSource>();

        // // Find the longest firing sound length in fireSounds
        // foreach(AudioClip sound in fireSounds){
        //     fireSoundDuration = Mathf.Max(sound.length, fireSoundDuration);
        // }
    }

    public eWeaponType type{
        get {return(_type);}
        set {SetType(value);}
    }

    public void SetType(eWeaponType wt){
        _type = wt;
        if(type == eWeaponType.none){
            this.gameObject.SetActive(false);
            return;
        }
        else{
            this.gameObject.SetActive(true);
        }

        // Get the WeaponDefinition for this type from Main
        def = Main.GET_WEAPON_DEFINITION(_type);
        // Destroy any old model and then attach a model for this weapon
        if(weaponModel != null) Destroy(weaponModel);
        weaponModel = Instantiate<GameObject>(def.weaponModelPrefab, transform);
        weaponModel.transform.localPosition = Vector3.zero;
        weaponModel.transform.localScale = Vector3.one;

        nextShotTime = 0; // You can fire immediately after _type is set.
    }

    private void Fire(){
        // If this.gameObject is inactive, return
        if( !gameObject.activeInHierarchy ) return;
        // If it hasn't been enought time between shots, return
        if ( Time.time < nextShotTime ) return;

        ProjectileHero p;
        Vector3 vel = Vector3.up * def.velocity;

        // Play the firing sound
        PlayFireSound();

        switch (type) {
            case eWeaponType.blaster:
                p = MakeProjectile();
                p.vel = vel;
                break;
            
            case eWeaponType.spread:
                p = MakeProjectile();
                p.vel = vel;
                p = MakeProjectile();
                p.transform.rotation = Quaternion.AngleAxis( 10,  Vector3.back );
                p.vel = p.transform.rotation * vel;
                p = MakeProjectile();
                p.transform.rotation = Quaternion.AngleAxis( -10, Vector3.back);
                p.vel = p.transform.rotation * vel;
                break;
        }
    }

    private ProjectileHero MakeProjectile(){
        GameObject go;
        go = Instantiate<GameObject>(def.projectilePrefab, PROJECTILE_ANCHOR);
        ProjectileHero p = go.GetComponent<ProjectileHero>();

        Vector3 pos = shotPointTrans.position;
        pos.z = 0;
        p.transform.position = pos;

        p.type = type;
        nextShotTime = Time.time + def.delayBetweenShots;
        return (p);
    }

    private void PlayFireSound(){
        // If there is no audio source on this gameObject, do nothing
        if(audioSource == null) return;

        // If there is no fireing sounds in the list, do nothing
        if(fireSounds.Length == 0) return;

        // If the startup firing sound is playing, do nothing
        if(audioSource.isPlaying && audioSource.clip == startupFireSound) return;

        // If the current time has passed the nextShotTime by a certain amount
        // then play the fire start up sound.
        if( (Time.time - nextShotTime) >=  delayToPlayStartupFireSound){
            audioSource.clip = startupFireSound;
        }
        // else play a random continuous fire sound if 
        else{
            audioSource.clip = fireSounds[Random.Range(0, fireSounds.Length)];
        }
        

        // If the longest firing sound can't finish before the next shot is
        // fired, speed up the sound so it can fit.
        // if(fireSoundDuration > def.delayBetweenShots){
        //     audioSource.pitch = audioSource.clip.length / def.delayBetweenShots;
        // } 
        // else{
        //     audioSource.pitch = 1f;
        // }

        // Play the selected sound at the selected speed (a.k.a. pitch)
        audioSource.Play();
    }
}
