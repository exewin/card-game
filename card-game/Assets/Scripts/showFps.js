private var waiting2 : Coroutine = null;

private var fps : int;
private var realfps : int;

function OnEnable()
{
	
	waiting2 = StartCoroutine("WaitFPS");
	
}

function Update () 
{
	

	if(waiting2==null)
	{
		realfps=fps;
		fps=0;
		waiting2 = StartCoroutine("WaitFPS");
	}
	
	fps++;
}

function WaitFPS()
{
	yield WaitForSeconds(1);
	waiting2=null;
}


function OnGUI()
{
	GUI.color=Color.red;
	GUI.depth=-1;
	GUI.Label(Rect(4,Screen.height-19,56,28),realfps+"");
}