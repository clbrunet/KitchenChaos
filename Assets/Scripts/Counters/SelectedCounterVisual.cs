using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedCounterVisual : MonoBehaviour
{
    [SerializeField] private BaseCounter baseCounter;
    [SerializeField] private GameObject[] selectedMeshes;

    private void OnEnable()
    {
        Player.OnPlayerLocalInstanceSet += Player_OnPlayerLocalInstanceSet;
    }

    private void OnDisable()
    {
        Player.OnPlayerLocalInstanceSet -= Player_OnPlayerLocalInstanceSet;
    }

    private void Player_OnPlayerLocalInstanceSet(object sender, System.EventArgs e)
    {
        Player.LocalInstance.OnSelectedCounterChanged += Player_OnSelectedCounterChanged;
    }

    private void Player_OnSelectedCounterChanged(object sender, Player.OnSelectedCounterChangedEventArgs e)
    {
        foreach (GameObject selectedMesh in selectedMeshes)
        {
            selectedMesh.SetActive(e.selectedCounter == baseCounter);
        }
    }
}
