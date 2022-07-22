using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controla el funcionamiento y características de GPS del dispositivo
/// </summary>
public class LocationGps : MonoBehaviour
{
	List<string> places = new List<string> (){"Home","Office"}; //Lugares de localización GPS

	private float currentLongitude=0f; //Longitud actual
	private float currentLatitude=0f; //Latitud actual

	public static LocationGps Instance{ set; get; } //Instancia singleton
    public float originalLatitude; //Latitud inicial
	public float originalLongitude; //longitud inicial
	public float radius; //radio
	public Text range,Distance; //Textos de rango y distancia entre los lugares de localización
	public GameObject Model;
	public Dropdown dropdown;

	public void Awake()
	{		
		Model.SetActive (false);
		range.text="Getting GPS";
		Distance.text = " Distance: ";
	}

	public void Start()
	{
		Instance = this;
		DontDestroyOnLoad (gameObject);
		PopulateList ();
		StartCoroutine (GetCoordinates());
	}

	void PopulateList() 
	{		
		dropdown.AddOptions (places);
	}

	/// <summary>
	/// Corrutina donde se obtienen coordenadas GPS
	/// </summary>
	/// <returns></returns>
	IEnumerator GetCoordinates()
	{
		while (true) 
		{
			if (!Input.location.isEnabledByUser)
			{
				Debug.Log ("Location is Not enabled by user ");
				yield break;
			}

			Input.location.Start (1f, .1f);
			int maxwait = 20;
			while (Input.location.status == LocationServiceStatus.Initializing && maxwait > 0) {
				yield return new WaitForSeconds (1);
				maxwait--;
			}

			if (maxwait <= 0) {
				Debug.Log ("Timed Out");
				yield break;
			}

			if (Input.location.status == LocationServiceStatus.Failed) 
			{
				Debug.Log ("Unable to determine location");
				yield break;
			} 
			else 
			{ 
				currentLatitude = Input.location.lastData.latitude;
				currentLongitude = Input.location.lastData.longitude;
				Vector2 pass = new Vector2 (currentLatitude,currentLongitude);
				Calc (originalLatitude, originalLongitude, currentLatitude, currentLongitude, radius);
			}
		}
	}

	/// <summary>
	/// Realiza calculos de ubicación de un dispositivo por GPS
	/// </summary>
	/// <param name="xc">Coordenada en x</param>
	/// <param name="yc">Coordenada en y</param>
	/// <param name="xp">Coordenada en x</param>
	/// <param name="yp">Coordenada en y</param>
	/// <param name="r">Radio</param>
	public void Calc(float xc, float yc, float xp, float yp,float r)
	{
		float distance=0f,x=0f,y=0f,r1,r2,r3,c;
		float R = 6378.137f;

		r1 = xc * Mathf.Deg2Rad;
		r2 = xp * Mathf.Deg2Rad;
		x = (xp - xc)*Mathf.Deg2Rad;
		y = (yp - yc)*Mathf.Deg2Rad;
		r3 = Mathf.Sin (x / 2) * Mathf.Sin (x / 2) + Mathf.Cos(r1) * Mathf.Cos(r2) * Mathf.Sin(y/2) * Mathf.Sin(y/2);
		c = 2 * Mathf.Atan2(Mathf.Sqrt(r3), Mathf.Sqrt(1-r3)); 
		distance = Mathf.RoundToInt(R * c * 1000f);
		Distance.text = " Distance: "+distance+" m";
		if (distance <= r)
		{
			range.text="In range";
			Model.SetActive (true);
		}
		else
		{			
			range.text="Not In range";
			Model.SetActive (false);
		}
	}

	/// <summary>
	/// Método que se ejecuta en cada momento calculando altitud y longitud
	/// </summary>
	public void Update()
	{  
		currentLatitude = Input.location.lastData.latitude;
		currentLongitude = Input.location.lastData.longitude;
	}

}