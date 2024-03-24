using UIHelper;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] private CustomButton _button;
    private void Start()
    {
        _button.Write("Kakoy pizdets", 0.3f);
    }
}
