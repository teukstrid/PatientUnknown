using UnityEngine;

public class NurseAnimationEvents : MonoBehaviour
{
    private NurseAudio nurseAudio;

    private void Awake()
    {
        nurseAudio = GetComponentInParent<NurseAudio>();
    }

// Audio for the footsteps of the nurses
    public void Footstep()
    {
        Debug.Log("Footstep animation event received!", this);

        if (nurseAudio != null)
        {
            nurseAudio.Footstep();
        }
        else
        {
            Debug.LogError("NurseAudio was not found in parent!", this);
        }
    }
}
