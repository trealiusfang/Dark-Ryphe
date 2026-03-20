using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class ParallaxHandler : MonoBehaviour
{
    public List<GameObject> parallaxObject;
    public float distancePerParallax = 1;
    public float sizePerDistance = 1.3f;
    public Vector3 firstObjectOffset;
    public Vector2 firstObjectScale;

    public void Update()
    {
        SetParallax();
    }
    
        
    public void SetParallax()
    {
        for (int i = 0; i < parallaxObject.Count; i++)
        {
            Vector3 offset = new Vector3(0, 0,distancePerParallax * i);
            Vector2 size = new Vector2((i == 0 ? 1 :  sizePerDistance * i), (i == 0 ? 1 : sizePerDistance * i));

            parallaxObject[i].transform.position = i == 0 ? firstObjectOffset  :offset;
            parallaxObject[i].transform.localScale = i == 0 ?firstObjectScale :size;
        }
    }
}
