// Jen to, co v UnityEngine.Modules chybí (balíček Core RP: Volume).
using System;
namespace UnityEngine.Rendering
{
    public class VolumeComponent : ScriptableObject { }
    public class VolumeProfile : ScriptableObject { public VolumeComponent Add(Type type, bool overrides = false) { return null; } }
    public class Volume : MonoBehaviour { public bool isGlobal; public float priority; public VolumeProfile sharedProfile; }
}
