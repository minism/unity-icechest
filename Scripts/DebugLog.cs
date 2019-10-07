using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CircularBuffer;
using System.Text;

namespace Ice {

  // UI component which renders all unity debug messages into UI text.
  public class DebugLog : MonoBehaviour {
    private Text textComponent;
    private CircularBuffer<string> buffer = new CircularBuffer<string>(150);

    void Start() {
      textComponent = GetComponentInChildren<Text>();
    }

    void OnEnable() {
      Application.logMessageReceived += HandleLogMessage;
    }

    void OnDisable() {
      Application.logMessageReceived -= HandleLogMessage;
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
      var prefix = PrefixForLogType(type);
      buffer.PushBack(prefix + logLine);
      if (textComponent == null) {
        return;
      }
      textComponent.text = JoinBuffer();
    }

    private static string PrefixForLogType(LogType type) {
      switch (type) {
        case LogType.Assert:
          return "[A] ";
        case LogType.Error:
          return "[E] ";
        case LogType.Exception:
          return "[Ex] ";
        case LogType.Warning:
          return "[W] ";
      }
      return "[I] ";
    }
  }

} // namespace Ice
