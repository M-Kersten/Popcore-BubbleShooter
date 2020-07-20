using UnityEngine;

[CreateAssetMenu(fileName = "Cell settings", menuName = "Settings/Cell")]
public class CellSettings : ScriptableObject
{
    [Header("Colors")]
    /// <summary>
    /// the amount of columns to display in the grid layout
    /// </summary>   
    public Color[] scoreColors;
}
