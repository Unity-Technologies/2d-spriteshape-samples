using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

// Dynamic modification of spline to follow the path of mouse movement.
// This script is just a simplified demo to demonstrate the idea.

public class SimpleDraw : MonoBehaviour
{
    public float minimumDistance = 1.0f;
    private Vector3 lastPosition;

	// Use this for initialization
	void Start ()
    {
        
    }
    
	// Update is called once per frame
	void Update ()
    {
        var mp = Input.mousePosition;
        mp.z = 10.0f;
        mp = Camera.main.ScreenToWorldPoint(mp);
        var dt = Mathf.Abs((mp - lastPosition).magnitude);
        var md = (minimumDistance > 1.0f) ? minimumDistance : 1.0f;
        if (Input.GetMouseButton(0) && dt > md)
        {
            var spriteShapeController = gameObject.GetComponent<SpriteShapeController>();
            var spline = spriteShapeController.spline;
            spline.InsertPointAt(spline.GetPointCount(), mp);
            var newPointIndex = spline.GetPointCount() - 1;
            spline.SetTangentMode(newPointIndex, ShapeTangentMode.Linear);

            // BevelCutoff and BevelSize can be used when the point and its neighbor
            // points on both sudes are linear. This creates a nice automatic curve.
            spline.SetBevelCutoff(newPointIndex, 150.0f);
            spline.SetBevelSize(newPointIndex, 0.25f);
            spline.SetHeight(newPointIndex, 1.0f);
            lastPosition = mp;
            Debug.Log(dt);
        }
	}
}
