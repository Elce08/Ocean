using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Space
{
    Air,
    Water,
}

public enum OnWaterState
{
    Rest,
    Move,
}

public class GameManager : MonoBehaviour
{
    FirstPersonController controller;

    private void Awake()
    {
        controller = FindObjectOfType<FirstPersonController>();
    }
}
