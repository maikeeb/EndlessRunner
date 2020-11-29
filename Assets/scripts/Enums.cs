using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enums {

    public enum Layer { Default = 0, TransparentFX, IgnoreRaycast, Water = 4, UI, Player = 8, Obstacle  }
    public enum State { Run = 0, Jump, Slide, Count}
}
