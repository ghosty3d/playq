using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ArrowEffectsController : MonoBehaviour {

    private Transform _transform;
    [SerializeField] private Transform _target;

    [SerializeField] private ParticleSystem _starsParticles;
    
    [SerializeField] private Transform _startPoint;
    [SerializeField] private Transform _endPoint;
    
    void Start() {

        if (_target == null) {
            _target = GameObject.FindGameObjectWithTag("Target").transform;
        }

        _transform = transform;

        _starsParticles.transform.DOMoveZ(_endPoint.transform.position.z, 1f);
        _starsParticles.Play();

        _transform.DOMove(new Vector3(0, 4.5f, -8.5f), 1f).SetEase(Ease.Flash);
        _transform.DOMove(new Vector3(0, 4f, -8f), 1f).SetEase(Ease.OutSine).SetDelay(1f);

        _transform.DOLookAt(_target.position, 3f);
    }
}