
using UnityEngine;

public interface IWaveManager
{
    Transform enterPosition { get; }
    Transform servicePosition { get; }
    Transform leavePosition { get; }
}