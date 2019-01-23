using System;
using UnityEngine.Events;

[Serializable]
public class StringUnityEvent : UnityEvent<string> {}

[Serializable]
public class IntUnityEvent : UnityEvent<int> {}

[Serializable]
public class PropertyUnityEvent : UnityEvent<IProperty> {}
