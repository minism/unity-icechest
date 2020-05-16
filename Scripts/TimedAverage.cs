using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Ice {

  public class TimedAverage {
    private float windowSize;
    private float timer;
    private float accumlator;

    public float Average { get; private set; }

    public TimedAverage(float windowSize = 1.0f) {
      this.windowSize = windowSize;
    }

    public void Update(float dt, float value) {
      accumlator += value;
      timer += dt;
      if (timer >= windowSize) {
        Average = accumlator / timer;
        timer -= windowSize;
        accumlator = 0.0f;
      }
    }
  }

} // namespace Ice
