/// <summary>
/// Enum to determine cell behaviour
/// </summary>
public enum CellState
{
    Empty, // A cell that can be filled by the player
    Preview, // A cell that is empty and being raycasted to
    Filled // A cell that includes a score
}