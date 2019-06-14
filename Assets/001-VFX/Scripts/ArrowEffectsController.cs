using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro.EditorUtilities;
using Unity.Collections.LowLevel.Unsafe;

public class ArrowEffectsController : MonoBehaviour {
    private Transform _transform;

    [Header("Arrow Transform:"), SerializeField]
    private Transform _arrowTransform;

    [Header("Target Transform:"), SerializeField]
    private Transform _target;

    [Header("Star Particles:"), SerializeField]
    private ParticleSystem _starsParticles;
    
    [SerializeField] private ParticleSystem _starsTrailsParticles;
    
    [SerializeField] private ParticleSystem _burstParticles;
    
    [SerializeField] private ParticleSystem _pulsarParticles;
    
    [SerializeField] private ParticleSystem _largeTrailsParticles;

    [SerializeField] private Transform _startPoint;
    [SerializeField] private Transform _endPoint;

    private Vector3    _originPosition;
    private Quaternion _originRotation;
    private Vector3 _originScale;
    
    private Vector3    _originArrowPosition;
    private Quaternion _originArrowRotation;
    private Vector3    _originArrowScale;
    

    private Material _arrowMaterial;
    private static readonly int DissolveAmount = Shader.PropertyToID("_DissolveAmount");
    private static readonly int HighlightPower = Shader.PropertyToID("_HighlightPower");
    private static readonly int TextureBlend = Shader.PropertyToID("_TextureBlend");

    void Start() {
        _transform = transform;

        _originPosition = _transform.position;
        _originRotation = _transform.rotation;
        _originScale = _transform.localScale;

        _originArrowPosition = _arrowTransform.position;
        _originArrowRotation = _arrowTransform.rotation;
        _originArrowScale = _arrowTransform.localScale;

        _arrowMaterial = _arrowTransform.GetComponent<Renderer>().sharedMaterial;
        
        if (_arrowMaterial.HasProperty(HighlightPower)) {
            _arrowMaterial.SetFloat(HighlightPower, 0f);
        }
        
        //Shader dissolve
        if (_arrowMaterial.HasProperty(DissolveAmount)) {
            _arrowMaterial.SetFloat(DissolveAmount, 2.0f);
        }
        
        if (_arrowMaterial.HasProperty(TextureBlend)) {
            _arrowMaterial.SetFloat(TextureBlend, 1f);
        }
    }

    public void playEffect() {
        if (_transform == null) {
            _transform = transform;
        }

        if (_target == null) {
            _target = GameObject.FindGameObjectWithTag("Target").transform;
        }
        
        if (_arrowMaterial.HasProperty(HighlightPower)) {
            _arrowMaterial.SetFloat(HighlightPower, 0f);
        }

        var effectSequence = DOTween.Sequence();

        effectSequence.Append(_starsParticles.transform.DOMoveZ(_endPoint.transform.position.z, 1f)).OnPlay(() => {
            _starsParticles.Play();
            
            //Shader dissolve play
            if (_arrowMaterial.HasProperty(DissolveAmount)) {
                _arrowMaterial.DOFloat(-2.0f, "_DissolveAmount", 1f);
            }
        });

        effectSequence.AppendCallback(() => {
            _starsTrailsParticles.Play();
        });
        
        effectSequence.Join(
            _arrowTransform.DOScale(new Vector3(1f, 1f, 2f),0.15f).SetEase(Ease.Flash).SetDelay(0.2f).OnPlay(() => {
                if (_arrowMaterial.HasProperty(HighlightPower)) {
                    _arrowMaterial.DOFloat(1, "_HighlightPower", 0.1f);
                }
                
                _burstParticles.Play();
                _pulsarParticles.Play();
            })
        );

        effectSequence.Append(
            _arrowTransform.DOScale(Vector3.one,0.1f).SetEase(Ease.Flash).OnPlay(() => {
                if (_arrowMaterial.HasProperty(HighlightPower)) {
                    _arrowMaterial.DOFloat(0, "_HighlightPower", 0.1f).SetEase(Ease.Flash);
                    _arrowMaterial.DOFloat(0, "_TextureBlend", 0.1f).SetEase(Ease.Flash);
                }
            })
        );
        
        effectSequence.AppendInterval(0.45f);

        effectSequence.Append(
            _arrowTransform.DOScale(new Vector3(0.1f, 0.1f, 1f), 0.1f)
        );

        effectSequence.Join(
            _arrowTransform.DOMove(_target.position, 0.2f).SetEase(Ease.OutFlash).OnPlay(() => {
                _largeTrailsParticles.transform.position = _arrowTransform.position;
                _largeTrailsParticles.transform.LookAt(_target);
                _largeTrailsParticles.Play();
            })
        );

        _transform.DOMove(new Vector3(0, 4.5f, -8.5f), 1f).SetEase(Ease.Flash);
        _transform.DOMove(new Vector3(0, 4f, -8f), 1f).SetEase(Ease.OutSine).SetDelay(1f);

        _arrowTransform.DOLookAt(_target.position, 3f);
    }

    public void resetEffect() {
        if (_transform == null) {
            _transform = transform;
        }

        _transform.DOComplete();

        _transform.position = _originPosition;
        _transform.rotation = _originRotation;
        _transform.localScale = _originScale;

        _arrowTransform.position = _originArrowPosition;
        _arrowTransform.rotation = _originArrowRotation;
        _arrowTransform.localScale = _originArrowScale;

        _starsParticles.transform.DOComplete();
        _starsParticles.Stop();

        _starsParticles.transform.position = _startPoint.position;
        
        if (_arrowMaterial.HasProperty(HighlightPower)) {
            _arrowMaterial.SetFloat(HighlightPower, 0f);
        }
        
        //Shader dissolve reset
        if (_arrowMaterial.HasProperty(DissolveAmount)) {
            _arrowMaterial.SetFloat(DissolveAmount, 2f);
        }
        
        if (_arrowMaterial.HasProperty(TextureBlend)) {
            _arrowMaterial.SetFloat(TextureBlend, 1f);
        }

        _burstParticles.Stop();
        _pulsarParticles.Stop();
        
        _largeTrailsParticles.Stop();
        
        //Star trails particles
        _starsTrailsParticles.Stop();
    }
}