﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private float _shakeDuration = 0.25f;
    private float _shakeIntensity = 0.1f;
    private float _decreaseFactor = 1.0f;

    private Vector3 _originalPosition;
    private Transform _shakeTransform;

    [SerializeField]
    private bool _shouldCameraShake;

    // Start is called before the first frame update
    void Awake()
    {
        if (_shakeTransform == null)
            _shakeTransform = GetComponent<Transform>() as Transform;
    }

    private void OnEnable()
    {
        _originalPosition = _shakeTransform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (_shouldCameraShake == true)
        {
            CameraShake();
        }
    }

    private void CameraShake()
    {
        if (_shakeDuration > 0)
        {
            // take a random point inside a sphere around _originalPosition and shake the camera in that direction with a defined intensity. (intensity 1.0 means we will reach the point)
            _shakeTransform.localPosition = _originalPosition + Random.insideUnitSphere * _shakeIntensity;
            _shakeDuration -= Time.deltaTime * _decreaseFactor;
        }
        else
        {
            _shouldCameraShake = false;
            _shakeDuration = 0.25f;
            this.gameObject.transform.position = _originalPosition;
            this.enabled = false;
        }
    }
    
    public void StartCameraShake(float intensity)
    {
        _shouldCameraShake = true;
        _shakeIntensity = intensity;
    }
}
