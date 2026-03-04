using JetBrains.Annotations;
using UnityEngine;

public class ScoreArea : MonoBehaviour
{
    public int playerID = 1;
    Item currentItem;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Item item = other.GetComponent<Item>();
        if (item == null) return;

        // すでにアイテムがあればスコア獲得して消す
        if (currentItem != null && currentItem != item)
        {
            GetScore(currentItem);
            Destroy(currentItem.gameObject);
        }

        currentItem = item;
        Debug.Log(playerID + "に入りました");
    }

    void GetScore(Item item)
    {
        if (item.player == 1)
        {
            GameManager.Instance.PlayerOneItemGet(item.score);
            Debug.Log("プレイヤー1のスコア獲得");
        }
        else if (item.player == 2)
        {
            GameManager.Instance.PlayerTwoItemGet(item.score);
            Debug.Log("プレイヤー2のスコア獲得");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Item item = other.GetComponent<Item>();
        if (item == currentItem)
        {
            currentItem = null;
        }
    }
}
