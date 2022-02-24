// dnSpy decompiler from Assembly-CSharp.dll class: ParticleGenerator
using System;
using UnityEngine;

public class ParticleGenerator : MonoBehaviour
{
    public float WaterSpawnTimeInterval = 0.01f;

    public float TapClosingTime = 2.5f;

    private float lastSpawnTime = float.MinValue;

    private Vector3 particleForce;

    private Transform particlesParent;

    public GameObject waterParticle;

    public float scale = 1;

    public Sprite[] waterSprites;

    private Color liquidColor;
    private void Awake()
	{
	}

	private void Start()
	{
		Invoke("TapClose", TapClosingTime);
		particlesParent = transform;
		if (transform.eulerAngles == Vector3.zero)
		{
			particleForce = Vector3.zero;
		}
		if (transform.eulerAngles.z == 90f)
		{
			particleForce = new Vector3(90f, 0f, 0f);
		}
		if (transform.eulerAngles.z == 270f)
		{
			particleForce = new Vector3(-90f, 0f, 0f);
		}
	}

	private void FixedUpdate()
	{
		if (lastSpawnTime + WaterSpawnTimeInterval < Time.time)
		{
            GameObject gameObject = Instantiate(waterParticle);
            gameObject.GetComponent<SpriteRenderer>().sprite = waterSprites[UnityEngine.Random.Range(0, waterSprites.Length)];
            gameObject.GetComponent<SpriteRenderer>().color = liquidColor;
            gameObject.GetComponent<TrailRenderer>().startColor = liquidColor;
            gameObject.GetComponent<TrailRenderer>().endColor = liquidColor;

            gameObject.GetComponent<Rigidbody2D>().AddForce(particleForce);
            gameObject.transform.localScale = Vector3.one * UnityEngine.Random.Range(0.5f, 0.7f);
			DynamicParticle component = gameObject.GetComponent<DynamicParticle>();
			gameObject.transform.position = new Vector3(UnityEngine.Random.Range(transform.position.x - 0.002f, transform.position.x + 0.002f), UnityEngine.Random.Range(transform.position.y - 0.002f, transform.position.y + 0.002f), 0f);
			gameObject.transform.parent = particlesParent;
			lastSpawnTime = Time.time;

            gameObject.transform.localScale = Vector3.one * scale;
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, transform.position.z + 0.5f);
		}
	}

    public void TapOpen(string pieceName)
    {
        liquidColor = LiquidColors.Instance.GetLiquidColor(pieceName);
        CancelInvoke("TapClose");
        this.enabled = true;
        Invoke("TapClose", 0.3f);
    }

	private void TapClose()
	{
		GetComponent<ParticleGenerator>().enabled = false;
	}

}
