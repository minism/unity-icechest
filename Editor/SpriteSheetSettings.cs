using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// If this is present in a directory with sprites, sprites in the same directory will automatically
/// be sliced according to these rules.
[CreateAssetMenu()]
public class SpriteSheetSettings : ScriptableObject {
  public int cellWidth;
  public int cellHeight;
}
