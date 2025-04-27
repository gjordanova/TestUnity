using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Features", menuName = "BrushFeatures", order = 1)]
public class FeatureData : ScriptableObject
{
  public bool PlayerCollision;
  public bool SkinSelectionScreen;
  public bool DailyRewards;
  public bool CustomFeature;
  public bool DebugMenu;
}
