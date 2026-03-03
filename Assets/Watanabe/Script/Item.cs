using UnityEngine;

public class Item : MonoBehaviour,IPauseResume
{
    public int score = 1;
    public bool inArea = false;
    public int player = 1;
    


    private void OnTriggerEnter2D(Collider2D other)
    {
        Item  currentItem = other.GetComponent<Item>();
        if(currentItem == null ) return;

        if( inArea )
        {
            if ( player == 1 )
            {
                GameManager.Instance.PlayerOneItemGet( score );
                Debug.Log("プレイヤー1のスコア獲得");
            }
            else if( player == 2 )
            {
                GameManager.Instance.PlayerTwoItemGet( score );
                Debug.Log("プレイヤー2のスコア獲得");
            }
            Destroy(gameObject);

        }
    }

    public void Pause()
    {
        
    }

    public void Resume()
    {

    }

}
