using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Serialization
{
    public class SaveDisplayBox : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI displayName;
        [SerializeField] TextMeshProUGUI fileDetails;
        [SerializeField] TextMeshProUGUI playerName;
        [SerializeField] TextMeshProUGUI moneyCount;
        [SerializeField] Button button;

        SaveData assignedSave;

        public void AttachToViewer(SaveInfoViewer viewer) => button.onClick.AddListener(() =>
        {
            if (assignedSave) { viewer.View(assignedSave); }
        });

        public void DisplayFor(SaveData save)
        {
            assignedSave = save;
            displayName.text = save.saveName;
            fileDetails.text = save.creationDate.ToString("g") + $" ({save.FileName})";
            playerName.text = save.playerName + "'s store";
            moneyCount.text = save.money.ToString("c");
        }
	
    }
}
