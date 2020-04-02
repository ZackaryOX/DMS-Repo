using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    [FMODUnity.EventRef]
    public string _Flicker = "";
    public GameObject _light1;
    public GameObject _light2;



    public float _timer;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Flicker(_light1, _light2));
    }

    // Update is called once per frame
    IEnumerator Flicker(GameObject _light, GameObject _2ndlight)
    {
        _timer = Random.Range(0.7f, 2f);
        _light.SetActive(true);
        _2ndlight.SetActive(true);
        FMODUnity.RuntimeManager.PlayOneShot(_Flicker, GetComponent<Transform>().position);
        yield return new WaitForSeconds(_timer);
        _light.SetActive(false);
        _2ndlight.SetActive(false);
        _timer = Random.Range(0.7f, 2f);
        yield return new WaitForSeconds(_timer);
        StartCoroutine(Flicker(_light, _2ndlight));
    }
}