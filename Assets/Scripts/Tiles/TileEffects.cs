using System;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class TileEffects : MonoBehaviour
{
    private bool isLerpingIn = false;
    private bool isLerpingOut = false;
    private int lerpSpeed = 4;
    private Vector3 fallDirection = Vector3.zero;

    public bool startLerpIn = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LerpIn();
    }

    // Update is called once per frame
    void Update()
    {
        if (isLerpingIn)
            LerpIn();
        
        if (isLerpingOut)
            LerpOut();

        if (startLerpIn)
        {
            LerpIn();
            startLerpIn = false;
        }
    }

    public void LerpIn()
    {
        if (!isLerpingIn)
        {
            transform.position += Vector3.down * 10;
            isLerpingIn = true;
        }

        if (transform.position.y < 0)
        {
            float spd = Math.Max(2, Math.Abs(transform.position.y) * lerpSpeed);
            transform.Translate(Vector3.up * (Time.deltaTime * spd));
        }
        else
        {
            transform.position = new Vector3(transform.position.x, 0, transform.position.z);
            isLerpingIn = false;
            LerpOut();
        }
    }

    public void LerpOut()
    {
        if (!isLerpingOut)
        {
            Destroy(gameObject, 2.0f);
            fallDirection = (Vector3.forward * Random.Range(-1f, 1f)) +
                            (Vector3.right * Random.Range(-1f, 1f)) +
                            (Vector3.up * Random.Range(-1f, 1f));
        }

        isLerpingOut = true;
        
        transform.Translate(Physics.gravity * Time.deltaTime, Space.World);
        transform.Rotate(fallDirection);
    }
}
