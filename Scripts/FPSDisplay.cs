using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Ice {

  public class FPSDisplay : MonoBehaviour {
    public float averageWindow = 1.0f;

    private Text label;

    private float timer;
    private int frames;
    private float accumulation;

    private void Start() {
      label = GetComponent<Text>();
    }

    private void Update() {
      frames++;
      timer += Time.deltaTime;
      accumulation += 1.0f / Time.deltaTime;

      if (timer >= averageWindow) {
        int fps = Mathf.RoundToInt(accumulation / frames);
        label.text = "FPS: " + fps;

        timer = 0.0f;
        frames = 0;
        accumulation = 0.0f;
      }
    }
  }

} // namespace Ice
