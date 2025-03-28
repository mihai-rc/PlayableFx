using UnityEngine;

namespace PlayableFx
{
    public class PlayableEffectHolder : MonoBehaviour
    {
        [field: SerializeField] public PlayableEffect Effect { get; protected set; }
    }
}