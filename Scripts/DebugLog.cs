using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CircularBuffer;
using System.Text;
using System;

namespace Ice {

  // UI component which renders all unity debug messages into UI text.
  public class DebugLog : MonoBehaviour {
    public int numLines = 150;

    private Text textComponent;
    private CircularBuffer<string> buffer;

    void Awake() {
      buffer = new CircularBuffer<string>(numLines);
      textComponent = GetComponentInChildren<Text>();
      FixLayout();
    }

    void OnEnable() {
      Application.logMessageReceived += HandleLogMessage;
    }

    void OnDisable() {
      Application.logMessageReceived -= HandleLogMessage;
    }

    [ExposeMethod]
    void FixLayout() {
      textComponent = GetComponentInChildren<Text>();

      var rect = textComponent.GetComponent<RectTransform>();
      rect.anchorMin = Vector2.zero;
      rect.anchorMax = new Vector2(1, 2);
      rect.sizeDelta = Vector2.zero;

      textComponent.alignment = TextAnchor.LowerLeft;
    }

    private string JoinBuffer() {
      StringBuilder builder = new StringBuilder();
      foreach (var line in buffer) {
        builder.Append(line);
        builder.Append("\n");
      }
      return builder.ToString();
    }

    private void HandleLogMessage(string logLine, string stack, LogType type) {
      buffer.PushBack(FormatLogLine(logLine, type));
      if (textComponent == null) {
        return;
      }
      textComponent.text = JoinBuffer();
    }

    private static string FormatLogLine(string logLine, LogType type) {
      var dt = DateTime.Now;
      var ts = dt.ToString("HH:mm:ss");
      switch (type) {
        case LogType.Assert:
          return $"<color=orange>[{ts}] {logLine}</color>";
        case LogType.Error:
          return $"<color=red>[{ts}] {logLine}</color>";
        case LogType.Exception:
          return $"<color=red>[Exception] [{ts}] {logLine}</color>";
        case LogType.Warning:
          return $"<color=yellow>[{ts}] {logLine}</color>";
      }
      return $"[{ts}] {logLine}";
    }
  }

} // namespace Ice
