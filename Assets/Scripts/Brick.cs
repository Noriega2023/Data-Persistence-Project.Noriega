using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Brick : MonoBehaviour
{
    public UnityEvent<int> onDestroyed;

    public int PointValue;

    void Start()
    {
        var renderer = GetComponentInChildren<Renderer>();

        MaterialPropertyBlock block = new MaterialPropertyBlock();
        switch (PointValue)
        {
            case 1:
                block.SetColor("_BaseColor", Color.green);
                break;
            case 2:
                block.SetColor("_BaseColor", Color.yellow);
                break;
            case 5:
                block.SetColor("_BaseColor", Color.blue);
                break;
            default:
                block.SetColor("_BaseColor", Color.red);
                break;
        }
        renderer.SetPropertyBlock(block);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (onDestroyed != null) // Aseguramos que haya un listener antes de invocar
        {
            onDestroyed.Invoke(PointValue);
        }
        else
        {
            Debug.LogWarning(" onDestroyed es NULL en " + gameObject.name);
        }

        Debug.Log(" Bloque destruido: " + gameObject.name); //  Verificamos que se destruyan

        // Delay para que la pelota rebote antes de eliminar el bloque
        Destroy(gameObject, 0.2f);
    }
}
