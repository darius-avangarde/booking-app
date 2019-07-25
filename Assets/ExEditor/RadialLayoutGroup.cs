using UnityEngine;

public class RadialLayoutGroup : MonoBehaviour
{

	public bool showGizmos = true;
	public bool useDegres = false;
	public Transform[] buttons;

	[HideInInspector]
	public float
		separationY,
		separationX,
		separationXdeg;

	[Space]

	[Range(1,20)]
	public int columns;
	[Tooltip("Radius of the defining cylinder")]
	[Range(1,50)]
	public float radius;
	[Range(0,180)]
	public float rotationY;

	private Vector3 offset,	cilinderOrigin, rotationOffsetY;
	private float ang;
	private int k;



	public void PlaceButtons()
	{
		Get3dButtons ();
		offset.y = 0;
		cilinderOrigin = transform.position - transform.forward * radius;
		rotationOffsetY.y = rotationY;

		k = 0;

		if (useDegres)
		{
			separationX = separationXdeg * radius * Mathf.Deg2Rad;
		}
		else
		{
			separationXdeg = separationX / radius * Mathf.Rad2Deg;
		}

		float separationAngle = (useDegres) ? Mathf.Deg2Rad * separationXdeg : separationX/radius;

		for (int i = 0; i < buttons.Length; i++)
		{
			ang = separationAngle*(columns-1)/2 + Mathf.PI/2 - transform.eulerAngles.y * Mathf.Deg2Rad;

			if (k == columns)
			{
				offset.y -= separationY;
				k = 0;
			}

			ang -= separationAngle * k++;
			offset.x = radius * Mathf.Cos (ang);
			offset.z = radius * Mathf.Sin (ang);

			buttons[i].position = cilinderOrigin + offset;
			buttons[i].LookAt(cilinderOrigin + Vector3.up * offset.y);
			buttons [i].Rotate (rotationOffsetY);
		}
	}

	private void OnDrawGizmosSelected()
	{
		if (showGizmos)
		{
			Gizmos.color = Color.black;
			Gizmos.DrawWireSphere (transform.position - (transform.forward * radius), radius);
			Gizmos.DrawLine (transform.position, transform.position - (transform.forward * radius));

			Gizmos.DrawLine (transform.position - (transform.up/2 + transform.right) * columns/2 * Mathf.PI*2*radius*(separationXdeg/360),
				transform.position + (-transform.up/2 + transform.right) * columns/2 * Mathf.PI*2*radius*(separationXdeg/360));
		}
	}
    [ContextMenu("dfgfg")]
    private void Get3dButtons ()
	{
		buttons = new Transform[transform.childCount];

		for (int i = 0; i < transform.childCount; i++)
		{
			buttons [i] = transform.GetChild (i);

		}
	}

}
