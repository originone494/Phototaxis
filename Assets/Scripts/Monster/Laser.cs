using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField, Header("旋转速度")] private float rotationSpeed;

    [SerializeField] private Color color = new Color(191 / 255, 36 / 255, 0);
    [SerializeField] private float colorIntensity = 4.3f;
    private float beamColorEnhance = 1;

    [SerializeField, Header("最大长度")] private float maxLength = 100;
    [SerializeField] private float thickness = 9;
    [SerializeField] private float noiseScale = 3.14f;

    [SerializeField] private GameObject startVFX;
    [SerializeField] private GameObject endVFX;

    [SerializeField] private GameObject line;
    private LineRenderer lineRenderer;

    public LayerMask layer;

    private void Awake()
    {
        line.SetActive(true);

        Physics2D.queriesStartInColliders = false;

        lineRenderer = GetComponentInChildren<LineRenderer>();

        lineRenderer.material.color = color * colorIntensity;
        lineRenderer.material.SetFloat("_LaserThickness", thickness);
        lineRenderer.material.SetFloat("_LaserScale", noiseScale);

        ParticleSystem[] particles = transform.GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem p in particles)
        {
            Renderer r = p.GetComponent<Renderer>();
            r.material.SetColor("_EmissionColor", color * (colorIntensity + beamColorEnhance));
        }
    }

    private void Start()
    {
        UpdateEndPosition();
    }

    private void Update()
    {
        transform.rotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z + Time.deltaTime * rotationSpeed);
        UpdateEndPosition();
    }

    private void UpdatePosition(Vector2 startPos, Vector2 dir)
    {
        dir = dir.normalized;
        transform.position = startPos;
        float rotationZ = Mathf.Atan2(dir.y, dir.x);
        transform.rotation = Quaternion.Euler(0, 0, rotationZ * Mathf.Rad2Deg);
    }

    private void UpdateEndPosition()
    {
        Vector2 centerPos = transform.position;
        float rotationZ = transform.rotation.eulerAngles.z;
        rotationZ *= Mathf.Deg2Rad;

        Vector2 direction = new Vector2(Mathf.Cos(rotationZ), Mathf.Sin(rotationZ));

        RaycastHit2D StartHit = Physics2D.Raycast(centerPos, -direction.normalized, maxLength, layer);
        RaycastHit2D EndHit = Physics2D.Raycast(centerPos, direction.normalized, maxLength, layer);

        float startLength = maxLength;
        float endLength = maxLength;
        float laserStartRotation = 180;
        float laserEndRotation = 180;

        if (StartHit)
        {
            startLength = (StartHit.point - centerPos).magnitude;

            laserStartRotation = Vector2.Angle(-direction, StartHit.normal);

            if (StartHit.transform.CompareTag("Player"))
            {
                BeAttackedSystem.Instance.onAttacked(playerControlSystem.Instance.sr.flipX);
            }
        }

        if (EndHit)
        {
            endLength = (EndHit.point - centerPos).magnitude;

            laserEndRotation = Vector2.Angle(direction, EndHit.normal);

            if (EndHit.transform.CompareTag("Player"))
            {
                BeAttackedSystem.Instance.onAttacked(playerControlSystem.Instance.sr.flipX);
            }
        }

        Vector2 startPosition = centerPos - startLength * direction;
        Vector2 endPosition = centerPos + endLength * direction;

        startVFX.transform.position = startPosition;
        endVFX.transform.rotation = Quaternion.Euler(0, 0, laserStartRotation);

        endVFX.transform.position = endPosition;
        endVFX.transform.rotation = Quaternion.Euler(0, 0, laserEndRotation);

        lineRenderer.SetPosition(0, startVFX.transform.position);
        lineRenderer.SetPosition(1, endVFX.transform.position);
    }
}