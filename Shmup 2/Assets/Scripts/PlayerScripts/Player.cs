using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance = null;

    private void Awake() {
        if(CreateSingleton() == false)
            return;
    }

    private bool CreateSingleton() {
        if(Instance == null)
        {
            Instance = this;
            return true;
        }
        else
        {
            Destroy(this);
            return false;
        }
    }
}
