
using UnityEngine;

public class TimeStateManager : MonoBehaviour
{
    public enum TimeState
    {
        Present,
        Past
    }

    [Header("Current Time State")]
    [SerializeField] private TimeState currentState = TimeState.Present;

    [Header("Present Objects")]
    [SerializeField] private GameObject[] presentObjects;

    [Header("Past Objects")]
    [SerializeField] private GameObject[] pastObjects;

    private void Start()
    {
        ApplyTimeState();
    }

    private void Update()
    {
        // Detect right mouse button click
        if (Input.GetMouseButtonDown(1))
        {
            ToggleTimeState();
        }
    }

    public void ToggleTimeState()
    {
        currentState = currentState == TimeState.Present
            ? TimeState.Past
            : TimeState.Present;

        ApplyTimeState();

        Debug.Log("Current time state: " + currentState);
    }

    private void ApplyTimeState()
    {
        foreach (GameObject obj in presentObjects)
        {
            if (obj != null)
                obj.SetActive(currentState == TimeState.Present);
        }

        foreach (GameObject obj in pastObjects)
        {
            if (obj != null)
                obj.SetActive(currentState == TimeState.Past);
        }
    }
}
