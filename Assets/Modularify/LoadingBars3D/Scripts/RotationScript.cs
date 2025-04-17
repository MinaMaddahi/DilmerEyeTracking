using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Modularify.LoadingBars3D.Demo
{
    public class RotationScript : MonoBehaviour
    {
        [SerializeField]
        private float _speed = 5f;
        [SerializeField]
        private LoadingBarParts _parts;
        [SerializeField]
        private LoadingBarSegments _segments;
        [SerializeField]
        private LoadingBarStraight _straight;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            transform.Rotate(Vector3.up, Time.deltaTime * _speed);
            float sinTime = Mathf.Sin(Time.time);
            float a = Mathf.InverseLerp(-1, 1, sinTime);
            float b = Mathf.Lerp(0, 1, a);
            _parts.SetPercentage(b);
            _segments.SetPercentage(b);
            _straight.SetPercentage(b);
        }
    }
}

