using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class VisualRotate : MonoBehaviour
{
    public Transform wheel1;
    public Transform wheel2;

    public float rotationDuration = 2f; // waktu untuk 1 putaran penuh

    void Start()
    {
        // Putar wheel1 360 derajat pada sumbu Z, loop terus menerus
        wheel1.DORotate(new Vector3(0, 0, 360), rotationDuration, RotateMode.FastBeyond360)
            .SetLoops(-1, LoopType.Restart)
            .SetEase(Ease.Linear);

        // Putar wheel2 360 derajat pada sumbu Z, loop terus menerus
        wheel2.DORotate(new Vector3(0, 0, 360), rotationDuration, RotateMode.FastBeyond360)
            .SetLoops(-1, LoopType.Restart)
            .SetEase(Ease.Linear);
    }
}
