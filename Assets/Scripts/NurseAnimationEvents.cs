using UnityEngine;

public class NurseAnimationEvents : MonoBehaviour
{
    private NurseAudio nurseAudio;

    private void Awake()
    {
        nurseAudio = GetComponentInParent<NurseAudio>();
    }

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
