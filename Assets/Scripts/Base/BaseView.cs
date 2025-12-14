using UnityEngine;

public class BaseView : MonoBehaviour
{
    [SerializeField] private Transform _selectionFrame;
    [SerializeField] private BaseColonizator _colonizator;

    private void Awake()
    {
        ChangeActiveState(false);
    }

    private void OnEnable()
    {
        _colonizator.SelectionChanged += ChangeActiveState;
    }

    private void OnDisable()
    {
        _colonizator.SelectionChanged -= ChangeActiveState;
    }

    private void ChangeActiveState(bool state)
    {
        _selectionFrame.gameObject.SetActive(state);
    }
}
