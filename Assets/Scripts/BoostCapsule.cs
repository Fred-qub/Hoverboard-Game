using UnityEngine;

public class BoostCapsule : MonoBehaviour
{
    public int boostCapsuleIndex;
    [SerializeField] private GameObject parentGameObject;
    private void OnTriggerEnter(Collider other)
    {
        //if the player hits a checkpoint, call the checkpoint passed function over in the game manager
        if (other.gameObject.CompareTag("Player"))
        {
            playerController.instance.addBoostResource(20);
        }
    }
}
