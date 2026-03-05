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

        // ���łɃA�C�e��������΃X�R�A�l�����ď���
        if (currentItem != null && currentItem != item)
        {
            GetScore(currentItem);
            Destroy(currentItem.gameObject);
        }

        currentItem = item;
        Debug.Log(playerID + "プレイヤーID");
    }

    void GetScore(Item item)
    {
        if (playerID == 1)
        {
            GameManager.Instance.PlayerOneItemGet(item.score);
            Debug.Log("プレイヤーいち");
        }
        else if (playerID == 2)
        {
            GameManager.Instance.PlayerTwoItemGet(item.score);
            Debug.Log("プレイヤーに");
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
