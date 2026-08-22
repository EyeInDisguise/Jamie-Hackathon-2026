using UnityEngine;
using UnityEngine.UI;

public class AbilityUI : MonoBehaviour
{
    [Header("Ability Icons")] [SerializeField]
    private Image dashIcon;

    [SerializeField] private Image wallIcon;
    [SerializeField] private Image gravityIcon;
    [SerializeField] private Image timeIcon;

    [Header("Colours")] [SerializeField] private Color inactiveColour = Color.gray;
    [SerializeField] private Color dashColour = Color.cyan;
    [SerializeField] private Color wallColour = new Color(1f, 0.5f, 0f);
    [SerializeField] private Color gravityColour = Color.magenta;
    [SerializeField] private Color timeColour = Color.yellow;

    [Header("Size")] [SerializeField] private float normalScale = 1f;
    [SerializeField] private float activeScale = 1.3f;

    public void SetAbility(int ability)
    {
        // Reset everything
        SetIcon(dashIcon, false, inactiveColour);
        SetIcon(wallIcon, false, inactiveColour);
        SetIcon(gravityIcon, false, inactiveColour);
        SetIcon(timeIcon, false, inactiveColour);

        // Highlight selected ability
        switch (ability)
        {
            case 1:
                SetIcon(dashIcon, true, dashColour);
                break;

            case 2:
                SetIcon(wallIcon, true, wallColour);
                break;

            case 3:
                SetIcon(gravityIcon, true, gravityColour);
                break;

            case 4:
                SetIcon(timeIcon, true, timeColour);
                break;
        }
    }


    private void SetIcon(Image icon, bool active, Color colour)
    {
        icon.color = colour;

        float scale = active ? activeScale : normalScale;
        icon.transform.localScale = Vector3.one * scale;
    }
}