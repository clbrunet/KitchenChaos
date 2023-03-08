using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSound : MonoBehaviour
{
    private Player player;

    [SerializeField] private AudioClip[] footstepClips;

    private void Awake()
    {
        player = GetComponentInParent<Player>();
    }

    private void Start()
    {
        const float FOOTSTEP_INTERVAL = 0.2f;
        InvokeRepeating(nameof(PlayFootstep), 0f, FOOTSTEP_INTERVAL);
    }

    private void PlayFootstep()
    {
        if (!player.IsWalking())
        {
            return;
        }
        SoundManager.Instance.PlayClips(footstepClips, transform.position);
    }
}
