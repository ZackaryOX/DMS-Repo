using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager
{

    [FMODUnity.EventRef]
    public string[] MusicEventNames;

    [FMODUnity.EventRef]
    public string[] SFXEventNames;


    private List<FMOD.Studio.EventInstance> MusicEventInstances = new List<FMOD.Studio.EventInstance>();
    private List<FMOD.Studio.EventInstance> SFXEventInstances = new List<FMOD.Studio.EventInstance>();
    FMOD.Studio.Bus SFXBus;
    FMOD.Studio.Bus MusicBus;
    FMOD.Studio.Bus MasterBus;
    FMOD.Studio.Bus ReverbBus;


    private float MasterVolume = 1.0f;
    private float MusicVolume = 0.5f;
    private float SFXVolume = 0.5f;
    

    private GameObject HeadObject;

    public AudioManager(string[] sfxtemp, string[] musictemp, GameObject temphead)
    {
        HeadObject = temphead;
        MusicEventNames = musictemp;
        SFXEventNames = sfxtemp;

        Start();

    }
    // Start is called before the first frame update
    void Start()
    {
        SFXBus = FMODUnity.RuntimeManager.GetBus("bus:/Master/SFX");
        MusicBus = FMODUnity.RuntimeManager.GetBus("bus:/Master/Music");
        MasterBus = FMODUnity.RuntimeManager.GetBus("bus:/Master");

        for(int i = 0; i < MusicEventNames.Length; i++)
        {
            FMOD.Studio.EventInstance temp = FMODUnity.RuntimeManager.CreateInstance(MusicEventNames[i]);
            MusicEventInstances.Add(temp);

        }

        for (int i = 0; i < SFXEventNames.Length; i++)
        {
            FMOD.Studio.EventInstance temp = FMODUnity.RuntimeManager.CreateInstance(SFXEventNames[i]);
            SFXEventInstances.Add(temp);
        }


    }
    public void SetSFXVolume(float temp)
    {
        SFXVolume = temp;
        
    }
    public void SetMusicVolume(float temp)
    {
        MusicVolume = temp;
        
    }
    public void SetMasterVolume(float temp)
    {
        MasterVolume = temp;
        
    }

    void AttachInstance(FMOD.Studio.EventInstance tempinstance, GameObject tempobj)
    {
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(tempinstance, tempobj.GetComponent<Transform>(), tempobj.GetComponent<Rigidbody>());
    }
    void PlayInstance(FMOD.Studio.EventInstance tempinstance)
    {
        if (tempinstance.isValid())
        {
            Debug.Log("isvalid");
            tempinstance.start();
        }
    }

    public void PlayMusic()
    {
        for(int i = 0; i < MusicEventInstances.Count; i++)
        {
            AttachInstance(MusicEventInstances[i], HeadObject);
            MusicEventInstances[i].start();
            
        }
       

    }
    // Update is called once per frame
    public void Update(float PlayerSanity)
    {
        MusicEventInstances[0].setParameterByName("Sanity", PlayerSanity);
        MusicBus.setVolume(MusicVolume);
        SFXBus.setVolume(SFXVolume);
        MasterBus.setVolume(MasterVolume);

    }
}
