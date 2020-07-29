using UnityEngine;
using UnityEngine.UI;

public class Weapon : MonoBehaviour
{
    public string weaponName;
    public int bulletsPerMag;
    public int bulletsTotal;
    public int currentBullets;
    public float range;
    public float fireRate;

    private float fireTimer;
    public Transform shootPoint;
    private Animator anim;
    public ParticleSystem muzzleFlash;
    public Text bulletsText;

    private bool isReloading;

    //사운드
    public AudioSource audioSource;
    public AudioClip shootSound;
    public AudioClip reloadSound;

    //프리팹
    public GameObject hitHolePrefab;
    public GameObject hitSparkPrefab;

    // Start is called before the first frame update
    void Start()
    {
        currentBullets = bulletsPerMag;
        anim = GetComponent<Animator>();
        bulletsText.text = currentBullets + "/" + bulletsTotal;
    }

    // Update is called once per frame
    void Update()
    {
        //AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);

        //isReloading = info.IsName("Reload");

        Fire_Button();
        //if (Input.GetButton("Fire1"))
        //{
        //    if (currentBullets > 0)
        //    {
        //        Fire();
        //    }
        //}

        //if (fireTimer < fireRate)
        //{
        //    fireTimer += Time.deltaTime;
        //}
    }
    void Fire_Button()
    {
        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);

        isReloading = info.IsName("Reload");

        if (Input.GetButton("Fire1"))
        {
            if (currentBullets > 0)
            {
                Fire();
            }
            else
            {
                DoReload();
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            DoReload();

        }

        if (fireTimer < fireRate)
        {
            fireTimer += Time.deltaTime;
        }

    }

    void Fire()
    {
        if (fireTimer < fireRate||isReloading)
        {
            return;
        }
        Debug.Log("Fired!");
        RaycastHit hit;
        if (Physics.Raycast(shootPoint.position, shootPoint.transform.forward, out hit, range))
        {
            Debug.Log("Hit!");
            GameObject hitSpark = Instantiate(hitSparkPrefab, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
            Destroy(hitSpark, 0.5f);
            GameObject hitHole = Instantiate(hitHolePrefab, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
            Destroy(hitHole, 5f);
        }
        currentBullets--;
        fireTimer = 0.0f;
        anim.CrossFadeInFixedTime("Fire", 0.01f);
        audioSource.PlayOneShot(shootSound);
        muzzleFlash.Play();
        bulletsText.text = currentBullets + "/" + bulletsTotal;

    }

    void DoReload()
    {
        if (!isReloading && currentBullets < bulletsPerMag && bulletsTotal > 0)
        {
            anim.CrossFadeInFixedTime("Reload", 0.01f);
            audioSource.PlayOneShot(reloadSound);
        }
    }

    public void Reload()
    {
        int bulletsToReload = bulletsPerMag - currentBullets;
        if (bulletsToReload > bulletsTotal)
        {
            bulletsToReload = bulletsTotal;
        }

        currentBullets += bulletsToReload;
        bulletsTotal -= bulletsToReload;
        bulletsText.text = currentBullets + "/" + bulletsTotal;
    }
}
