using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostProcessHandle : MonoBehaviour
{
    public Shader defaultFBO;
    public Shader openEyeFBO;
    public Shader SanityFBO;
    Renderer rend;
    bool wait = false;
    // Start is called before the first frame update
    void Start()
    {
       rend = GetComponent<Renderer>();
    }


    // Update is called once per frame
    void Update()
    {
        if (wait == false)
        {
            StartCoroutine(shaderAnim(3.25f));
        }

    }

    IEnumerator shaderAnim(float time)
    {
        rend.material.SetFloat("_Amount", 1.0f);
        for (float t = 1.0f; t >= -1.0f; t -= Time.deltaTime)
        {
            yield return null; // wait 1 frame
            rend.material.SetFloat("_Amount", t);
        }
        rend.material.SetFloat("_Amount", -1.0f);
        wait = true;
        rend.material.shader = defaultFBO;
    }

}
