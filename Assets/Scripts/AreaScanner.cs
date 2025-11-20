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
    private List<Collider> FindedResources;
    public bool CanScan { get; private set; }
    public int ScannedAmount => FindedResources.Count;
    public event Action ScanPerformed;


    private void Start()
    {
        CanScan = true;
        StartCoroutine(StartDelayRountine());
    }

    public Transform GetResourcePosition()
    {
        Transform resource = FindedResources[0].transform;
        FindedResources.RemoveAt(0);
        return resource;
    }

    private void Scan()
    {
        FindedResources = Physics.OverlapBox(transform.position, _scanningBoxAreaSize, Quaternion.identity, _layerMask).ToList();
        SortByDistance();
        CanScan = false;
        ScanPerformed?.Invoke();
    }

    private void SortByDistance()
    {
        FindedResources.Sort((x, y) => { 
            return Vector3.Distance(transform.position, x.transform.position)
            .CompareTo(Vector3.Distance(transform.position, y.transform.position)); });
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
