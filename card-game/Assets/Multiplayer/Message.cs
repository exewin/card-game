using UnityEngine;


public class Message : MonoBehaviour
{
	public int info1;
	public int info2;
	public bool info3;
	
	public int xchar;
	public string customName;
	public int[] deck;
	
	
    public void Stats(int pos, int card, bool bul)
    {
		info1=pos;
		info2=card;
		info3=bul;
	}
	
	public void StartInfo(int a,string b,int[] c)
	{
		xchar=a;
		customName=b;
		deck=c;
		
	}
	
	
    void OnCollisionEnter(Collision collision)
    {
        var hit = collision.gameObject;
        var hitPlayer = hit.GetComponent<PlayerS>();
        if (hitPlayer != null)
        {
            var playerS = hit.GetComponent<PlayerS>();
           
			if(customName!="")
				 playerS.TakePlayerInfo(xchar, customName, deck);
			else
				 playerS.TakeCard(info1, info2, info3);
			
            Destroy(gameObject);
        }
    }
	

}