﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level settings", menuName = "LevelSettings")]
public class LevelSettings : ScriptableObject
{
    [Header("General")]
    /// <summary>
    /// the amount of collums to display in the grid layout
    /// </summary>
    public int TotalCollums;
    /// <summary>
    /// the amount of rows to display in the grid layout
    /// </summary>
    public int TotalRows;
    /// <summary>
    /// the starting amount of rows to display in the grid layout
    /// </summary>
    public int StartingRows;
}
