using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteBillboard : MonoBehaviour
{
    [SerializeField] private bool freezeXZAxis = true;
    [SerializeField] Camera camera;

    private void Start()
    {
        Transform parent = transform.parent;
        while (camera == null && parent != null)
        {
            camera = parent.GetComponentInChildren<Camera>();
            parent = parent.parent;
        }
    }

    // Start is called before the first frame update
    private void LateUpdate()
    {
        if (camera == null)
        {
            return;
        }

        if (freezeXZAxis)
        {
            transform.rotation = Quaternion.Euler(0f, camera.transform.rotation.eulerAngles.y, 0f);
        }
        else
        {
            transform.rotation = camera.transform.rotation;
        }
    }
}
