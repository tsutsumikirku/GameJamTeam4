using JetBrains.Annotations;
using UnityEngine;

public class ScoreArea : MonoBehaviour
{
    public int playerID = 1;
    private void OnTriggerEnter2D(Collider2D other)
    {
        Item item = other.GetComponent<Item>();
        if (item == null) return;
        item.inArea = true;
        item.player = playerID;
        Debug.Log(playerID  + "‚É“ü‚è‚Ü‚µ‚½");
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        Item item =other.GetComponent<Item>();
        if (item == null) return;
        item.inArea = false;
        Debug.Log("ƒGƒŠƒA‚©‚ço‚Ü‚µ‚½");
    }

}
