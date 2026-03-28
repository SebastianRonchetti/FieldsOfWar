using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] Image color;
    Transform cam;

    void Awake()
    {
        if(gameObject.GetComponentInParent<GameObject>().tag == "Enemy")
        {
            color.color = new Color32(235, 26, 2, 255);
        } else
        {
            color.color = new Color32(41, 149, 11, 255);
        }
    }
    public void setMaxValue(int val)
    {
        slider.maxValue = val;
        slider.value = val;
    }

    public void reduceHP(int val)
    {
        slider.value -= val;
    }

    void LateUpdate()
    {
        if(!cam) cam = FindFirstObjectByType<Camera>().gameObject.transform;
        transform.LookAt(transform.position + cam.forward);
    }
}