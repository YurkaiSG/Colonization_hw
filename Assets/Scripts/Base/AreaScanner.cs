using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AreaScanner : MonoBehaviour
{
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private Vector3 _scanningBoxAreaSize = new Vector3(260.0f, 20.0f, 260.0f);
    [SerializeField] private int _cooldown = 10;

    private int _firstScanDelay = 3;
    public bool CanScan { get; private set; }
    public event Action<List<Collider>> Scanned;

    private void Start()
    {
        CanScan = true;
        StartCoroutine(StartDelayRountine());
    }

    private void Scan()
    {
        Scanned?.Invoke(Physics.OverlapBox(transform.position, _scanningBoxAreaSize, Quaternion.identity, _layerMask).ToList());
        CanScan = false;
    }

    private IEnumerator WaitRountine(int waitingTime)
    {
        float interval = 1;
        float timePassed = 0;
        WaitForSeconds wait = new WaitForSeconds(interval);

        while (enabled && timePassed < waitingTime)
        {
            timePassed += interval;
            yield return wait;
        }
    }

    private IEnumerator ScanRoutine()
    {
        while (enabled)
        {
            Scan();
            yield return WaitRountine(_cooldown);
            CanScan = true;
        }
    }

    private IEnumerator StartDelayRountine()
    {
        yield return WaitRountine(_firstScanDelay);
        StartCoroutine(ScanRoutine());
    }
}
