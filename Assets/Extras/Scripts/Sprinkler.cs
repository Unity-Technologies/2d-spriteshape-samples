using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class Sprinkler : MonoBehaviour
{

    public GameObject m_Prefab;
    public float m_RandomFactor = 10.0f;

    // Use this for initialization
    void Start ()
    {
        SpriteShapeController ssc = GetComponent<SpriteShapeController>();
        Spline spl = ssc.spline;

        for (int i = 1; i < spl.GetPointCount() - 1; ++i)
        {
            if (Random.Range(0, 100) > m_RandomFactor)
            {
                var go = GameObject.Instantiate(m_Prefab);
                go.transform.position = spl.GetPosition(i);
            }
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
